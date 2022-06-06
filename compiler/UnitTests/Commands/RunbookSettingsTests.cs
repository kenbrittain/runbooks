using Spectre.Cli;

namespace Runbook.Commands;

public class RunbookSettingsTests
{
    [Fact]
    public void Validate_ZeroDefinesPresent_SucceedsValidation()
    {
        var settings = new RunbookSettings();
        var valid = settings.Validate();
        Assert.True(valid.Successful);
    }

    [Fact]
    public void Validate_OneProperlyFormattedDefine_SucceedsValidation()
    {
        var settings = new RunbookSettings();
        settings.Defines = new[] { "NAME=VALUE" };
        
        var valid = settings.Validate();
        
        Assert.True(valid.Successful);
    }

    [Fact]
    public void Validate_MultipleProperlyFormattedDefines_SucceedsValidation()
    {
        var settings = new RunbookSettings();
        settings.Defines = new[] { "NAME1=VALUE1", "NAME2=DEFINE2" };
        
        var valid = settings.Validate();
        
        Assert.True(valid.Successful);
    }

    [Fact]
    public void Validate_OneBadDefine_ErrorsValidation()
    {
        var settings = new RunbookSettings();
        settings.Defines = new[] { "NAME" };
        
        var valid = settings.Validate();
        
        Assert.False(valid.Successful);
    }

    [Fact]
    public void Validate_ManyDefinesWithSingleBad_ErrorsValidation()
    {
        var settings = new RunbookSettings();
        settings.Defines = new[] { "NAME1=VALUE1", "NAME2=DEFINE2", "BAD_NAME" };
        
        var valid = settings.Validate();
        
        Assert.False(valid.Successful);
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