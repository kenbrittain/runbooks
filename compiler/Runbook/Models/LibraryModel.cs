namespace Runbook.Models;

/// <summary>
/// Data model for parsing the <c>library.yaml</c> files that define
/// </summary>
public class LibraryModel : StandardModel
{
    private List<string> _packages;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryModel"/> class that
    /// is empty and ready for accepted newly parsed elements.
    /// </summary>
    public LibraryModel()
    {
        _packages = new List<string>();
    }
    
    /// <summary>
    /// Names of the packages contained in this library.
    /// </summary>
    public IEnumerable<string> Packages => _packages;
}
