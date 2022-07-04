using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateFlowIn, Result<IncidentCreateFlowOut, Failure<IncidentCreateFlowFailureCode>>>;

internal sealed partial class IncidentCreateQueueItemHandler : IQueueItemHandler<FlowMessage<IncidentJsonCreateFlowIn>, FlowActivity>
{
    public static IncidentCreateQueueItemHandler Create(IIncidentCreateFunc incidentCreateFunc, ILoggerFactory? loggerFactory)
    {
        _ = incidentCreateFunc ?? throw new ArgumentNullException(nameof(incidentCreateFunc));

        return new(incidentCreateFunc, loggerFactory);
    }

    private const string NotAllowedFailureMessage
        =
        "Не удалось создать инцидент. Данная операция не разрешена для вашей учетной записи. Обратитесь к администратору";

    private const string UnexpectedFailureMessage
        =
        "При создании инцидента произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее";

    private readonly IIncidentCreateFunc incidentCreateFunc;

    private readonly ILogger? logger;

    private IncidentCreateQueueItemHandler(IIncidentCreateFunc incidentCreateFunc, ILoggerFactory? loggerFactory)
    {
        this.incidentCreateFunc = incidentCreateFunc;
        logger = loggerFactory?.CreateLogger<IncidentCreateQueueItemHandler>();
    }
}