using Microsoft.Extensions.DependencyInjection;
using Spectre.Cli;

namespace Runbook;

public sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;
 
    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }
 
    public object? Resolve(Type? type)
    {
        return _provider.GetRequiredService(type!);
    }
}

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;
 
    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }
 
    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.BuildServiceProvider());
    }
 
    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }
 
    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }
}