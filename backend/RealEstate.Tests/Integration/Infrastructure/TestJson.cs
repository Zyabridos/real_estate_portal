using System.Text.Json;
using System.Text.Json.Serialization;

namespace RealEstate.Tests.Integration.Infrastructure;

public static class TestJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true) }
    };
}