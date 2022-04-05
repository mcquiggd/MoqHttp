using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace moqhttp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private const string _url = "https://jsonplaceholder.typicode.com/posts";

    public PostsController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<JsonElement>> GetPosts()
    {
        var response = await _httpClient.GetAsync(_url);
        var body = await response.Content.ReadAsStringAsync();
        var posts = JsonSerializer.Deserialize<IEnumerable<JsonElement>>(body);

        return posts;
    }

    public async Task<JsonElement> CreatePost(string title)
    {
        var payload = new
        {
            title
        };

        var httpContent = new StringContent(JsonSerializer.Serialize(payload));
        var response = await _httpClient.PostAsync(_url, httpContent);
        var body = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<JsonElement>(body);

        return created;
    }
}