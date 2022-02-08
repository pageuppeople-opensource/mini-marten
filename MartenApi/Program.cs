using Marten;
using Marten.Events.Projections;
using Marten.Storage;
using MartenApi.EventStore.Document;
using MartenApi.EventStore.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMarten(options =>
{
    options.Connection(
        "Server=localhost;Port=5432;Database=marten;User Id=postgres;Password=postgres;");

    options.Policies.AllDocumentsAreMultiTenanted();
    options.Advanced.DefaultTenantUsageEnabled = false;
    options.Events.TenancyStyle = TenancyStyle.Conjoined;

    options.Events.MetadataConfig.HeadersEnabled = true;
    options.Events.MetadataConfig.CorrelationIdEnabled = true;
    options.Events.MetadataConfig.CausationIdEnabled = true;

    options.Projections.Add<DocumentAggregation>(ProjectionLifecycle.Live);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build sessions per request
// TODO: Throw in a tenant manager
builder.Services.AddScoped<IMartenSessionFactory, MartenSessionFactory>();

// As these services dont need anything injected themselves, they can be singletons
builder.Services.AddSingleton<IDocumentService, DocumentService>();


var app = builder.Build();

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
