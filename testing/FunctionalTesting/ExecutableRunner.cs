using System.Diagnostics;

namespace FunctionalTesting;

/// <summary>
/// This class runs a program and captures the standard output and error. The
/// output can be accessed and used for testing the program expectations.
/// </summary>
public class ExecutableRunner : IRunner
{
    /// <summary>
    /// Program to run (absolute or relative) 
    /// </summary>
    private readonly string _program;
    
    /// <summary>
    /// Standard output text.
    /// </summary>
    private readonly List<string> _output;
    
    /// <summary>
    /// Standard error text.
    /// </summary>
    private readonly List<string> _errors;
    
    /// <summary>
    /// Returned program exit code. Default is -1 for errors and timeout.
    /// </summary>
    private int _exitCode;

    /// <summary>
    /// Program args used for the run.
    /// </summary>
    private IEnumerable<string> _args;

    public ExecutableRunner(string program)
    {
        _program = program ?? throw new ArgumentNullException(nameof(program));
        
        _output = new List<string>();
        _errors = new List<string>();
        
        _exitCode = -1;
        _args = Enumerable.Empty<string>();
    }

    /// <summary>
    /// Returns the arguments used for the run.
    /// </summary>
    public IEnumerable<string> Args => _args;

    /// <summary>
    /// The OS exit code from the program.
    /// </summary>
    public int ExitCode => _exitCode;
    
    /// <summary>
    /// Number of standard output lines indexed.
    /// </summary>
    public int Lines => _output.Count;

    /// <summary>
    /// Number of standard error line indexed.
    /// </summary>
    public int Errors => _errors.Count;

    /// <summary>
    /// Returns the program.
    /// </summary>
    public string Program => _program;

    /// <summary>
    /// Enumerate over all of the error lines of text.
    /// </summary>
    public IEnumerable<string> AllErrors => _errors;

    /// <summary>
    /// Enumerate the output lines of text.
    /// </summary>
    public IEnumerable<string> AllLines => _output;

    /// <summary>
    /// Get the specified line of standard error text.
    /// </summary>
    /// <param name="index">The zero-based index of the text to get.</param>
    /// <returns>The error text at the index location.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is less than zero or greater than <see cref="Errors"/></exception>
    public string GetError(int index) => _errors[index];

    /// <summary>
    /// Get the specified line of standard output text.
    /// </summary>
    /// <param name="index">The zero-based index of the text to get.</param>
    /// <returns>The error text at the index location.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is less than zero or greater than <see cref="Errors"/></exception>
    public string GetLine(int index) => _output[index];

    /// <summary>
    /// Run the program with no arguments and wait indefinitely,
    /// </summary>
    /// <returns>The exit code from the program.</returns>
    public int Run()
    {
        var n = Run(Enumerable.Empty<string>(), -1);
        return n;
    }
    
    /// <summary>
    /// Run the program with the specified arguments.
    /// </summary>
    /// <param name="args">Arguments used to run the program.</param>
    /// <param name="timeoutSeconds">Timeout in seconds.</param>
    /// <returns>OS exit code from the program.</returns>
    public int Run(IEnumerable<string> args, int timeoutSeconds)
    {
        _args = args;
        
        _output.Clear();
        _errors.Clear();
        
        var p = new Process();

        p.StartInfo.CreateNoWindow = true;
        
        // Capture output and error console text locally.
        p.OutputDataReceived += (sender, eventArgs) => _output.Add(eventArgs.Data ?? "\n");
        p.ErrorDataReceived += (sender, eventArgs) => _errors.Add(eventArgs.Data ?? "\n");

        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;
        
        // Add in the args before we run the program.
        p.StartInfo.FileName = _program;
        foreach (var arg in args)
        {
            p.StartInfo.ArgumentList.Add(arg);
        }

        try
        {
            p.Start();

            // Required to actually get the events.
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            var timeoutMillis = timeoutSeconds * 1000;
            var exited = p.WaitForExit(timeoutMillis);
            if (exited)
            {
                // Call to get output events per the docs
                p.WaitForExit();
            }
            else
            {
                // program run timed out
                p.Kill();
                throw new Exception("Did not exit");
            }

            _exitCode = p.ExitCode;
            return _exitCode;
        }
        catch (SystemException e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            p.Close();
        }
    }
}