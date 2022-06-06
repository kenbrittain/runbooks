using System.Collections.Generic;
using System.Linq;

namespace Runbook;

public class RunbookContextTests
{
    [Fact]
    public void RunbookContext_Default_CreatesEmptyContext()
    {
        var context = new RunbookContext();
        Assert.Empty(context.Names);
    }
    
    [Fact]
    public void Get_GetValue_ReturnsValues()
    {
        var context = new RunbookContext();
        context.Set("NAME", "VALUE");

        var v = context.Get<string>("NAME");
        
        Assert.Equal("VALUE", v);
    }

    [Fact]
    public void Get_MissingValue_ThrowsException()
    {
        var context = new RunbookContext();
        Assert.Throws<KeyNotFoundException>(() => context.Get<string>("MISSING"));
    }

    [Fact]
    public void TryGet_MissingProperty_Fails()
    {
        var context = new RunbookContext();
        var found = context.TryGet("NAME", out object? _);
        Assert.False(found);
    }

    [Fact]
    public void TryGet_ExistingProperty_Succeeds()
    {
        var context = new RunbookContext();
        context.Set("NAME", "VALUE");
        
        var found = context.TryGet("NAME", out object? value);
        
        Assert.True(found);
        Assert.Equal("VALUE", value);
    }
}