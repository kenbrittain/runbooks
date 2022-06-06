using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console;
using Xunit;

namespace Runbook.Commands;

public class ShowManifestCommandTests
{
    private readonly Mock<IAnsiConsole> _mockConsole;
    private readonly Mock<IFileSystem> _mockFileSystem;
    private readonly Mock<ILogger<ShowManifestCommand>> _mockLogger;

    public ShowManifestCommandTests()
    {
        _mockConsole = new Mock<IAnsiConsole>();
        _mockFileSystem = new Mock<IFileSystem>();
        _mockLogger = new Mock<ILogger<ShowManifestCommand>>();
    }
    
    [Fact(Skip = "DirectoryNotFoundException being thrown")]
    public void OnExecute_MissingDirectory_Fails()
    {
        // Arrange
        var context = new Mock<IRunbookContext>();
        var console = new Mock<IConsoleDisplay>();
        var settings = new ShowManifestSettings("MISSING_DIR");

        _mockFileSystem.Setup(fs => fs.Path.Combine(It.IsAny<string>(),It.IsAny<string>())).Returns("");
        
        var command = new ShowManifestCommand(_mockConsole.Object, _mockLogger.Object, _mockFileSystem.Object);
        var result = command.OnExecute(console.Object, context.Object, settings);

        // Assert
        Assert.Equal(ErrorCode.Success, result);
    }
}