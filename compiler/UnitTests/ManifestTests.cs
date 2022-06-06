using System;
using System.IO;

namespace Runbook;

public class ManifestTests
{
    [Fact]
    public void AddSource_AddObject_IncreasesSources()
    {
        var manifest = new Manifest();
        var source = new ManifestSource();

        manifest.AddSource(source);
        
        Assert.NotNull(manifest.Sources);
        Assert.NotEmpty(manifest.Sources);
    }

    [Fact]
    public void AddSource_WithNullObject_ThrowsException()
    {
        var manifest = new Manifest();
        Assert.Throws<ArgumentNullException>(() => manifest.AddSource(null!));
    }

    [Fact]
    public void AddView_AddExistingView_ReplacesView()
    {
        var manifest = new Manifest();
        var newView = new ManifestView
        {
            Name = "NAME",
            Generator = "NEW_TEST",
            Path = "NEW_PATH",
            FullPath = "NEW_FULLPATH"
        };
        var sameView = new ManifestView
        {
            Name = "NAME",
            Generator = "SAME_TEST",
            Path = "SAME_PATH",
            FullPath = "SAME_FULLPATH"
        };

        manifest.AddView(newView);
        manifest.AddView(sameView);
        
        Assert.NotEmpty(manifest.Views);
        Assert.Single(manifest.Views);
        Assert.Collection(manifest.Views, view => Assert.Equal("NAME", view.Name));
    }

    [Fact]
    public void AddView_AddNewView_AppendsViews()
    {
        var manifest = new Manifest();
        var view = new ManifestView
        {
            Name = "NAME",
            Generator = "TEST",
            Path = "PATH",
            FullPath = "FULLPATH"
        };

        manifest.AddView(view);
        
        Assert.NotEmpty(manifest.Views);
        Assert.Single(manifest.Views);
    }

    [Fact]
    public void Load_ValidYaml_UpdatesManifest()
    {
        var yaml = @"
sources: []
views: []
scripts: []
";

        using var reader = new StringReader(yaml);
        var manifest = Manifest.Load(reader);
    }

}