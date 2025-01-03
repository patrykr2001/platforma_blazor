using BlazorWebAssembly.Components;
using BlazorWebAssembly.Endpoints;
using Data;
using Data.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddOptions<BlogApiJsonDirectAccessSettings>().Configure(options =>
{
    options.DataPath = Path.Combine("..", "..", "..", "..", "Data");
    options.BlogPostsFolder = "BlogPosts";
    options.CategoriesFolder = "Categories";
    options.TagsFolder = "Tags";
});
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapBlogPostApi();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebAssembly.Client._Imports).Assembly)
    .AddAdditionalAssemblies(typeof(SharedComponents._Imports).Assembly);

app.Run();