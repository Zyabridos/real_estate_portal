namespace RealEstate.Domain.Entities;

public sealed class PropertyImage
{
    public string Id { get; set; } = default!;
    public string PropertyId { get; set; } = default!;
    public string Url { get; set; } = default!;
    public int SortOrder { get; set; }
}