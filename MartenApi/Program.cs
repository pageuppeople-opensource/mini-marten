using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Storage;
using MartenApi.EventStore;
using MartenApi.EventStore.Command;
using MartenApi.EventStore.Impl;
using MartenApi.EventStore.Impl.Id;
using MartenApi.EventStore.Query;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMarten(options =>
{
    options.Connection(
        "Server=localhost;Port=5432;Database=marten;User Id=postgres;Password=postgres;");

    // Multi tenancy
    options.Policies.AllDocumentsAreMultiTenanted();
    //options.Advanced.DefaultTenantUsageEnabled = false;
    options.Events.TenancyStyle = TenancyStyle.Conjoined;

    // Enable metadata
    options.Events.MetadataConfig.HeadersEnabled = true;
    options.Events.MetadataConfig.CorrelationIdEnabled = true;
    options.Events.MetadataConfig.CausationIdEnabled = true;

    // Use string types as streamids. Generated ids will be guids serialised as string.
    options.Events.StreamIdentity = StreamIdentity.AsString;

    // Add entity ids
    options.Storage.ConfigureEntityIdsForEventStore();

    // Add projections
    options.Projections.ConfigureProjectionsForEventStore();

}).AddAsyncDaemon(builder.Environment.IsDevelopment() ? DaemonMode.Solo : DaemonMode.HotCold);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build sessions per request
// TODO: Throw in a tenant manager
builder.Services.AddScoped<IMartenSessionFactory, MartenSessionFactory>();
builder.Services.AddScoped<IDocumentQueryService, DocumentQueryService>();
builder.Services.AddScoped<IDocumentCommandService, DocumentCommandService>();

// As these services don't need anything injected themselves (apart from other singletons) they can be singletons
builder.Services.AddSingleton<IIdProvider, IdProvider>();

var app = builder.Build();

await (app.Services.GetService<IDocumentStore>() as DocumentStore)!.Schema.ApplyAllConfiguredChangesToDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();