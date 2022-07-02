using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Runbook;

/// <summary>
/// Defines a step in a runbook.
/// </summary>
public class RunbookStep
{
    public RunbookStep()
    {
        Name = string.Empty;
        Summary = string.Empty;
        Note = string.Empty;
    }
    
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Note { get; set; }
}

/// <summary>
/// Data model for the runbook. 
/// </summary>
public class Runbook
{
    private List<RunbookStep> _steps;
    
    /// <summary>
    /// Constructor used for Yaml deserialization.
    /// </summary>
    public Runbook()
    {
        _steps = new List<RunbookStep>();
        Title = string.Empty;
        Summary = string.Empty;
        Note = string.Empty;
        Author = string.Empty;
        Contact = string.Empty;
    }

    /// <summary>
    /// Single line of text used to describe this runbook.
    /// </summary>
    [Required]
    [YamlMember(Alias = "title")]
    public string Title { get; set; }
    
    /// <summary>
    /// Tells what does this runbook accomplishes.
    /// </summary>
    [Required]
    [YamlMember(Alias = "summary")]
    public string Summary { get; set; }
    
    [YamlMember(Alias = "note")]
    public string Note { get; set; }

    [Required]
    [YamlMember(Alias = "author")]
    public string Author { get; set; }

    [Required]
    [YamlMember(Alias = "contact")]
    public string Contact { get; set; }

    [YamlMember(Alias = "steps")]
    public List<RunbookStep> Steps
    {
        get { return _steps; }
        set { _steps = value; }
    }

    /// <summary>
    /// Append the runbook with a step to perform.
    /// </summary>
    /// <param name="step">Activity to perform</param>
    /// <exception cref="ArgumentNullException">Thrown when the step is null</exception>
    public void AddStep(RunbookStep step)
    {
        if (step == null)
        {
            throw new ArgumentNullException(nameof(step));
        }
        _steps.Add(step);
    }

    /// <summary>
    /// Determine if the model is valid. This require the <c>Title</c>,
    /// <c>Summary</c>, <c>Author</c>, and <c>Context</c> to contain non-null
    /// and non-empty values.
    /// </summary>
    /// <returns><c>true</c> fi the mode is valid; otherwise <c>false</c></returns>
    public bool IsValid()
    {
        var titleOk = !string.IsNullOrWhiteSpace(Title);
        var summaryOk = !string.IsNullOrWhiteSpace(Summary);
        var authorOk = !string.IsNullOrWhiteSpace(Author);
        var contactOk = !string.IsNullOrWhiteSpace(Contact);

        return titleOk && summaryOk && authorOk && contactOk;
    }

    public static Runbook Load(string path)
    {
        using var yamlReader = new StreamReader(path);
        var newManifest = Load(yamlReader);
        return newManifest;
    }

    public static Runbook Load(TextReader yaml)
    {
        var yamlText = yaml.ReadToEnd();
        var newManifest = new Runbook();
        newManifest.LoadYaml(yamlText);
        return newManifest;
    }
    
    public void LoadYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var runbookBuilder = deserializer.Deserialize<RunbookBuilder>(yaml);
        runbookBuilder.Build(this);
    }

    public void Save(string path)
    {
        using var yamlWriter = new StreamWriter(path);
        Save(yamlWriter);
    }

    public void Save(TextWriter writer)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithIndentedSequences()
            .Build();
        var yaml = serializer.Serialize(this);
        writer.Write(yaml);    
    }
}

class RunbookBuilder
{
    public RunbookBuilder()
    {
        this.Title = string.Empty;
        this.Summary = string.Empty;
        this.Note = string.Empty;
        this.Author = string.Empty;
        this.Contact = string.Empty;
        this.Steps = new List<RunbookStep>();
    }
    
    [Required]
    [YamlMember(Alias = "title")]
    public string Title { get; set; }
    
    [Required]
    [YamlMember(Alias = "summary")]
    public string Summary { get; set; }
    
    [YamlMember(Alias = "note")]
    public string Note { get; set; }

    [Required]
    [YamlMember(Alias = "author")]
    public string Author { get; set; }

    [Required]
    [YamlMember(Alias = "contact")]
    public string Contact { get; set; }

    [YamlMember(Alias = "steps")]
    public List<RunbookStep> Steps { get; set; }

    public void Build(Runbook runbook)
    {
        runbook.Title = this.Title;
        runbook.Summary = this.Summary;
        runbook.Note = this.Note;
        runbook.Author = this.Author;
        runbook.Contact = this.Contact;

        foreach (var step in this.Steps)
        {
            runbook.AddStep(step);
        }
    }
}
