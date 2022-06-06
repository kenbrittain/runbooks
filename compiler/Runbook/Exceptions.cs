using System.Diagnostics;
using System.Runtime.Serialization;
using Runbook.Commands;

namespace Runbook;

public class RunbookException : Exception
{
    public RunbookException()
    {
        this.ErrorCode = ErrorCode.Failure;
    }

    public RunbookException(ErrorCode errorCode) 
        : base()
    {
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new exception with the specified error code and inner
    /// exception. 
    /// </summary>
    /// <param name="errorCode">The runbook error code.</param>
    /// <param name="cause">The cause of this exception.</param>
    public RunbookException(ErrorCode errorCode, Exception cause) 
        : base(cause.Message, cause)
    {
        this.ErrorCode = errorCode;
    }

    public RunbookException(ErrorCode errorCode, string? message)
        : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public RunbookException(ErrorCode errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        this.ErrorCode = errorCode;
    }

    public ErrorCode ErrorCode { get; }
}