namespace Runbook;

/// <summary>
/// Collection of exit errorCode to the OS. Commands execute and return something
/// from this enumeration to indicate what happened.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// Use this errorCode when the command completes as expected.
    /// </summary>
    Success = 0, // A generic yeah!

    /// <summary>
    /// Use this errorCode when you have no other options.
    /// </summary>
    Failure = 1,

    /// <summary>
    /// The specified file or directory path contains unsupported characters.
    /// </summary>
    PathHasInvalidChars = 2,

    /// <summary>
    /// The specified mime type is not supported.
    /// </summary>
    MimeTypeNotFound = 3,

    /// <summary>
    /// The specified manifest file was not found. 
    /// </summary>
    ManifestFileNotFound = 4,

    /// <summary>
    /// The specified directory not not found.
    /// </summary>
    DirectoryNotFound = 5,

    /// <summary>
    /// The specified path is too long for the operating system.
    /// </summary>
    PathTooLong = 6,
    
    /// <summary>
    /// Access to the resource is denied
    /// </summary>
    UnauthorizedAccess = 7,
    
    /// <summary>
    /// The path contains invalid characters.
    /// </summary>
    DirectoryIsFilePath = 8,
    
    /// <summary>
    /// The directory name contains invalid characters.
    /// </summary>
    DirectoryHasInvalidChars = 9
}
