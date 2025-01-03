using Data.Models.Interfaces;
using Data.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebAssembly.Endpoints;

public static class BlogPostEndpoints
{
    public static void MapBlogPostApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/BlogPosts",
            async (IBlogApi api, [FromQuery] int numberOfPosts, [FromQuery] int startIndex) =>
            {
                return Results.Ok(await api.GetBlogPostsAsync(numberOfPosts, startIndex));
            });

        app.MapGet("/api/BlogPostCount",
            async (IBlogApi api) =>
            {
                return Results.Ok(await api.GetBlogPostCountAsync());
            });

        app.MapGet("/api/BlogPosts/{*id}",
            async (IBlogApi api, string id) =>
            {
                return Results.Ok(await api.GetBlogPostAsync(id));
            });

        app.MapPut("/api/BlogPosts",
            async (IBlogApi api, [FromBody] BlogPost item) =>
            {
                return Results.Ok(await api.SaveBlogPostAsync(item));
            }).RequireAuthorization();

        app.MapDelete("/api/BlogPosts/{*id}",
            async (IBlogApi api, string id) =>
            {
                await api.DeleteBlogPostAsync(id);
                return Results.Ok();
            }
            ).RequireAuthorization();
    }
}