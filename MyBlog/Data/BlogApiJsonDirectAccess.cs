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

    private async Task SaveAsync<T>(List<T>? list, string folder, string filename, T item)
    {
        var filePath = Path.Combine(_settings.DataPath, folder, filename);
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item));
        list ??= new();

        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }
    
    private void DeleteAsync<T>(List<T>? list, string folder, string id)
    {
        var filePath = Path.Combine(_settings.DataPath, folder, id + ".json");
        if(File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // ignored
            }
        }
    }
    
    public async Task<int> GetBlogPostCountAsync()
    {
        await LoadBlogPostsAsync();
        return _blogPosts?.Count ?? 0;
    }

    public async Task<List<BlogPost>?> GetBlogPostsAsync(int take, int skip)
    {
        await LoadBlogPostsAsync();
        return _blogPosts?.Skip(skip).Take(take).ToList() ?? [];
    }

    public async Task<List<Category>?> GetCategoriesAsync()
    {
        await LoadCategoriesAsync();
        return _categories ?? [];
    }

    public async Task<List<Tag>?> GetTagsAsync()
    {
        await LoadTagsAsync();
        return _tags ?? [];
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        await LoadBlogPostsAsync();
        if(_blogPosts == null)
        {
            throw new Exception("Brak wpisów bloga");
        }
        return _blogPosts.FirstOrDefault(b => b.Id == id);
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        await LoadCategoriesAsync();
        if(_categories == null)
        {
            throw new Exception("Brak kategorii");
        }
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        await LoadTagsAsync();
        if(_tags == null)
        {
            throw new Exception("Brak tagów");
        }
        return _tags.FirstOrDefault(t => t.Id == id);
    }

    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        if (item.Id == null)
        {
            item.Id = Guid.NewGuid().ToString();
        }
        await SaveAsync(_blogPosts, _settings.BlogPostsFolder, item.Id + ".json", item);
        return item;
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        if (item.Id == null)
        {
            item.Id = Guid.NewGuid().ToString();
        }
        await SaveAsync(_categories, _settings.CategoriesFolder, item.Id + ".json", item);
        return item;
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        if (item.Id == null)
        {
            item.Id = Guid.NewGuid().ToString();
        }
        await SaveAsync(_tags, _settings.TagsFolder, item.Id + ".json", item);
        return item;
    }

    public Task DeleteBlogPostAsync(string id)
    {
        DeleteAsync(_blogPosts, _settings.BlogPostsFolder, id);
        if(_blogPosts != null)
        {
            var item = _blogPosts.FirstOrDefault(b => b.Id == id);
            if(item != null)
            {
                _blogPosts.Remove(item);
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteCategoryAsync(string id)
    {
        DeleteAsync(_categories, _settings.CategoriesFolder, id);
        if(_categories != null)
        {
            var item = _categories.FirstOrDefault(c => c.Id == id);
            if(item != null)
            {
                _categories.Remove(item);
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteTagAsync(string id)
    {
        DeleteAsync(_tags, _settings.TagsFolder, id);
        if(_tags != null)
        {
            var item = _tags.FirstOrDefault(t => t.Id == id);
            if(item != null)
            {
                _tags.Remove(item);
            }
        }
        return Task.CompletedTask;
    }

    public Task InvalidateCacheAsync()
    {
        _blogPosts = null;
        _categories = null;
        _tags = null;
        return Task.CompletedTask;
    }
}