using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

using IIncidentCreateQueueItemHandler = IQueueItemHandler<FlowMessage<IncidentJsonCreateFlowIn>, FlowActivity>;

internal static class AppHostBuilder
{
    private const string DataverseSectionName = "Dataverse";

    internal static IHostBuilder ConfigureIncidentCreateQueueProcessor(this IHostBuilder hostBuilder)
        =>
        IsServiceBusUsed() switch
        {
            true    => UseIncidentCreateQueueItemHandler().ConfigureBusQueueProcessor(hostBuilder),
            _       => UseIncidentCreateQueueItemHandler().ConfigureQueueProcessor(hostBuilder)
        };

    private static bool IsServiceBusUsed()
        =>
        new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build()
        .GetValue("Feature:IsServiceBusUsed", false);

    private static Dependency<IIncidentCreateQueueItemHandler> UseIncidentCreateQueueItemHandler()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("IncidentCreate"))
        .UseDataverseApiClient(
            DataverseSectionName)
        .UseIncidentCreateApi()
        .UseIncidentCreateFlowLogic(
            GetIncidentCreateFlowOption)
        .UseIncidentCreateFlowQueue();

    private static IncidentCreateFlowOption GetIncidentCreateFlowOption(this IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var baseUri = new Uri(configuration.GetDataverseApiClientOption(DataverseSectionName).ServiceUrl);
        var template = configuration.GetRequiredSection(DataverseSectionName)["IncidentCardRelativeUrlTemplate"];

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;

        return new(
            incidentCardUrlTemplate: uri.ReplaceInvariant("%7B", "{").ReplaceInvariant("%7D", "}"));
    }

    private static string ReplaceInvariant(this string source, string oldValue, string newValue)
        =>
        source.Replace(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
}