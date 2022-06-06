using System.Reflection;

namespace Runbook;

/// <summary>
/// Represents a resource that is embedded in an assembly.
/// </summary>
public class AssemblyResource
{
    /// <summary>
    /// The name of the resource identified as <c>Namespace.Directory.File.ext</c>.
    /// </summary>
    private readonly string _resourceName;
    
    /// <summary>   
    /// The assembly to load the resource from.
    /// </summary>
    private readonly Assembly _resourceAssembly;
    
    /// <summary>
    /// Initializes a new <see cref="AssemblyResource"/> with the specified
    /// name. If the assembly is not specified then the executing assembly
    /// is used by default.
    /// </summary>
    /// <param name="resourceName">Name of the resource to load.</param>
    /// <param name="resourceAssembly">Assembly to load resource from.</param>
    /// <exception cref="ArgumentNullException">The <c>resourceName</c> is <c>null</c>.</exception>
    public AssemblyResource(string resourceName, Assembly resourceAssembly)
    {
        _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        _resourceAssembly = resourceAssembly ?? throw new ArgumentNullException(nameof(resourceAssembly));
    }

    /// <summary>
    /// The assembly the resource is loaded from.
    /// </summary>
    public Assembly ResourceAssembly => _resourceAssembly;

    /// <summary>
    /// The name of the resource to load.
    /// </summary>
    public string ResourceName => _resourceName;

    /// <summary>
    /// Loads the resource from a stream. The caller must dispose of the
    /// stream. 
    /// </summary>
    /// <returns>The resource stream.</returns>
    public Stream? GetResourceStream()
    {
        var stream = _resourceAssembly.GetManifestResourceStream(_resourceName);
        return stream; 
    }

    /// <summary>
    /// Loads the resource as text. 
    /// </summary>
    /// <returns>The text of the resource; or <c>null</c> if the resource was not found.</returns>
    public string? GetResourceText()
    {
        using var stream = GetResourceStream();
        if (stream != null)
        {
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            return text;
        }

        return null;
    }

    /// <summary>
    /// Determine if the resource exists in the assembly.
    /// </summary>
    /// <returns><c>true</c> if the resource exists; otherwise <c>false</c>.</returns>
    public bool ResourceExists()
    {
        var allNames = _resourceAssembly.GetManifestResourceNames();
        var foundName = allNames.FirstOrDefault(n => n == _resourceName);
        return foundName != default(string);
    }
}