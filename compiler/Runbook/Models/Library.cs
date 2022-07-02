namespace Runbook.Models;

/// <summary>
/// Data model for parsing the <c>library.yaml</c> files that define
/// </summary>
public class Library : StandardModel
{
    private List<string> _packages;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryModel"/> class that
    /// is empty and ready for accepted newly parsed elements.
    /// </summary>
    public Library()
    {
        _packages = new List<string>();
    }
    
    /// <summary>
    /// Names of the packages contained in this library.
    /// </summary>
    public IEnumerable<string> Packages => _packages;

    /// <summary>
    /// Append the <see cref="Packages"/> in this library with the package name.
    /// </summary>
    /// <param name="packageName">Name of the package.</param>
    public void AddPackage(string packageName)
    {
	if (packageName == null || packageName == string.Empty)
	{
 	    throw new ArgumentException($"The package name must contain a value!", nameof(packageName));
	    
	}

	_packages.Add(packageName);
    }   
}
