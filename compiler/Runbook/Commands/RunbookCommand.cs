using System.ComponentModel;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Cli;
using Spectre.Console;

namespace Runbook.Commands;

/// <summary>
/// Common settings for runbook commands.
/// </summary>
public class RunbookSettings : CommandSettings
{
    public RunbookSettings()
    {
    }

    [Description("Display more information")]
    [CommandOption("--verbose")]
    public bool Verbose { get; init; }

    [Description("Set a named value in the runbook context")]
    [CommandOption("-D|--define")]
    public string[]? Defines { get; set; }

    public override Spectre.Cli.ValidationResult Validate()
    {
        // Check the definitions and stop at the first error.
        if (Defines != null)
        {
            foreach (var definition in Defines)
            {
                if (!IsKeyValuePair(definition))
                {
                    return Spectre.Cli.ValidationResult.Error($"{definition} is not in the `Key=Value` format.");
                }
            }
        }
        return Spectre.Cli.ValidationResult.Success();

        bool IsKeyValuePair(string definition)
        {
            if (definition != null)
            {
                var keyValuePairParts = definition.Split('=');
                if (keyValuePairParts.Length == 2)
                {
                    if (keyValuePairParts[0].Length > 0 && keyValuePairParts[1].Length > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

/// <summary>
/// Defines common properties and helpers for commands. All runbook
/// commands should be derived from this class and implement the
/// <c>OnExecute</c> method.
/// </summary>
/// <typeparam name="TSettings">The settings for this command.</typeparam>
public abstract class RunbookCommand<TSettings> : Command<TSettings>
    where TSettings : CommandSettings
{
    /// <summary>
    /// Used for interacting with the console.   
    /// </summary>
    private readonly IAnsiConsole _console;

    private readonly IFileSystem _fs;

    /// <summary>
    /// Used for logging command information.
    /// </summary>
    private readonly ILogger<RunbookCommand<TSettings>> _logger;

    /// <summary>
    /// Creates a new <see cref="RunbookCommand{TSettings}"/>
    /// </summary>
    /// <param name="console">The console for this command.</param>
    /// <param name="logger">The logger for this command.</param>
    /// <exception cref="ArgumentNullException">Thrown is either parameter is <c>null</c>.</exception>
    protected RunbookCommand(IAnsiConsole console, ILogger<RunbookCommand<TSettings>> logger, IFileSystem fs)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fs = fs ?? throw new ArgumentNullException(nameof(fs));
    }

    /// <summary>
    /// Execute our version of the command.
    /// </summary>
    /// <param name="_">We've had no use for this context yet.</param>
    /// <param name="settings">Settings for this command.</param>
    /// <returns>zero `0` for success; otherwise an error value.</returns>
    public override int Execute(CommandContext _, TSettings settings)
    {
        var result = ErrorCode.Failure;

        try
        {
            AnsiConsole.Status()
                .Spinner(Spinner.Known.SimpleDots)
                .Start("Starting", status =>
                {
                    var context = new RunbookContext();
                    var console = new AnsiConsoleDisplay(_console, status);
                    result = OnExecute(console, context, settings);
                    _logger.LogInformation($"OnExecute complete. [result={result}]");
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return (int)result;
    }

    /// <summary>
    /// Override this method in the command classes to perform the requested
    /// work.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="settings"></param>
    /// <para name="console"></para>
    /// <returns></returns>
    public abstract ErrorCode OnExecute(IConsoleDisplay console, IRunbookContext context, TSettings settings);

    // ========================================================================
    // PROTECTED
    // ========================================================================

    /// <summary>
    /// Create a directory and subdirectories in the path and log all errors.
    /// </summary>
    /// <param name="dir">The directory to create.</param>
    /// <exception cref="ArgumentNullException">The directory is null.</exception>
    public void CreateDirectory(string dir)
    {
        if (dir == null)
        {
            throw new ArgumentNullException(nameof(dir));
        }

        if (dir == string.Empty)
        {
            throw new ArgumentException(nameof(dir));
        }

        try
        {
            var dirInfo = _fs.Directory.CreateDirectory(dir);
            _logger.LogInformation("Directory created. [dir={dir},path={path}]", dir, dirInfo.FullName);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError(e, "Unauthorized access to directory. [dir={dir}]", dir);
            throw new RunbookException(ErrorCode.UnauthorizedAccess, e);
        }
        catch (DirectoryNotFoundException e)
        {
            _logger.LogError(e, "Directory not found. [dir={dir}]", dir);
            throw new RunbookException(ErrorCode.DirectoryNotFound, e);
        }
        catch (IOException e)
        {
            _logger.LogError(e, "Directory name is a file path. [dir={dir}]", dir);
            throw new RunbookException(ErrorCode.DirectoryIsFilePath, e);
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, "Directory name contains invalid characters. [dir={dir}]", dir);
            throw new RunbookException(ErrorCode.DirectoryHasInvalidChars, e);
        }
        catch (NotSupportedException e)
        {
            _logger.LogError(e, "Path contains a colon. [dir={dir}]", dir);
            throw new RunbookException(ErrorCode.PathHasInvalidChars, e);
        }
    }

    /// <summary>
    /// Create the directory and all subdirectories in the file path.
    /// </summary>
    /// <param name="path">The file path.</param>
    public void CreateFileDirectory(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path == string.Empty)
        {
            throw new ArgumentException(nameof(path));
        }

        try
        {
            var dir = _fs.Path.GetDirectoryName(path);
            var dirExists = _fs.Directory.Exists(dir);
            if (!dirExists)
            {
                CreateDirectory(dir);
            }
        }
        catch (PathTooLongException e)
        {
            _logger.LogError("Path too long. [path={path}]", path);
            throw new RunbookException(ErrorCode.PathTooLong, e);
        }
    }

    /*
     * This method calculates the output directory required to hold the content
     * file using the source location as the model. It recreates all of the
     * needed subdirectories off of the source location. This allows for a
     * parallel output directory structure to be created.
     *
     * sourceDir = root directory where the files were found {~/content}
     * outputDir = root output dir (can be located in rootDir) {~/content/_out}
     * contentPath = path to the content being copied {~/content/blog/posts/index.html}
     * 
     * offsetPath = path to the destination file {blog/posts/index.html}
     * offsetDir = directory from the rootDir where the file is located {blog/posts/}
     * outputPath = {~/content/_out/blog/posts/index.html}
     */

    public string GetOutputDirectory(string sourceDir, string outputDir, string contentPath)
    {
        if (sourceDir == outputDir)
        {
            throw new ArgumentException();
        }

        var offsetPath = contentPath;
        var contentStartsWithSource = contentPath.StartsWith(sourceDir);
        if (contentStartsWithSource)
        {
            offsetPath = contentPath.Substring(sourceDir.Length);
            var offsetStartWithDirSeparator = offsetPath.StartsWith(_fs.Path.DirectorySeparatorChar);
            if (offsetStartWithDirSeparator)
            {
                offsetPath = offsetPath.Substring(1);
            }
        }

        var offsetDir = _fs.Path.GetDirectoryName(offsetPath);

        var dir = _fs.Path.Combine(outputDir, offsetDir);
        return dir;
    }

    /// <summary>
    /// Write the contents of the reader to the file.
    /// </summary>
    /// <param name="filePath">The file to write.</param>
    /// <param name="reader">The contents of the file.</param>
    public void WriteFileContents(string filePath, StreamReader reader)
    {
        var contents = reader.ReadToEnd();
        WriteFileContents(filePath, contents);
    }

    /// <summary>
    /// Write the contents of the reader to the file.
    /// </summary>
    /// <param name="filePath">The file to write.</param>
    /// <param name="contents">The contents of the file.</param>
    public void WriteFileContents(string filePath, string contents)
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (filePath == string.Empty)
        {
            throw new ArgumentException(nameof(filePath));
        }

        if (contents == null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        CreateFileDirectory(filePath);
        _fs.File.WriteAllText(filePath, contents);
    }
}