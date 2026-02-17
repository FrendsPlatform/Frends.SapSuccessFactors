using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.SapSuccessFactors.Request.Definitions;
using Frends.SapSuccessFactors.Request.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Frends.SapSuccessFactors.Request;

/// <summary>
/// Task Class for SapSuccessFactors operations.
/// </summary>
public static class SapSuccessFactors
{
    /// <summary>
    /// Factory for creating REST clients. Can be overridden in tests to provide a mock client.
    /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable SA1401 // Fields should be private
    public static Func<RestClientOptions, IRestClient> RestClientConstructor = options => new RestClient(options);
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA2211 // Non-constant fields should not be visible

    /// <summary>
    /// Frends Task for executing SAP SuccessFactors requests.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.SAPSuccessFactors.Request)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Options.</param>
    /// <param name="cancellationToken">Frends cancellation token.</param>
    /// <returns>Object { bool Success, dynamic Data, string ErrorMessage, int StatusCode, Dictionary&lt;string, string&gt; Headers }</returns>
    public static async Task<Result> Request(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            return await CustomRequest(input, connection, options, cancellationToken);
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }

    private static async Task<Result> CustomRequest(
        Input input,
        Connection connection,
        Options options,
        CancellationToken cancellationToken)
    {
        ValidateInput(input, connection);

        var fullUrl = GetRequestUrl(input, connection);

        var baseUrl = input.UrlType == UrlType.Custom
            ? new Uri(fullUrl).GetLeftPart(UriPartial.Authority)
            : connection.ApiServer;

        var clientOptions = new RestClientOptions(baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(options.ConnectionTimeoutSeconds),
            FollowRedirects = options.FollowRedirects,
        };

        if (options.AllowInvalidCertificate)
        {
            clientOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        var client = RestClientConstructor(clientOptions);

        var resource = GetResourcePath(fullUrl, connection.ApiServer);

        var request = new RestRequest(resource, GetRestSharpMethod(input.RequestMethod));

        AddAuthentication(request, connection);

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");

        if (input.Headers != null && input.Headers.Length > 0)
        {
            foreach (var header in input.Headers)
            {
                if (!string.IsNullOrWhiteSpace(header.Name))
                {
                    request.AddHeader(header.Name, header.Value ?? string.Empty);
                }
            }
        }

        if ((input.RequestMethod == RequestMethod.POST || input.RequestMethod == RequestMethod.PUT || input.RequestMethod == RequestMethod.PATCH)
            && !string.IsNullOrWhiteSpace(input.Body))
        {
            request.AddStringBody(input.Body, ContentType.Json);
        }

        var response = await client.ExecuteAsync(request, cancellationToken);

        dynamic responseData = null;
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            responseData = input.ResultFormat == ReturnFormat.JToken
                ? JToken.Parse(response.Content)
                : response.Content;
        }

        var headers = response.Headers?
            .GroupBy(h => h.Name ?? string.Empty)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(h => h.Value?.ToString() ?? string.Empty)))
            ?? new Dictionary<string, string>();

        if (!response.IsSuccessful)
        {
            var errorMessage = ExtractErrorFromResponse(responseData, response);
            throw new HttpRequestException($"SAP SuccessFactors API error: {(int)response.StatusCode} - {errorMessage}", null, response.StatusCode);
        }

        return new Result
        {
            Success = true,
            Data = responseData,
            StatusCode = (int)response.StatusCode,
            Headers = headers,
            Error = null,
        };
    }

    private static void ValidateInput(Input input, Connection connection)
    {
        if (input.UrlType == UrlType.Builder)
        {
            if (string.IsNullOrWhiteSpace(connection.ApiServer))
                throw new ArgumentNullException(nameof(connection), "API Server is required when using URL Builder mode");

            if (string.IsNullOrWhiteSpace(input.Endpoint))
                throw new ArgumentNullException(nameof(input), "Endpoint is required when using URL Builder mode");
        }
        else if (input.UrlType == UrlType.Custom)
        {
            if (string.IsNullOrWhiteSpace(input.CustomUrl))
                throw new ArgumentNullException(nameof(input), "Custom URL is required when using Custom URL mode");

            if (!Uri.TryCreate(input.CustomUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Custom URL is not a valid absolute URL", nameof(input));
        }

        if (connection.Authentication == Authentication.Basic)
        {
            if (string.IsNullOrWhiteSpace(connection.Username))
                throw new ArgumentNullException(nameof(connection), "Username is required for Basic authentication");
            if (string.IsNullOrWhiteSpace(connection.Password))
                throw new ArgumentNullException(nameof(connection), "Password is required for Basic authentication");
        }

        if (connection.Authentication == Authentication.OAuth)
        {
            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new ArgumentNullException(nameof(connection), "Access Token is required for OAuth authentication");
        }
    }

    private static string GetRequestUrl(Input input, Connection connection)
    {
        if (input.UrlType == UrlType.Custom)
        {
            return input.CustomUrl;
        }
        else
        {
            return BuildODataUrl(connection, input);
        }
    }

    private static string BuildODataUrl(Connection connection, Input input)
    {
        var apiServer = connection.ApiServer.TrimEnd('/');
        var endpoint = input.Endpoint.TrimStart('/');

        string baseUrl;
        if (connection.ODataVersion == ODataVersion.V4)
        {
            baseUrl = $"{apiServer}/odata/v4/{endpoint}";
        }
        else
        {
            baseUrl = $"{apiServer}/odata/v2/{endpoint}";
        }

        if (!string.IsNullOrWhiteSpace(input.QueryParameters))
        {
            var cleanParams = input.QueryParameters.TrimStart('?', '&');
            baseUrl += $"?{cleanParams}";
        }

        return baseUrl;
    }

    private static string GetResourcePath(string fullUrl)
    {
        if (fullUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(fullUrl);
            return uri.PathAndQuery;
        }

        return fullUrl;
    }

    private static Method GetRestSharpMethod(RequestMethod method)
    {
        return method switch
        {
            RequestMethod.GET => Method.Get,
            RequestMethod.POST => Method.Post,
            RequestMethod.PUT => Method.Put,
            RequestMethod.PATCH => Method.Patch,
            RequestMethod.DELETE => Method.Delete,
            _ => Method.Get,
        };
    }

    private static void AddAuthentication(RestRequest request, Connection connection)
    {
        switch (connection.Authentication)
        {
            case Authentication.Basic:
                var basicAuth = Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes($"{connection.Username}:{connection.Password}"));

                request.AddHeader("Authorization", $"Basic {basicAuth}");
                break;

            case Authentication.OAuth:
                request.AddHeader("Authorization", $"Bearer {connection.AccessToken}");
                break;
        }
    }

    private static string ExtractErrorFromResponse(dynamic responseData, RestResponse response)
    {
        try
        {
            if (responseData is JToken jToken)
            {
                var errorNode = jToken["error"];
                if (errorNode != null)
                {
                    // Try V2 format
                    var messageValue = errorNode["message"]?["value"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(messageValue))
                        return messageValue;

                    // Try V2 format
                    var message = errorNode["message"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
            }
        }
        catch
        {
            // If extraction fails, use response error
        }

        return response.ErrorMessage ?? $"HTTP {(int)response.StatusCode}: {response.Content}";
    }
}
