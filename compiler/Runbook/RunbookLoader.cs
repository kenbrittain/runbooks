using System.IO;

namespace Runbook;

/// <summary>
/// Responsible for loading runbook data from a user specified location.
/// </summary>
public interface IRunbookLoader
{
    /// <summary>
    /// Locate the runbook data from the specified location.
    /// </summary>
    /// <param name="name">Name of the runbook to load.</param>
    /// <param name="location">Custom string used for location.</param>
    Stream LoadRunbook(string name, string location);
}

public interface IRunbookParser
{
    /// <summary>
    /// Parse the data into a runbook.
    /// </summary>
    /// <param name="name">Name of the runbook.</param>
    /// <param name="data">Stream for runbook data.</param>
    /// <exception cref="RunbookException">Thrown if the data cannot be parsed into a runbook.</exception>
    Runbook ParseRunbook(string name, Stream data);
}

// =============================================================================
// Default implementation classes.
// =============================================================================

public class DirectoryRunbookLoader : IRunbookLoader
{
    public Stream LoadRunbook(string name, string location)
    {
	throw new NotSupportedException();
    }
}

public class YamlRunbookParser : IRunbookParser
{
    public Runbook ParseRunbook(string name, Stream data)
    {
	throw new NotSupportedException();
    }
}
