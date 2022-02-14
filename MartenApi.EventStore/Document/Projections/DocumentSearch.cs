using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Marten;
using Marten.Events;
using Marten.Events.CodeGeneration;
using Marten.Linq;
using Marten.Schema;

namespace MartenApi.EventStore.Document.Projections;

public record DocumentSearch(
    long DocumentId,
    [property: Identity] string StreamKey,
    [property: DuplicateField] string Owner,
    [property: FullTextIndex] string Title,
    [property: DuplicateField] DateTimeOffset LastModified)
{
    public static DocumentSearch Create(DocumentCreated @event, IEvent metadata)
    {
        return new DocumentSearch(@event.DocumentId, metadata.StreamKey!, @event.Owner, @event.Title,
            metadata.Timestamp);
    }

    public DocumentSearch Apply(DocumentTitleUpdated @event, DocumentSearch current, IEvent metadata)
    {
        return current with {Title = @event.Title, LastModified = metadata.Timestamp};
    }

    public DocumentSearch Apply(DocumentOwnerChanged @event, DocumentSearch current, IEvent metadata)
    {
        return current with {Owner = @event.NewOwner, LastModified = metadata.Timestamp};
    }

    public DocumentSearch Apply(DocumentSearch current, IEvent @event)
    {
        return current with {LastModified = @event.Timestamp};
    }
}

//public class CompiledTimeline : ICompiledListQuery<DocumentSearch>, IQueryPlanning
//{
//    private string? _searchQuery;
//    public int PageSize { get; set; } = 10;

//    [MartenIgnore] public int Page { private get; set; } = 1;
//    public int SkipCount => (Page - 1) * PageSize;

//    [MartenIgnore]
//    public string? SearchQuery
//    {
//        get => _searchQuery;
//        set => _searchQuery = string.IsNullOrWhiteSpace(value)
//            ? null
//            : $"{Regex.Replace(value.Trim(), @"[^a-zA-Z0-9]+", "&", RegexOptions.ECMAScript | RegexOptions.Compiled)}:*";
//    }

//    // And hey, if you have a public QueryStatistics member on your compiled
//    // query class, you'll get the total number of records
//    //public QueryStatistics Statistics { get; } = new QueryStatistics();

//    public Expression<Func<IMartenQueryable<DocumentSearch>, IEnumerable<DocumentSearch>>> QueryIs()
//    {
//        return query => query.Where(x => SearchQuery == null || x.PlainTextSearch(SearchQuery) ||)
//            .Skip(SkipCount).Take(PageSize);
//    }

//    public void SetUniqueValuesForQueryPlanning()
//    {
//        Page = 3; // Setting Page to 3 forces the SkipCount and PageSize to be different values
//        PageSize = 20; // This has to be a positive value, or the Take() operator has no effect
//        SearchQuery = Guid.NewGuid().ToString();
//    }
//}