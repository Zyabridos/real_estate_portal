using System.Net.Http.Json;

namespace RealEstate.Tests.Integration.Infrastructure;

public static class HttpContentExtensions
{
    public static Task<T?> ReadFromJsonTestAsync<T>(this HttpContent content, CancellationToken ct = default)
        => content.ReadFromJsonAsync<T>(TestJson.Options, ct);
}