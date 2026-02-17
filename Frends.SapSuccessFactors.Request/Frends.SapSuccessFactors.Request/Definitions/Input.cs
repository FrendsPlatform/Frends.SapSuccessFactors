using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Choose how to construct the URL.
    /// </summary>
    /// <example>Builder</example>
    [DefaultValue(UrlType.Builder)]
    public UrlType UrlType { get; set; }

    /// <summary>
    /// HTTP method for the request.
    /// </summary>
    /// <example>GET</example>
    [DefaultValue(RequestMethod.GET)]
    public RequestMethod RequestMethod { get; set; }

    /// <summary>
    /// Entity name or endpoint path (Builder mode only).
    /// </summary>
    /// <example>User</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(UrlType), "", UrlType.Builder)]
    public string Endpoint { get; set; }

    /// <summary>
    /// OData query parameters (Builder mode only)
    /// </summary>
    /// <example>$filter=userId eq 'user123'&amp;$select=userId,username</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(UrlType), "", UrlType.Builder)]
    public string QueryParameters { get; set; }

    /// <summary>
    /// Full custom URL (Custom mode only).
    /// Allows you to call ANY URL, including non-OData endpoints.
    /// </summary>
    /// <example>https://api.successfactors.com/odata/v2/User?$filter=status eq 'active'</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(UrlType), "", UrlType.Custom)]
    public string CustomUrl { get; set; }

    /// <summary>
    /// Request body as JSON.
    /// </summary>
    /// <example>{"userId": "user123", "username": "john.doe"}</example>
    [DisplayFormat(DataFormatString = "Json")]
    [UIHint(nameof(RequestMethod), "", RequestMethod.POST, RequestMethod.PUT, RequestMethod.PATCH)]
    public string Body { get; set; }

    /// <summary>
    /// Additional HTTP headers (optional)
    /// </summary>
    /// <example>[{ "Name": "X-Custom-Header", "Value": "CustomValue" }]</example>
    [DisplayFormat(DataFormatString = "Expression")]
    public Header[] Headers { get; set; }

    /// <summary>
    /// Format of the result to return
    /// </summary>
    /// <example>JToken</example>
    [DefaultValue(ReturnFormat.JToken)]
    public ReturnFormat ResultFormat { get; set; }
}