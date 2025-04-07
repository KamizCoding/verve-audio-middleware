using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseStaticFiles(); // ✅ Serve wwwroot/results/latest.json to frontend

app.MapGet("/", () => "🎧 Verve Middleware is running!");

await app.RunAsync();
