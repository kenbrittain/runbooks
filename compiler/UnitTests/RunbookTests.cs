namespace Runbook.UnitTests;

public class RunbookModelTests
{
    [Fact]
    public void DefaultCtorCreateStepsProperty()
    {
        var model = new Runbook();
        Assert.NotNull(model.Steps);
    }

    [Fact]
    public void AddStepIncreasesStepsCount()
    {
        var model = new Runbook { Title = "TITLE", Summary = "SUMMARY", Author = "AUTHOR", Contact = "CONTACT" };
        model.AddStep(new RunbookStep { Name = "NAME", Summary = "SUMMARY", Note = "NOTE" });
        Assert.NotEmpty(model.Steps);
    }

    [Fact]
    public void DefaultCtorUsedForDeserializationMakesInvalidObject()
    {
        var model = new Runbook();
        var valid = model.IsValid();
        Assert.False(valid);
    }
}