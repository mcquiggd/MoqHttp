
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using MoqHttp.API.Controllers;
using Xunit;

namespace MoqHttp.Tests;

public class PostsTest
{
    [Fact]
    public async Task ShouldReturnPosts()
    {
        // Arrange        
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"[{ ""id"": 1, ""title"": ""Cool post!""}, { ""id"": 100, ""title"": ""Some title""}]"),
        };

        var handlerMock = MockHttpMessageHandler.SetupMock(response);

        var httpClient = new HttpClient(handlerMock.Object);
        var posts = new PostsController(httpClient);

        // Act
        var retrievedPosts = await posts.GetPosts();

        // Assert
        Assert.NotNull(retrievedPosts);

        handlerMock.VerifySendAsync(HttpMethod.Get, 1);
    }

    [Fact]
    public async Task ShouldCallCreatePostApi()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"{ ""id"": 101 }"),
        };

        var handlerMock = MockHttpMessageHandler.SetupMock(response);
        var httpClient = new HttpClient(handlerMock.Object);
        var posts = new PostsController(httpClient);

        // Act
        var retrievedPosts = await posts.CreatePost("Best post");

        // Assert
        handlerMock.VerifySendAsync(HttpMethod.Post, 1);
    }
}

public static class MockHttpMessageHandler
{
    public static Mock<HttpMessageHandler> SetupMock(HttpResponseMessage httpResponseMessage)
    {
        var mock = new Mock<HttpMessageHandler>();

        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        return mock;
    }

    public static void VerifySendAsync(this Mock<HttpMessageHandler> mock, HttpMethod httpMethod, int times)
    {
        mock.Protected().Verify(
            "SendAsync",
            Times.Exactly(times),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == httpMethod),
            ItExpr.IsAny<CancellationToken>());
    }
}
