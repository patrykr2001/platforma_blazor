using System.Text.Json;
using Data.Models.Interfaces;
using Data.Models.Models;
using Microsoft.Extensions.Options;

namespace Data;

public class BlogApiJsonDirectAccess : IBlogApi
{
    private readonly BlogApiJsonDirectAccessSettings _settings;

    public BlogApiJsonDirectAccess(IOptions<BlogApiJsonDirectAccessSettings> options)
    {
        _settings = options.Value;
        if(!Directory.Exists(_settings.DataPath))
        {
            Directory.CreateDirectory(_settings.DataPath);
        }
        if(!Directory.Exists(Path.Combine(_settings.DataPath, _settings.BlogPostsFolder)))
        {
            Directory.CreateDirectory(Path.Combine(_settings.DataPath, _settings.BlogPostsFolder));
        }
        if(!Directory.Exists(Path.Combine(_settings.DataPath, _settings.CategoriesFolder)))
        {
            Directory.CreateDirectory(Path.Combine(_settings.DataPath, _settings.CategoriesFolder));
        }
        if(!Directory.Exists(Path.Combine(_settings.DataPath, _settings.TagsFolder)))
        {
            Directory.CreateDirectory(Path.Combine(_settings.DataPath, _settings.TagsFolder));
        }
    }

    private List<BlogPost>? _blogPosts;
    private List<Category>? _categories;
    private List<Tag>? _tags;
    
    private void Load<T>(ref List<T>? list, string folder)
    {
        if(list == null)
        {
            list = new();
            var fullPath = Path.Combine(_settings.DataPath, folder);
            foreach(var file in Directory.GetFiles(fullPath))
            {
                var json = File.ReadAllText(file);
                var item = JsonSerializer.Deserialize<T>(json);
                if(item != null)
                {
                    list.Add(item);
                }
            }
        }
    }
    
    private Task LoadBlogPostsAsync()
    {
        Load(ref _blogPosts, _settings.BlogPostsFolder);
        return Task.CompletedTask;
    }
    
    private Task LoadCategoriesAsync()
    {
        Load(ref _categories, _settings.CategoriesFolder);
        return Task.CompletedTask;
    }
    
    private Task LoadTagsAsync()
    {
        Load(ref _tags, _settings.TagsFolder);
        return Task.CompletedTask;
    }
    
    public Task<int> GetBlogPostCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<BlogPost>?> GetBlogPostsAsync(int take, int skip)
    {
        throw new NotImplementedException();
    }

    public Task<List<Category>?> GetCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>?> GetTagsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BlogPost?> GetBlogPostAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategoryAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Tag?> GetTagAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> SaveCategoryAsync(Category item)
    {
        throw new NotImplementedException();
    }

    public Task<Tag?> SaveTagAsync(Tag item)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBlogPostAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategoryAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTagAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task InvalidateCacheAsync()
    {
        throw new NotImplementedException();
    }
}