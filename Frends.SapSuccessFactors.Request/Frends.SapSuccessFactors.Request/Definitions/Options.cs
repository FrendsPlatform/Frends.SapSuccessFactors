using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    /// <example>30</example>
    [DefaultValue(30)]
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Allow invalid SSL certificates
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool AllowInvalidCertificate { get; set; } = false;

    /// <summary>
    /// Follow HTTP redirects
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool FollowRedirects { get; set; } = true;

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}