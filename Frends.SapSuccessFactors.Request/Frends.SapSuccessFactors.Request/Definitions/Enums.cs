namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// URL construction method
/// </summary>
public enum UrlType
{
    /// <summary>
    /// Build URL automatically from endpoint and parameters
    /// </summary>
    Builder,

    /// <summary>
    /// Provide complete custom URL
    /// </summary>
    Custom,
}

/// <summary>
/// OData API version
/// </summary>
public enum ODataVersion
{
    /// <summary>
    /// OData V2
    /// </summary>
    V2,

    /// <summary>
    /// OData V4
    /// </summary>
    V4,
}

/// <summary>
/// Authentication method for SAP SuccessFactors
/// </summary>
public enum Authentication
{
    /// <summary>
    /// Basic authentication
    /// </summary>
    Basic,

    /// <summary>
    /// OAuth 2.0 Bearer token
    /// </summary>
    OAuth,
}

/// <summary>
/// HTTP method
/// </summary>
public enum RequestMethod
{
    /// <summary>
    /// GET method - retrieve data
    /// </summary>
    GET,

    /// <summary>
    /// POST method - create new resource
    /// </summary>
    POST,

    /// <summary>
    /// PUT method - replace existing resource
    /// </summary>
    PUT,

    /// <summary>
    /// PATCH method - update existing resource
    /// </summary>
    PATCH,

    /// <summary>
    /// DELETE method - remove resource
    /// </summary>
    DELETE,
}

/// <summary>
/// Return format for response
/// </summary>
public enum ReturnFormat
{
    /// <summary>
    /// Return as string
    /// </summary>
    String,

    /// <summary>
    /// Return as JSON token (JToken)
    /// </summary>
    JToken,
}