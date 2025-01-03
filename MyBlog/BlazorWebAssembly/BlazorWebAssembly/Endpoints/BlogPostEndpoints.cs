using Data.Models.Interfaces;
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
    }
}