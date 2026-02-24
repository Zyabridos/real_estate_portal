using RealEstate.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(o =>
{
    o.ValidateScopes = true;
    o.ValidateOnBuild = true;
});

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApi();

app.Run();