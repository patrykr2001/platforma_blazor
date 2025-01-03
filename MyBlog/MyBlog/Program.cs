using Auth0.AspNetCore.Authentication;
using MyBlog.Components;
using Data;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharedComponents;
using SharedComponents.RazorComponents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOptions<BlogApiJsonDirectAccessSettings>().Configure(options =>
{
    options.DataPath = Path.Combine("..", "..", "..", "Data");
    options.BlogPostsFolder = "BlogPosts";
    options.CategoriesFolder = "Categories";
    options.TagsFolder = "Tags";
});
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Authority"] ?? "";
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "";
});
builder.Services.AddTransient<ILoginStatus, LoginStatus>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(SharedComponents._Imports).Assembly);

app.MapGet("/authentication/login",
    async (string redirectUri, HttpContext context) =>
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(redirectUri)
            .Build();
        await context.ChallengeAsync(
            Auth0Constants.AuthenticationScheme, authenticationProperties);
    });

app.MapGet("/authentication/logout",
    async (HttpContext context) =>
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri("/")
            .Build();
        
        await context.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    });

app.Run();