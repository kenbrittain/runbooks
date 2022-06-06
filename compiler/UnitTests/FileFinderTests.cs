using System;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Runbook;

public class FileFinderTests
{
    [Fact]
    public void FileFinder_BadDirectory_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new FileFinder(""));
    }
    
    [Fact]
    public void AddInclude_AddEmptyPatterns_ThrowsException()
    {
        var ff = new FileFinder("ROOT");
        Assert.Throws<ArgumentException>(() => ff.AddInclude(""));
        Assert.Throws<ArgumentException>(() => ff.AddExclude(""));
    }

    [Fact]
    public void Find_NoIncludePattern_FindsNoFiles()
    {
        var ff = new FileFinder("ROOT");
        
        var results = ff.Find();
        
        Assert.Empty(results);
    }

    [Fact]
    public void Find_IncludeAllFiles_FindAllFiles()
    {
        var files = new[] { "file.ext", "dir/file.ext" };
        var root = new InMemoryDirectoryInfo("/", files);
        var ff = new FileFinder(root);
        ff.AddInclude("**/*");
        
        var results = ff.Find();
        
        Assert.NotEmpty(results);
    }
}