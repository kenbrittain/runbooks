using System.ComponentModel;
using System.IO.Abstractions;

using Microsoft.Extensions.Logging;

using Spectre.Cli;
using Spectre.Console;

namespace Runbook.Commands;

class CreateManifestSettings : RunbookSettings
{
    public CreateManifestSettings()
    {
        this.Directory = $"{Path.PathSeparator}";
    }

    [Description("Specifies the name of the manifest file")]
    [CommandArgument(0, "<MANIFEST>")]
    public string? Directory { get; set; }

    [Description("File pattern to include")]
    [CommandOption("--include")]
    public string[]? Include { get; set; }
    
    [Description("File pattern to exclude")]
    [CommandOption("--exclude")]
    public string[]? Exclude { get; set; }
    
    [Description("Overwrites an existing manifest if it exists")]
    [CommandOption("-f|--force")]
    public bool Force { get; set; }
}

/// <summary>
/// Command to create the manifest file locally for later processing. The
/// command allows for adding and excluding files to process.
/// </summary>
class CreateManifestCommand : RunbookCommand<CreateManifestSettings>
{
    private readonly ILogger<CreateManifestCommand> _logger;
    private readonly IFileSystem _fileSystem;
    
    public CreateManifestCommand(IFileSystem fileSystem, IAnsiConsole console, ILogger<CreateManifestCommand> logger) 
        : base(console, logger, fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override ErrorCode OnExecute(IConsoleDisplay console, IRunbookContext context, CreateManifestSettings settings)
    {
        var includes = GetPatternsList(settings.Include);
        var excludes = GetPatternsList(settings.Exclude);
        var dir = settings.Directory ?? $".{Path.PathSeparator}";
        
        // If no patterns are given then add the default
        if (includes.Count == 0)
        {
            var yamlPattern = $"**{_fileSystem.Path.DirectorySeparatorChar}*.yml";
            includes.Add(yamlPattern);
        }
        
        try
        {
            var manifest = new Manifest();

            manifest.Find(dir, includes, excludes);
            var path = _fileSystem.Path.Combine(dir, "manifest.yml");
            manifest.Save(path);

            console.WriteVerbose(settings.Verbose, $"Wrote manifest. [name={manifest.ToString()},sources={manifest.Sources.Count()},views={manifest.Views.Count()},scripts={manifest.Sources.Count()}]");
            return ErrorCode.Success;
        }
        catch (DirectoryNotFoundException e)
        {
            console.WriteVerbose(settings.Verbose, $"Directory not found. [dir={settings.Directory}]");
            _logger.LogError(e, "Manifest directory not found. [dir={dir}]", settings.Directory);
            return ErrorCode.Failure;
        }
    }
    
    // ------------------------------------------------------------------------
    // PRIVATE
    // ------------------------------------------------------------------------

    /// <summary>
    /// Convert the specified patterns into a list for processing.
    /// </summary>
    /// <param name="source">The settings parameter to convert.</param>
    /// <returns>This will always return a valid <c>List</c> object (even if empty).</returns>
    private List<string> GetPatternsList(string[]? source)
    {
        var items = new List<string>();

        if (source != null)
        {
            items.AddRange(source);
        }

        return items;
    }
}