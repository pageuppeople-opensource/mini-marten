// Set the defaults for the project
using StronglyTypedIds;

[assembly: StronglyTypedIdDefaults(
    backingType: StronglyTypedIdBackingType.Long,
    converters: StronglyTypedIdConverter.NewtonsoftJson)]

namespace MartenApi.EventStore;

[StronglyTypedId]
public partial struct DocumentId { }

[StronglyTypedId]
public partial struct UserId { }