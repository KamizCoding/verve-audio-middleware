using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.UseWebRoot("wwwroot");

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", () => "🎧 Verve Middleware is running!");

await app.RunAsync();
