using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Runbook;

/// <summary>
/// This class handles finding files. It wraps up the file globbing from
/// Microsoft because there were issues with class name clashes between this
/// package and the <c>System.IO.Abstractions</c> package.
/// </summary>
public class FileFinder
{
    private readonly Matcher _fileGlobbing;
    private readonly DirectoryInfoBase _rootDir;

    /// <summary>
    /// Create a new file finder starting at the specified directory. All
    /// file found will be relative to this directory.
    /// </summary>
    /// <param name="dir">The directory to start in.</param>
    public FileFinder(string dir)
        : this(new DirectoryInfoWrapper(new DirectoryInfo(dir)))
    {
    }

    /// <summary>
    /// Create a new file finder at the specified directory. 
    /// </summary>
    /// <param name="dir">The directory to start in.</param>
    /// <exception cref="ArgumentNullException">Thrown if the specified directory is null.</exception>
    public FileFinder(DirectoryInfoBase dir)
    {
        _rootDir = dir ?? throw new ArgumentNullException(nameof(dir));
        _fileGlobbing = new Matcher();
    }

    /// <summary>
    /// Add the include pattern to the file finder.
    /// </summary>
    /// <param name="pattern">The file pattern.</param>
    /// <exception cref="ArgumentException">Thrown if the pattern is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the pattern is null.</exception>
    public void AddInclude(string pattern)
    {
        if (pattern == null) throw new ArgumentNullException(nameof(pattern));
        if (pattern == string.Empty) throw new ArgumentException("", nameof(pattern));

        _fileGlobbing.AddInclude(pattern);
    }

    /// <summary>
    /// Include multiple patterns.
    /// </summary>
    /// <param name="patterns">The file patterns.</param>
    public void AddInclude(IEnumerable<string> patterns)
    {
        foreach (var p in patterns)
        {
            AddInclude(p);
        }
    }
    
    /// <summary>
    /// Add the exclude pattern to the file finder.
    /// </summary>
    /// <param name="pattern">The file pattern.</param>
    /// <exception cref="ArgumentException">Thrown if the pattern is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the pattern is null.</exception>
    public void AddExclude(string pattern)
    {
        if (pattern == null) throw new ArgumentNullException(nameof(pattern));
        if (pattern == string.Empty) throw new ArgumentException("", nameof(pattern));
        
        _fileGlobbing.AddExclude(pattern);
    }

    /// <summary>
    /// Exclude multiple patterns.
    /// </summary>
    /// <param name="patterns">The patterns to exclude.</param>
    public void AddExclude(IEnumerable<string> patterns)
    {
        foreach (var p in patterns)
        {
            AddExclude(p);
        }
    }
    /// <summary>
    /// Iterate over the files pattern locating the files.
    /// </summary>
    /// <returns>The files found using the defined patterns.</returns>
    public IEnumerable<ManifestFile> Find()
    {
        var results = _fileGlobbing.Execute(_rootDir);
        foreach (var file in results.Files)
        {
            var model = new ManifestFile
            {
                Name = Path.GetFileNameWithoutExtension(file.Path),
                Path = file.Path,
                FullPath = Path.GetFullPath(file.Path)
            };
            yield return model;
        }
    }
}