namespace Runbook.Models;

/// <summary>
/// Data model for parsing the <c>library.yaml</c> files that define
/// </summary>
public class LibraryModel
{
    private List<string> _packages;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryModel"/> class that
    /// is empty and ready for accepted newly parsed elements.
    /// </summary>
    public LibraryModel()
    {
        _packages = new List<string>();
        Version = 0;
        Title = "Untitled";
        Summary = string.Empty;
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

    /// <summary>
    /// Names of the packages contained in this library.
    /// </summary>
    public IEnumerable<string> Packages => _packages;
}
