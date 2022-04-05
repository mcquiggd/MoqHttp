using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace MoqHttp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private const string URL = "https://jsonplaceholder.typicode.com";

    public PostsController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(URL);

    }

    [HttpGet]
    public async Task<IEnumerable<JsonElement>> GetPosts()
    {
        var response = await _httpClient.GetAsync("/posts");
        var body = await response.Content.ReadAsStringAsync();
        var posts = JsonSerializer.Deserialize<IEnumerable<JsonElement>>(body);

        return posts;
    }

    [HttpPost]
    public async Task<JsonElement> CreatePost(string title)
    {
        var payload = new
        {
            title
        };

        var httpContent = new StringContent(JsonSerializer.Serialize(payload));
        var response = await _httpClient.PostAsync("/posts", httpContent);
        var body = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<JsonElement>(body);

        return created;
    }
}