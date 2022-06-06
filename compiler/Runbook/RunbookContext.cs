using Spectre.Console;

namespace Runbook;

/// <summary>
/// Defines the methods and resources available to a runbook while executing.
/// </summary>
public interface IRunbookContext
{
    /// <summary>
    /// Get the type attribute from the context.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    /// <returns>The object identified by the attribute name.</returns>
    /// <exception cref="InvalidCastException">If the type cannot be converted to the desired type.</exception>
    T Get<T>(string name);

    /// <summary>
    /// Set the attribute in the context.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    /// <param name="value">The value of the attribute.</param>
    void Set<T>(string name, T @value);

    /// <summary>
    /// Exposes the context as a dictionary for template rendering.
    /// This method provides a clone of the context so changes that are made
    /// do not affect the providing context.
    /// </summary>
    /// <returns>A clone of the context values.</returns>
    IDictionary<string, object> ToDictionary();

    /// <summary>
    /// Gets the raw object for the named attribute.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns><c>true</c> if the attribute was returned; otherwise <c>false</c>.</returns>
    bool TryGet(string name, out object? @value);
}

/// <summary>
/// Responsible for handling properties associated with a runbook. 
/// </summary>
public class RunbookContext : IRunbookContext
{
    private Dictionary<string, object> _values;

    /// <summary>
    /// Creates an empty context bound to the default console.
    /// </summary>
    public RunbookContext()
    {
        _values = new Dictionary<string, object>();
    }

    /// <summary>
    /// Enumerate the names in the context.
    /// </summary>
    public IEnumerable<string> Names => _values.Keys;

    /// <summary>
    /// Add or replace an attribute. If the <c>value</c> is <c>null</c> then
    /// the attribute is removed from the context.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="value">Value of the property</param>
    /// <exception cref="ArgumentNullException">Thrown when <c>name</c> is <c>null</c>.</exception>
    public void Set<T>(string name, T @value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (@value == null)
        {
            _values.Remove(name);
        }
        else
        {
            _values[name] = @value;
        }
    }

    /// <summary>
    /// Get the property for the name.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Thrown when <c>name</c> does not exist</exception>
    public T Get<T>(string name)
    {
        var value = _values[name];
        return (T)value;
    }

    /// <summary>
    /// Gets the property for the name.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="value">Value of the property</param>
    /// <returns><c>true</c> of the property was found; otherwise <c>false</c></returns>
    public bool TryGet(string name, out object? value)
    {
        var found = _values.Keys.Contains(name);
        if (found)
        {
            value = _values[name];
            return true;
        }
        
        value = null;
        return false;
    }

    public IDictionary<string, object> ToDictionary()
    {
        var clone = new Dictionary<string, object>();
        foreach (var keyValuePair in _values)
        {
            clone.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return clone;
    }
}