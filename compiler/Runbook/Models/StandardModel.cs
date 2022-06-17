namespace Runbook.Models;

/// <summary>
/// Base class for common runbook model properties.
/// </summary>
public class StandardModel
{
    /// <summary>
    /// Default contructors for derived class initialization.
    /// </summary>
    public StandardModel()
    {
        this.Version = 0;
        this.Title = "Untitled";
        this.Summary = string.Empty;
    }

    /// <summary>
    /// Default contructors for derived class initialization.
    /// </summary>
    public StandardModel(int version, string title, string? summary)
    {
        this.Version = version;
        this.Title = title;
        this.Summary = summary ?? string.Empty;
    }

    /// <summary>
    /// The version number for this file format. The default version number
    /// is 1.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The display text for this library.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The summary text to display for this library.
    /// </summary>
    public string Summary { get; set; }
}
