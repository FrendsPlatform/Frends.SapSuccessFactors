namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// HTTP Header
/// </summary>
public class Header
{
    /// <summary>
    /// Header name
    /// </summary>
    /// <example>X-Custom-Header</example>
    public string Name { get; set; }

    /// <summary>
    /// Header value
    /// </summary>
    /// <example>CustomValue123</example>
    public string Value { get; set; }
}
