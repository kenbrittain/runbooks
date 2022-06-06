// See https://aka.ms/new-console-template for more information

using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Cli;
using Spectre.Console;

using Runbook;
using Runbook.Commands;

static class Program
{
    public static int Main(string[] args)
    {
        var services = new ServiceCollection();

        ConfigureLogging(services);
        RegisterServices(services);

        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName(Messages.ApplicationName);
            ConfigureCommands(config);
#if DEBUG
            config.PropagateExceptions();
            config.ValidateExamples();
#endif
        });

        return app.Run(args);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAnsiConsole>(AnsiConsole.Console);
        services.AddSingleton<IFileSystem>(new FileSystem());
    }

    private static void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(config => config.AddConsole());
    }

    private static void ConfigureCommands(IConfigurator config)
    {
        var createManifest = config.AddCommand<CreateManifestCommand>(Messages.CreateManifestCommandText);
        createManifest.WithDescription(Messages.CreateManifestDescriptionText);

        var showManifest = config.AddCommand<ShowManifestCommand>(Messages.ShowManifestCommandText);
        showManifest.WithDescription(Messages.ShowManifestDescriptionText);

        var exportView = config.AddCommand<ExportRunbookCommand>(Messages.ExportRunbookCommandText);
        exportView.WithDescription(Messages.ExportRunbookDescriptionText);
    }
}