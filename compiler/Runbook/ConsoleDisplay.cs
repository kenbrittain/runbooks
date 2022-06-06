using Spectre.Console;

namespace Runbook;

/// <summary>
/// Defines the methods for interacting with the console from a command.
/// </summary>
public interface IConsoleDisplay
{
    /// <summary>
    /// Write an exception to the console.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="ex">Exception to display.</param>
    void WriteError(string text, Exception? ex = null);

    /// <summary>
    /// Display text on the console.
    /// </summary>
    /// <param name="text">The text to display.</param>
    void WriteLine(string text);
    
    /// <summary>
    /// Conditionally display text on on the console. Verbose output will
    /// be formatted differently than normal output. 
    /// </summary>
    /// <param name="verbose">Displays the text if <c>true</c>.</param>
    /// <param name="text">Text to display.</param>
    void WriteVerbose(bool verbose, string text);
    
    /// <summary>
    /// Update the status message.
    /// </summary>
    /// <param name="status">Text to display for this update.</param>
    void UpdateStatus(string status);
}

/// <summary>
/// Implementation of the <see cref="IConsoleDisplay"/> for the
/// Spectre.Console package's <see cref="IAnsiConsole"/>.
/// </summary>
public class AnsiConsoleDisplay : IConsoleDisplay
{
    /// <summary>
    /// Text used for the verbose prefix in output.
    /// </summary>
    private static string VerbosePrefix = "===>";

    private readonly IAnsiConsole _console;
    private StatusContext? _status;
    
    public AnsiConsoleDisplay(IAnsiConsole console, StatusContext status)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _status = status ?? throw new ArgumentNullException(nameof(status));
    }

    /// <inheritdoc/>
    public void WriteError(string text, Exception? e)
    {
        WriteLine(text);
        if (e != null)
        {
            _console.WriteException(e);
        }
    }

    /// <inheritdoc/>
    public void WriteLine(string text)
    {
        _console.WriteLine(text, Style.Plain);
    }

    /// <inheritdoc/>
    public void UpdateStatus(string status)
    {
        _status?.Status(status);
    }

    /// <inheritdoc/>
    public void WriteVerbose(bool verbose, string text)
    {
        _console.WriteLine($"{VerbosePrefix} {text}", Style.Plain);
    }
}