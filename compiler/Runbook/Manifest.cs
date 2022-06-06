using System.IO.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Runbook;

/// <summary>
/// This model contains all of the properties common to file based artifacts.
/// </summary>
public class ManifestFile
{
    /// <summary>
    /// The name of a file is not necessarily the actual filename. It is
    /// usually derived from the file name but is more symbolic.
    /// </summary>
    [YamlMember(Alias = "id")]
    public string? Id { get; set; }

    /// <summary>
    /// The name of a file is not necessarily the actual filename. It is
    /// usually derived from the file name but is more symbolic.
    /// </summary>
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The relative path to this file.
    /// </summary>
    [YamlMember(Alias = "path")]
    public string? Path { get; set; }
    
    /// <summary>
    /// The absolute path to the file.
    /// </summary>
    [YamlIgnore]
    public string? FullPath { get; set; }
}

/// <summary>
/// Model describing the Yaml source files.
/// </summary>
public class ManifestSource : ManifestFile
{
}

/// <summary>
/// Model describing an generated view file.
/// </summary>
public class ManifestView : ManifestFile
{
    /// <summary>
    /// Name of the ?
    /// </summary>
    [YamlMember(Alias = "generator")]
    public string? Generator { get; set; }
}

/// <summary>
/// Model for a generated script file. 
/// </summary>
public class ManifestScript : ManifestFile
{
    /// <summary>
    /// The actual programming language of the script. 
    /// </summary>
    public string? Language { get; set; }
}

/// <summary>
/// Model describing the manifest used to process the files and store state
/// during the executing commands.
/// </summary>
public class Manifest
{
    private readonly List<ManifestSource> _sources;
    private readonly List<ManifestView> _views;
    private readonly List<ManifestScript> _scripts;
    private readonly IFileSystem _fs;
    
    /// <summary>
    /// Initializes a new <see cref="Manifest"/> that is empty.
    /// </summary>
    public Manifest()
        : this(new FileSystem())
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Manifest"/> that using the
    /// specified file system.
    /// </summary>
    public Manifest(IFileSystem fs)
    {
        _fs = fs;
        _sources = new List<ManifestSource>();
        _views = new List<ManifestView>();
        _scripts = new List<ManifestScript>();
    }

    
    /// <summary>
    /// Collection of Yaml source files read.
    /// </summary>
    public IEnumerable<ManifestSource> Sources => _sources;

    /// <summary>
    /// Collection of views generated.
    /// </summary>
    public IEnumerable<ManifestView> Views => _views;

    /// <summary>
    /// Collection of scripts generated.
    /// </summary>
    public IEnumerable<ManifestScript> Scripts => _scripts;

    /// <summary>
    /// Appends the model with a new script file.
    /// </summary>
    /// <param name="script">Script file.</param>
    /// <exception cref="ArgumentNullException">Thrown when the added file if <c>null</c>.</exception>
    public void AddScript(ManifestScript script)
    {
        if (script == null)
        {
            var message = string.Format(Messages.NullArgumentExceptionFormat, nameof(script));
            throw new ArgumentNullException(message);
        }
        _scripts.Add(script);
    }

    /// <summary>
    /// Appends the model with a new source file.
    /// </summary>
    /// <param name="source">The source file.</param>
    /// <exception cref="ArgumentNullException">Thrown when the added file if <c>null</c>.</exception>
    public void AddSource(ManifestSource source)
    {
        if (source == null)
        {
            var message = string.Format(Messages.NullArgumentExceptionFormat, nameof(source));
            throw new ArgumentNullException(message);
        }
        _sources.Add(source);
    }
    
    /// <summary>
    /// Appends the model with the generated view file.
    /// </summary>
    /// <param name="view">The generated view file.</param>
    /// <exception cref="ArgumentNullException">Thrown when the added file if <c>null</c>.</exception>
    public void AddView(ManifestView view)
    {
        if (view == null)
        {
            var message = string.Format(Messages.NullArgumentExceptionFormat, nameof(view));
            throw new ArgumentNullException(message);
        }

        var existingIndex = _views.FindIndex(v => v.Name == view.Name);
        if (existingIndex != -1)
        {
            _views[existingIndex] = view;
        }
        else
        {
            _views.Add(view);
        }
    }

    /// <summary>
    /// Finds all source files in the specified directory.
    /// </summary>
    /// <param name="dir">Root directory path to begin searching.</param>
    public void FindFiles(string dir)
    {
        Find(dir, Enumerable.Empty<string>(), Enumerable.Empty<string>());
    }
    
