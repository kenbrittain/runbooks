namespace Runbook.Models;

/// <summary>
/// Data model for loading package definitions.
/// </summary>
public class PackageModel : StandardModel
{
    private readonly List<string> _runbooks;

    public PackageModel()
    {
	_runbooks = new List<string>();
    }

    /// <summary>
    /// Append the <c>runbooks</c> section with the specified name.
    /// </summary>
    /// <param name="runbookName">Root name of the runbook.</param>
    /// <exception cref="Argumentexception">Thrown when the name if <c>null</c> or empty.</exception>
    public void AddRunbook(string  runbookName)
    {
	if (runbookName == null || runbookName == string.Empty)
	{
	    throw new ArgumentException($"The runbook name must contain a value!", nameof(runbookName));
	}
	
	_runbooks.Add(runbookName);
    }
}

