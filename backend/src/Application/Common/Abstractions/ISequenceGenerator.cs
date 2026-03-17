namespace RealEstate.Application.Common.Abstractions;

public interface ISequenceGenerator
{
    Task<int> GetNextValueAsync(string sequenceName, CancellationToken ct);
}