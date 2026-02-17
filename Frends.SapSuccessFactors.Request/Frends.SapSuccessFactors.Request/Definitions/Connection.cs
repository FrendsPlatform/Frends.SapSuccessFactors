using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// SAP SuccessFactors API Server URL (required for Builder mode)
    /// </summary>
    /// <example>https://api.successfactors.com</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ApiServer { get; set; }

    /// <summary>
    /// OData API version (required for Builder mode)
    /// </summary>
    /// <example>V2</example>
    [DefaultValue(ODataVersion.V2)]
    public ODataVersion ODataVersion { get; set; }

    /// <summary>
    /// Authentication method (OAuth recommended, Basic deprecated)
    /// </summary>
    /// <example>OAuth</example>
    [DefaultValue(Authentication.OAuth)]
    public Authentication Authentication { get; set; }

    /// <summary>
    /// Username for Basic authentication (deprecated - use OAuth instead)
    /// </summary>
    /// <example>admin@company</example>
    [UIHint(nameof(Authentication), "", Authentication.Basic)]
    [DisplayFormat(DataFormatString = "Text")]
    public string Username { get; set; }

    /// <summary>
    /// Password for Basic authentication (deprecated - use OAuth instead)
    /// </summary>
    /// <example>password123</example>
    [PasswordPropertyText]
    [UIHint(nameof(Authentication), "", Authentication.Basic)]
    public string Password { get; set; }

    /// <summary>
    /// OAuth 2.0 Bearer access token.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    [PasswordPropertyText]
    [UIHint(nameof(Authentication), "", Authentication.OAuth)]
    public string AccessToken { get; set; }
}
