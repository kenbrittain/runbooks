using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Cli;
using Spectre.Console;

namespace Runbook.Commands;

public class DebugSettings : CommandSettings
{
}

public class DebugRunbookCommand : RunbookCommand<DebugSettings>
{
    public DebugRunbookCommand(IAnsiConsole console, ILogger<RunbookCommand<DebugSettings>> logger, IFileSystem fs)
        : base(console, logger, fs)
    {
    }

    public override ErrorCode OnExecute(IConsoleDisplay console, IRunbookContext context, DebugSettings settings)
    {
        throw new System.NotImplementedException();
    }
}

public class RunbookCommandTests
{
    private static readonly string PathToNowhere = "/path/to/nowhere";
    
    private readonly Mock<IAnsiConsole> _mockConsole;
    private readonly Mock<IFileSystem> _mockFileSystem;
    private readonly Mock<ILogger<DebugRunbookCommand>> _mockLogger;

    public RunbookCommandTests()
    {
        _mockConsole = new Mock<IAnsiConsole>();
        _mockFileSystem = new Mock<IFileSystem>();
        _mockLogger = new Mock<ILogger<DebugRunbookCommand>>();
    }

    [Fact]
    public void CreateDirectory_NullDir_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentNullException>(() => command.CreateDirectory(null!));
    }

    [Fact]
    public void CreateDirectory_EmptyDir_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentException>(() => command.CreateDirectory(string.Empty));
    }

    [Fact]
    public void CreateDirectory_InaccessibleDir_ThrowsException()
    {
        _mockFileSystem.Setup(fs => fs.Directory.CreateDirectory(PathToNowhere)).Throws<UnauthorizedAccessException>();
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);

        var exception = Assert.Throws<RunbookException>(() => command.CreateDirectory(PathToNowhere));
        Assert.Equal(ErrorCode.UnauthorizedAccess, exception.ErrorCode);
    }

    [Fact]
    public void CreateDirectory_DirNotFound_ThrowsException()
    {
        _mockFileSystem.Setup(fs => fs.Directory.CreateDirectory(PathToNowhere)).Throws<DirectoryNotFoundException>();
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        
        var exception = Assert.Throws<RunbookException>(() => command.CreateDirectory(PathToNowhere));
        Assert.Equal(ErrorCode.DirectoryNotFound, exception.ErrorCode);
    }

    [Fact]
    public void CreateDirectory_DirectoryIsFile_ThrowsException()
    {
        _mockFileSystem.Setup(fs => fs.Directory.CreateDirectory(It.IsAny<string>())).Throws<IOException>();
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);

        var exception = Assert.Throws<RunbookException>(() => command.CreateDirectory(PathToNowhere));
        Assert.Equal(ErrorCode.DirectoryIsFilePath, exception.ErrorCode);
        _mockFileSystem.Verify(fs => fs.Directory.CreateDirectory(PathToNowhere));
    }

    [Fact]
    public void CreateDirectory_InvalidPathChars_ThrowsException()
    {
        _mockFileSystem.Setup(fs => fs.Directory.CreateDirectory(It.IsAny<string>())).Throws<NotSupportedException>();
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);

        var exception = Assert.Throws<RunbookException>(() => command.CreateDirectory(PathToNowhere));
        Assert.Equal(ErrorCode.PathHasInvalidChars, exception.ErrorCode);
        _mockFileSystem.Verify(fs => fs.Directory.CreateDirectory(PathToNowhere));
    }

    [Fact]
    public void CreateFileDirectory_NullPath_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentNullException>(() => command.CreateFileDirectory(null!));
    }

    [Fact]
    public void CreateFileDirectory_EmptyPath_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentException>(() => command.CreateFileDirectory(string.Empty));
    }

    [Fact]
    public void CreateFileDirectory_PathTooLong_ThrowsException()
    {
        _mockFileSystem.Setup(fs => fs.Path.GetDirectoryName(It.IsAny<string>())).Throws<PathTooLongException>();
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);

        var exception = Assert.Throws<RunbookException>(() => command.CreateFileDirectory(PathToNowhere));
        Assert.Equal(ErrorCode.PathTooLong, exception.ErrorCode);
        _mockFileSystem.Verify(fs => fs.Path.GetDirectoryName(PathToNowhere));
    }

    [Fact]
    public void WriteFileContents_NullPath_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentNullException>(() => command.WriteFileContents(null!, "CONTENTS"));
    }
    
    [Fact]
    public void WriteFileContents_EmptyPath_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        Assert.Throws<ArgumentException>(() => command.WriteFileContents(string.Empty, "CONTENTS"));
    }

    [Fact]
    public void WriteFileContents_NullContents_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        string nullString = null!;
        Assert.Throws<ArgumentNullException>(() => command.WriteFileContents(PathToNowhere, nullString));
    }
    
    [Fact]
    public void GetOutputDirectory_SourceSameAsOutput_ThrowsException()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, new FileSystem());
        Assert.Throws<ArgumentException>(() => command.GetOutputDirectory("SOURCE", "SOURCE", "SOURCE/CONTENT.EXT"));
    }
    
    [Fact]
    public void GetOutputDirectory_OutputInsideSource_JustOutput()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, new FileSystem());
        var dir = command.GetOutputDirectory("SOURCE", "SOURCE/OUTPUT", "SOURCE/CONTENT.EXT");
        Assert.Equal("SOURCE/OUTPUT", dir);
    }

    [Fact]
    public void GetOutputDirectory_OutputOutsideSource_JustOutput()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, new FileSystem());
        var dir = command.GetOutputDirectory("SOURCE", "OUTPUT", "SOURCE/CONTENT.EXT");
        Assert.Equal("OUTPUT", dir);
    }
    
    [Fact]
    public void GetOutputDirectory_SubDirectory_JustOutput()
    {
        var command = new DebugRunbookCommand(_mockConsole.Object, _mockLogger.Object, new FileSystem());
        var dir = command.GetOutputDirectory("SOURCE", "OUTPUT", "SOURCE/DIR/CONTENT.EXT");
        Assert.Equal("OUTPUT/DIR", dir);
    }
}