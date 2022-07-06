using System;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Flow.Incident.Create;
internal static class AppHostBuilder
{
    private const string DataverseSectionName = "Dataverse";

    internal static IHostBuilder ConfigureIncidentCreateQueueProcessor(this IHostBuilder hostBuilder)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("IncidentCreate"))
        .UseDataverseApiClient(
            DataverseSectionName)
        .UseIncidentCreateApi()
        .UseIncidentCreateFlowLogic(
            GetIncidentCreateFlowOption)
        .UseIncidentCreateFlowQueue()
        .ConfigureQueueProcessor(
            hostBuilder);

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