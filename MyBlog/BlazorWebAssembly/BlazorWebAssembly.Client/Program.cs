using BlazorWebAssembly.Client;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<IBlogApi, BlogApiWebClient>();
builder.Services.AddHttpClient();

await builder.Build().RunAsync();