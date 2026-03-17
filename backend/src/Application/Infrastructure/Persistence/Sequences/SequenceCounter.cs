using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Infrastructure.Persistence.Sequences;

public sealed class SequenceCounter
{
    [BsonId]
    public string Name { get; set; } = default!;

    public int Value { get; set; }
}