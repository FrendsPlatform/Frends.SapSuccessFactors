using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.SapSuccessFactors.Request.Definitions;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace Frends.SapSuccessFactors.Request.Tests;

[TestFixture]
public class FunctionalTests
{
    private Mock<IRestClient> _restClientMock;

    [SetUp]
    public void Setup()
    {
        _restClientMock = new Mock<IRestClient>();
        SapSuccessFactors.RestClientConstructor = _ => _restClientMock.Object;
    }

    [TearDown]
    public void TearDown()
    {
        SapSuccessFactors.RestClientConstructor = options => new RestClient(options);
    }

    [Test]
    public async Task Request_BuilderMode_GET_ReturnsSuccess()
    {
        SetupHttpRequest(content: "{ \"d\": { \"results\": [] } }");

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.GET,
                Endpoint = "User",
                QueryParameters = "$top=5",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V2,
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false, ConnectionTimeoutSeconds = 30 },
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task Request_BuilderMode_POST_WithBody_ReturnsSuccess()
    {
        SetupHttpRequest(content: "{ \"userId\": \"user123\" }");

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.POST,
                Endpoint = "User",
                Body = "{ \"username\": \"john.doe\" }",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V2,
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false, ConnectionTimeoutSeconds = 30 },
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task Request_CustomUrl_ReturnsSuccess()
    {
        SetupHttpRequest(content: "{ \"status\": \"ok\" }");

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Custom,
                RequestMethod = RequestMethod.GET,
                CustomUrl = "https://api.successfactors.com/odata/v2/User?$top=10",
                ResultFormat = ReturnFormat.String,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task Request_BasicAuthentication_ReturnsSuccess()
    {
        SetupHttpRequest(content: "{ \"status\": \"ok\" }");

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.GET,
                Endpoint = "User",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V2,
                Authentication = Authentication.Basic,
                Username = "testuser",
                Password = "testpass",
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task Request_ODataV4_ReturnsSuccess()
    {
        SetupHttpRequest(content: "{ \"status\": \"ok\" }");

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.GET,
                Endpoint = "User",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V4,
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task Request_ApiReturnsError_ThrowErrorOnFailureFalse_ReturnsErrorInResult()
    {
        SetupHttpRequest(
            success: false,
            content: "{ \"error\": { \"message\": \"User not found\" } }",
            statusCode: System.Net.HttpStatusCode.NotFound);

        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.GET,
                Endpoint = "User",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V2,
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("User not found"));
        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task Request_MissingCustomUrl_ReturnsError()
    {
        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Custom,
                RequestMethod = RequestMethod.GET,
                CustomUrl = null,
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                Authentication = Authentication.OAuth,
                AccessToken = "test-token",
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom URL is required"));
    }

    [Test]
    public async Task Request_MissingAccessToken_ReturnsError()
    {
        var result = await SapSuccessFactors.Request(
            new Input
            {
                UrlType = UrlType.Builder,
                RequestMethod = RequestMethod.GET,
                Endpoint = "User",
                ResultFormat = ReturnFormat.JToken,
            },
            new Connection
            {
                ApiServer = "https://api.successfactors.com",
                ODataVersion = ODataVersion.V2,
                Authentication = Authentication.OAuth,
                AccessToken = null,
            },
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Access Token is required"));
    }

    [Test]
    public void Request_MissingUsername_ThrowsArgumentNullException()
    {
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await SapSuccessFactors.Request(
                new Input
                {
                    UrlType = UrlType.Builder,
                    RequestMethod = RequestMethod.GET,
                    Endpoint = "User",
                    ResultFormat = ReturnFormat.JToken,
                },
                new Connection
                {
                    ApiServer = "https://api.successfactors.com",
                    ODataVersion = ODataVersion.V2,
                    Authentication = Authentication.Basic,
                    Username = null,
                    Password = "password",
                },
                new Options { ThrowErrorOnFailure = true },
                CancellationToken.None));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Does.Contain("Username"));
    }

    private void SetupHttpRequest(
        bool success = true,
        string content = "{ \"status\": \"ok\" }",
        System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK)
    {
        _restClientMock.Setup(o => o
            .ExecuteAsync(
                It.IsAny<RestRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RestResponse
            {
                IsSuccessStatusCode = success,
                Content = content,
                StatusCode = statusCode,
                ResponseStatus = ResponseStatus.Completed,
            });
    }
}