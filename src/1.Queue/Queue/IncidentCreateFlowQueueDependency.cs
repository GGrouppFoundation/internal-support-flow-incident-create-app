using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

using IIncidentCreateQueueItemHandler = IQueueItemHandler<FlowMessage<IncidentJsonCreateFlowIn>, FlowActivity>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateFlowIn, Result<IncidentCreateFlowOut, Failure<IncidentCreateFlowFailureCode>>>;

public static class IncidentCreateFlowQueueDependency
{
    public static Dependency<IIncidentCreateQueueItemHandler> UseIncidentCreateFlowQueue(this Dependency<IIncidentCreateFunc> dependency)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.With(GetLoggerFactory).Fold<IIncidentCreateQueueItemHandler>(IncidentCreateQueueItemHandler.Create);
    }

    private static ILoggerFactory? GetLoggerFactory(this IServiceProvider serviceProvider)
        =>
        serviceProvider.GetServiceOrAbsent<ILoggerFactory>().OrDefault();
}