using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Runbook;

public class AssemblyResourceTests
{
    private readonly Mock<Assembly> _mockAssembly;

    public AssemblyResourceTests()
    {
        _mockAssembly = new Mock<Assembly>();
    }
    
    [Fact]
    public void AssemblyResource_NullParameters_ThrowsExceptions()
    {
        Assert.Throws<ArgumentNullException>(() => new AssemblyResource("RESOURCE_00", null!));
        Assert.Throws<ArgumentNullException>(() => new AssemblyResource(null!, Assembly.GetExecutingAssembly()));
    }

    [Fact]
    public void AssemblyResource_SpecifiedAssembly_UsesGivenAssembly()
    {
        var resource = new AssemblyResource("RESOURCE_00", _mockAssembly.Object);
        Assert.Same(_mockAssembly.Object, resource.ResourceAssembly);
    }

    [Fact]
    public void GetResourceStream_BadResourceName_ReturnsNull()
    {
        var bytes = Encoding.ASCII.GetBytes("ANYTHING");
        using var stream = new MemoryStream(bytes);
        
        _mockAssembly.Setup(asm => asm.GetManifestResourceStream("RESOURCE_01")).Returns(stream);
        
        var resource = new AssemblyResource("RESOURCE_00", _mockAssembly.Object);
        using var textStream = resource.GetResourceStream();
        
        Assert.Null(textStream);
    }

    [Fact]
    public void GetResourceStream_ResourceNameExists_ReturnsStream()
    {
        var bytes = Encoding.ASCII.GetBytes("RESOURCE_TEXT");
        using var stream = new MemoryStream(bytes);

        _mockAssembly.Setup(asm => asm.GetManifestResourceStream("RESOURCE_01")).Returns(stream);
        
        var resource = new AssemblyResource("RESOURCE_01", _mockAssembly.Object);
        using var textStream = resource.GetResourceStream();

        Assert.NotNull(textStream);
    }
    
    [Fact]
    public void GetResourceText_BadResourceName_ReturnsNull()
    {
        var bytes = Encoding.ASCII.GetBytes("ANYTHING");
        using var stream = new MemoryStream(bytes);
        
        _mockAssembly.Setup(asm => asm.GetManifestResourceStream("RESOURCE_01")).Returns(stream);
        
        var resource = new AssemblyResource("RESOURCE_00", _mockAssembly.Object);
        var text = resource.GetResourceText();
        
        Assert.Null(text);
    }

    [Fact]
    public void GetResourceText_ResourceNameExists_ReturnsText()
    {
        var bytes = Encoding.ASCII.GetBytes("RESOURCE_TEXT");
        using var stream = new MemoryStream(bytes);
        
        _mockAssembly.Setup(asm => asm.GetManifestResourceStream("RESOURCE_01")).Returns(stream);
        
        var resource = new AssemblyResource("RESOURCE_01", _mockAssembly.Object);
        var text = resource.GetResourceText();

        Assert.NotNull(text);
        Assert.Equal("RESOURCE_TEXT", text);
    }
    
    [Fact]
    public void HasResource_Exists_ReturnsTrue()
    {
        _mockAssembly.Setup(asm => asm.GetManifestResourceNames()).Returns(new string[] { "RESOURCE_01", "RESOURCE_02" });
        
        var resource = new AssemblyResource("RESOURCE_01", _mockAssembly.Object);
        var exists = resource.ResourceExists();
        
        Assert.True(exists);
    }

    [Fact]
    public void HasResource_DoesNotExist_ReturnsFalse()
    {
        _mockAssembly.Setup(asm => asm.GetManifestResourceNames()).Returns(new string[] { "RESOURCE_01", "RESOURCE_02" });
        
        var resource = new AssemblyResource("RESOURCE_00", _mockAssembly.Object);
        var exists = resource.ResourceExists();
        
        Assert.False(exists);
    }
}