    /// <summary>
    /// Find the source files using the specified file patterns.
    /// </summary>
    /// <param name="dir">Root directory path to begin searching.</param>
    /// <param name="includes">File patterns to include in the search.</param>
    /// <param name="excludes">File patterns to exclude in the search.</param>
    public void Find(string dir, IEnumerable<string> includes, IEnumerable<string> excludes)
    {
        var finder = new FileFinder(dir);

        finder.AddExclude("manifest.yml");
        if (includes.Count() == 0)
        {
            var yamlPattern = $"**{_fs.Path.DirectorySeparatorChar}*.yml";
            finder.AddInclude(yamlPattern);
        }

        finder.AddInclude(includes);
        finder.AddExclude(excludes);

        foreach (var file in finder.Find())
        {
            var source = new ManifestSource
            {
                Name = Path.GetFileNameWithoutExtension(file.Path),
                Path = file.Path,
                FullPath = Path.GetFullPath(file.Path!)
            };
            AddSource(source);
        }
    }
    
    /// <summary>
    /// Reads the manifest Yaml from the specified file.
    /// </summary>
    /// <param name="fileName">File to read Yaml.</param>     
    /// <returns>The loaded manifest document.</returns>
    public static Manifest Load(string fileName)
    {
        using var yamlReader = new StreamReader(fileName);
        var newManifest = Load(yamlReader);
        return newManifest;
    }

    /// <summary>
    /// Reads the manifest Yaml from the specified <see cref="TextReader"/>.
    /// </summary>
    /// <param name="yamlReader">The <see cref="TextReader"/> used to get the Yaml data.</param>
    /// <returns>The loaded manifest document.</returns>
    public static Manifest Load(TextReader yamlReader)
    {
        var yamlText = yamlReader.ReadToEnd();
        var newManifest = new Manifest();
        newManifest.LoadYaml(yamlText);
        return newManifest;
    }

    /// <summary>
    /// Reads the manifest Yaml from the specified <see cref="TextReader"/>.
    /// </summary>
    /// <param name="yamlText"><see cref="String"/> containing he Yaml to load.</param>
    /// <returns>The loaded manifest document.</returns>
    public void LoadYaml(string yamlText)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var manifestBuilder = deserializer.Deserialize<ManifestBuilder>(yamlText);
        manifestBuilder.Build(this);
    }

    /// <summary>
    /// Write the Yaml manifest to the specified file.
    /// </summary>
    /// <param name="fileName">File to write.</param>
    public void Save(string fileName)
    {
        using var yamlWriter = new StreamWriter(fileName);
        Save(yamlWriter);
    }

    /// <summary>
    /// Write the Yaml manifest to the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="yamlWriter"></param>
    public void Save(TextWriter yamlWriter)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithIndentedSequences()
            .Build();
        var yaml = serializer.Serialize(this);
        yamlWriter.Write(yaml);
    }
}

/// <summary>
/// This class is used for deserializing the manifest so that members do not
/// need to made public or exposed as <c>List</c> objects.
/// </summary>
class ManifestBuilder
{
    /// <summary>
    /// Initializes a builder for deserialization.
    /// </summary>
    public ManifestBuilder()
    {
        this.Sources = new List<ManifestSource>();
        this.Views = new List<ManifestView>();
        this.Scripts = new List<ManifestScript>();
    }
    
    /// <summary>
    /// Sources <see cref="System.Collections.Generic.List{T}"/> used for deserialization.
    /// </summary>
    public List<ManifestSource> Sources { get; set; }

    /// <summary>
    /// Views <see cref="System.Collections.Generic.List{T}"/> used for deserialization.
    /// </summary>
    public List<ManifestView> Views { get; set; }
    
    /// <summary>
    /// Scripts <see cref="System.Collections.Generic.List{T}"/> used for deserialization.
    /// </summary>
    public List<ManifestScript> Scripts { get; set; }

    /// <summary>
    /// Initialize a new manifest with the deserialized collections.
    /// </summary>
    /// <param name="manifest"><see cref="Manifest"/> object to build in-place.</param>
    public void Build(Manifest manifest)
    {
        foreach (var sourceFile in Sources)
        {
            manifest.AddSource(sourceFile);
        }
        
        foreach (var viewFile in Views)
        {
            manifest.AddView(viewFile);
        }
        
        foreach (var scriptFile in Scripts)
        {
            manifest.AddScript(scriptFile);
        }
    }
}