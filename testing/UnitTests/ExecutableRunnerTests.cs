using FunctionalTesting;

namespace UnitTests;

public class ExecutableRunnerTests
{
    [Fact]
    public void ExecutableRunner_CommandIsNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new ExecutableRunner(null!));
    }
    
    [Fact]
    public void ExecutableRunner_NothingRun_HasZeroLines()
    {
        // Arrange
        var runner = new ExecutableRunner("COMMAND");
        
        // Assert
        Assert.Equal(0, runner.Lines);
    }
    
    [Fact]
    public void GetLines_BadIndex_ThrowsException()
    {
        // Arrange
        var runner = new ExecutableRunner("COMMAND");
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => runner.GetLine(999));
    }
    
    [Fact(Skip = "Use for testing ExecutableRunner only")]
    public void LS_NoArgs_ReturnsSuccess()
    {
        // Arrange
        var ls = new ExecutableRunner("ls");
        
        // Act
        ls.Run();
        
        // Assert
        Assert.Equal(0, ls.ExitCode);
    }

    /*
    [Fact]
    public void Method_Scenario_Expectation()
    {
        // Arrange
        // Act
        // Assert
    }
    */
}