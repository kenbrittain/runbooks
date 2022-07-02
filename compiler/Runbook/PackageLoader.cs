using System.IO;
using Runbook.Models;

namespace Runbook;

/// <summary>
/// Responsible for loading package data for a specified location.
/// </summary>
public interface IPackageLoader
{
    /// <summary>
    /// Loads the package data from the specified location.
    /// </summary>
    /// <param name="name">Name of the package to load.</param>
    /// <param name="location">Custom string used for location.</param>
    Stream LoadPackage(string name, string location);
}

/// <summary>
/// Defines the methods used to parse package definition into the data
/// model.
/// </summary>
public interface IPackageParser
{
    /// <summary>
    /// Parse the package stream in the data model.
    /// </summary>
    Package ParsePackage(string name, Stream data);
}

// =============================================================================
// Default implementation classes.
// =============================================================================

/// <summary>
/// YAML parser for package files.
/// </summary>
public class YamlPackageParser : IPackageParser
{
    public YamlPackageParser()
    {
    }

    public Package ParsePackage(string name, Stream data)
    {
	throw new NotSupportedException();
    }
}

/// <summary>
/// Load pacakge file from a directory.
/// </summary>
public class FilePackageLoader : IPackageLoader
{
    /// <summary>
    /// Gets the package data as a stream from the specified file.
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="packageLocation"></param>
    public Stream LoadPackage(string packageName, string packageLocation)
    {
	throw new NotSupportedException();
    }
}
