namespace FunctionalTesting;

/// <summary>
/// Represents the ability to run programs for testing. The output will be
/// indexed and made accessible to be used for testing message, etc.
/// </summary>
public interface IRunner
{
    /// <summary>
    /// The arguments passed to the program when run. This can be an empty if
    /// no arguments were given. 
    /// </summary>
    IEnumerable<string> Args { get; }
    
    /// <summary>
    /// Returns the number of error output lines of text.
    /// </summary>
    public int Errors { get; }
    
    /// <summary>
    /// The program exit code. This will be -1 if the run was timed out and
    /// the program was killed.
    /// </summary>
    int ExitCode { get; }
    
    /// <summary>
    /// Returns the number of standard output lines of text.
    /// </summary>
    int Lines { get; }
    
    /// <summary>
    /// The program command line that was executed.
    /// </summary>
    string Program { get; }

    /// <summary>
    /// Enumerate over all of the error lines of text.
    /// </summary>
    IEnumerable<string> AllErrors { get;  }

    /// <summary>
    /// Enumerate the output lines of text.
    /// </summary>
    IEnumerable<string> AllLines { get; }

    /// <summary>
    /// Gets the indexed text from the error output.
    /// </summary>
    /// <param name="line">The line of text to get.</param>
    /// <returns>The line of text from the output.</returns>
    /// <exception cref="IndexOutOfRangeException">The index is less than zero or greater than <see cref="Errors"/>.</exception>
    string GetError(int line);
    
    /// <summary>
    /// Gets the indexed text from the console output.
    /// </summary>
    /// <param name="index">The line of text to get.</param>
    /// <returns>The line of text from the output.</returns>
    /// <exception cref="IndexOutOfRangeException">The index is less than zero or greater than <see cref="Lines"/>.</exception>
    string GetLine(int index);
    
    /// <summary>
    /// Run the system under test and indexes the console output.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <para name="timeout">Seconds before timeout when running.</para>
    /// <returns>Returns the exit code from the program run.</returns>
    int Run(IEnumerable<string> args, int timeout);
}