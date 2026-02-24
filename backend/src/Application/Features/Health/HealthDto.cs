namespace RealEstate.Application.Features.Health;

public sealed record HealthDto(string Status, string Service, string Environment, string Mongo);