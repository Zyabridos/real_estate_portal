using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace RealEstate.Infrastructure.Mongo.Conventions;

public static class MongoConventions
{
    private static bool _registered;

    public static void Register()
    {
        // Prevent multiple registrations since MongoDB serializers and conventions are global
        if (_registered) return;
        _registered = true;

        // Convention pack
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(), // PascalCase -> camelCase
            new EnumRepresentationConvention(BsonType.String), // All enum we keep as a sting
            new IgnoreExtraElementsConvention(true) // Ignore fields that are not defined in C# classes
        };

        ConventionRegistry.Register(
            "RealEstateConventions",
            pack,
            _ => true // Apply to all classes
        );
    }
}