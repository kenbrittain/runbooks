using System.ComponentModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Spectre.Cli;
using Spectre.Console;
using Stubble.Core.Builders;
using ValidationResult = Spectre.Cli.ValidationResult;

namespace Runbook.Commands;

public class ExportRunbookSettings : RunbookSettings
{
    private static string[] SupportedMimeTypes = { "text/html", "text/markdown" };

    public ExportRunbookSettings()
    {
        this.Manifest = $".{Path.DirectorySeparatorChar}";
        this.ManifestIndex = false;
        this.MimeType = "text/markdown";
        this.OutputDir = ".";
    }
    
    [Description("Specifies the name of the manifest file.")]
    [CommandArgument(0, "<MANIFEST>")]
    public string Manifest { get; set; }

    [Description("Specifies the type of view to export.")]
    [CommandOption("-t|--type")]
    public string MimeType { get; set; }

    [Description("Directory to write exported files.")]
    [CommandOption("-o|--output")]
    public string OutputDir { get; set; }
    
    [Description("Write index file for the exported runbooks")]
    [CommandOption("--index")]
    public bool ManifestIndex { get; set; }

    public override ValidationResult Validate()
    {
        var foundMimeType = SupportedMimeTypes.FirstOrDefault(mt => mt == MimeType);
        if (foundMimeType == default(string))
        {
            return ValidationResult.Error($"The requested mimetype [{MimeType}] is not supported.");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// This command generates a View of the runbook and write it to an output
/// folder. The manifest is updated to include the new generated file for
/// tracking between runs.
/// </summary>
public class ExportRunbookCommand : RunbookCommand<ExportRunbookSettings>
{
    private readonly ILogger<ExportRunbookCommand> _logger;
    private readonly IFileSystem _fileSystem;
    
    public ExportRunbookCommand(
        IAnsiConsole console,
        ILogger<ExportRunbookCommand> logger,
        IFileSystem fileSystem)
        : base(console, logger, fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public override ErrorCode OnExecute(IConsoleDisplay console, IRunbookContext context, ExportRunbookSettings settings)
    {
        console.WriteVerbose(settings.Verbose, $"Using manifest file. [path={settings.Manifest}]");
        
        var manifestAttr = _fileSystem.File.GetAttributes(settings.Manifest);
        var isFile = manifestAttr.HasFlag(FileAttributes.Normal);

        var manifestDir = settings.Manifest; 
        if (isFile)
        {
            manifestDir = _fileSystem.Path.GetDirectoryName(settings.Manifest);
        }

        CreateDirectory(settings.OutputDir);

        var manifestFilePath = _fileSystem.Path.Combine(settings.Manifest, "manifest.yml");
        var manifest = Manifest.Load(manifestFilePath);
        foreach (var runbookSource in manifest.Sources)
        {
            console.UpdateStatus($"{runbookSource.Path}");
            Debug.Assert(runbookSource.Path != null);
            WriteView(runbookSource.Path, settings.MimeType, manifestDir, settings.OutputDir, manifest, context);
        }

        if (settings.ManifestIndex)
        {
            console.UpdateStatus($"Writing index file");
            WriteIndex(settings.MimeType, manifestDir, settings.OutputDir, manifest, context);
        }

        console.UpdateStatus($"Updating manifest.yml");
        manifest.Save(manifestFilePath);

        return ErrorCode.Success;
    }

    /// <summary>
    /// Render the embedded template using Mustache.
    /// </summary>
    /// <param name="templateName">The name of the embedded resource template.</param>
    /// <param name="context">The current runbook.</param>
    /// <returns>The content from the template.</returns>
    private string RenderTemplateContent(string templateName, IRunbookContext context)
    {
        var resource = new AssemblyResource(templateName, Assembly.GetCallingAssembly());
        Debug.Assert(resource.ResourceExists());
        var template = resource.GetResourceText();

        var stubble = new StubbleBuilder().Build();
        var content = stubble.Render(template, context.ToDictionary());
        return content;
    }

    /// <summary>
    /// Render and write the index file to the output directory.
    /// </summary>
    /// <param name="mimeType">The requested view type.</param>
    /// <param name="manifestDir">Location of the manifest file.</param>
    /// <param name="outputDir">Root of the requested output directory.</param>
    /// <param name="manifest">Manifest model.</param>
    /// <param name="context">The current runbook context.</param>
    private void WriteIndex(string mimeType, string manifestDir, string outputDir, Manifest manifest, IRunbookContext context)
    {
        var resourceName = "Runbook.Resources.Templates.Index.html";
        var fileExtension = ".html";
            
        if (mimeType == "text/markdown")
        {
            resourceName = "Runbook.Resources.Templates.Index.md";
            fileExtension = ".md";
        }

        var indexFile = _fileSystem.Path.ChangeExtension("index", fileExtension);
        var indexDir = GetOutputDirectory(manifestDir, outputDir, "index.html");
        var indexPath = Path.Combine(indexDir, indexFile);

        var content = RenderTemplateContent(resourceName, context);
        WriteFileContents(indexPath, content);
            
        // update the manifest
        var v = new ManifestView
        {
            Generator = resourceName,
            Name = _fileSystem.Path.GetFileNameWithoutExtension(indexPath),
            Path = RemoveOutputDirectory(indexPath, outputDir),
            FullPath = _fileSystem.Path.GetFullPath(indexPath)
        };
        manifest.AddView(v);        
    }

    /// <summary>
    /// Render and write the view to the output directory.
    /// </summary>
    /// <param name="sourcePath">The Yaml source file path.</param>
    /// <param name="mimeType">The requested view type.</param>
    /// <param name="manifestDir">Location of the manifest file.</param>
    /// <param name="outputDir">Root of the requested output directory.</param>
    /// <param name="manifest">Manifest model.</param>
    /// <param name="context">The current runbook context.</param>
    private void WriteView(string sourcePath, string mimeType, string manifestDir, string outputDir, Manifest manifest, IRunbookContext context)
    {
        var runbookPath = _fileSystem.Path.Combine(manifestDir, sourcePath);

        var fileStream = new FileStream(runbookPath, FileMode.Open);
        var fileReader = new StreamReader(fileStream);
        var runbook = Runbook.Load(fileReader);
            
        // Setup the context for rendering
        context.Set("Runbook", runbook);
        context.Set("Manifest", manifest);

        // Define the template names
        var resourceName = "Runbook.Resources.Templates.Runbook.html";
        var fileExtension = ".html";
            
        if (mimeType == "text/markdown")
        {
            resourceName = "Runbook.Resources.Templates.Runbook.md";
            fileExtension = ".md";
        }
            
        // Fixup the filename
        var runbookFileName = Path.GetFileName(runbookPath);
        var contentFileName = Path.ChangeExtension(runbookFileName, fileExtension);

        // Get the output dir based path
        var contentDir = GetOutputDirectory(manifestDir, outputDir, runbookPath);
        var contentPath = Path.Combine(contentDir, contentFileName);

        // Write the content
        var content = RenderTemplateContent(resourceName, context);
        WriteFileContents(contentPath, content);

        // Update the manifest
        var v = new ManifestView
        {
            Generator = resourceName,
            Name = _fileSystem.Path.GetFileNameWithoutExtension(contentPath),
            Path = RemoveOutputDirectory(contentPath, outputDir),
            FullPath = _fileSystem.Path.GetFullPath(contentPath)
        };
        manifest.AddView(v);
        
    }

    /// <summary>
    /// Remove the output directory from the given path. This method is used to
    /// generate a relative path from a source file.
    /// </summary>
    /// <param name="path">Path to remove output from.</param>
    /// <param name="output">Output path to check for.</param>
    /// <returns>The relative path to the content.</returns>
    private string RemoveOutputDirectory(string path, string output)
    {
        var result = path;
        
        var outputInPath = path.StartsWith(output);
        if (outputInPath)
        {
            result = path.Substring(output.Length);
            var directorySeparator = result.StartsWith($"{Path.DirectorySeparatorChar}");
            if (directorySeparator)
            {
                result = result.Substring($"{Path.DirectorySeparatorChar}".Length);
            }
        }

        return result;
    }
}