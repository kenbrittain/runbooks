using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Cli;
using Spectre.Console;

namespace Runbook.Commands;

public class ShowManifestSettings : RunbookSettings
{
    public ShowManifestSettings()
        : this($".{Path.PathSeparator}")
    {
    }

    public ShowManifestSettings(string dir)
    {
        this.Directory = dir;
    }
    
    [Description("Specifies the name of the manifest file")]
    [CommandArgument(0, "<MANIFEST>")]
    public string Directory { get; set; }
}

/// <summary>
/// Command to create the manifest file locally for later processing. The
/// command allows for adding and excluding files to process.
/// </summary>
public class ShowManifestCommand : RunbookCommand<ShowManifestSettings>
{
    private readonly ILogger<ShowManifestCommand> _logger;
    private readonly IFileSystem _fs;
    
    public ShowManifestCommand(IAnsiConsole console, ILogger<ShowManifestCommand> logger, IFileSystem fs) 
        : base(console, logger, fs)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fs = fs ?? throw new ArgumentNullException(nameof(fs));
    }

    public override ErrorCode OnExecute(IConsoleDisplay console, IRunbookContext context, ShowManifestSettings settings)
    {
        try
        {
            var path = Path.Combine(settings.Directory, "manifest.yml");
            var manifest = Manifest.Load(path);

            var jsonWriter = new StringWriter();  
            manifest.Save(jsonWriter);

            var json = jsonWriter.ToString();
            console.WriteLine(json);

            return ErrorCode.Success;
        }
        catch (DirectoryNotFoundException e)
        {
            console.WriteVerbose(settings.Verbose, $"Directory not found. [dir={settings.Directory}]");
            _logger.LogError(e, "Manifest directory not found. [dir={dir}]", settings.Directory);
            return ErrorCode.Failure;
        }
    }
}