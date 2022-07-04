using System;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IIncidentCreateFlowFunc = IAsyncValueFunc<IncidentCreateFlowIn, Result<IncidentCreateFlowOut, Failure<IncidentCreateFlowFailureCode>>>;

internal sealed partial class IncidentCreateFlowFunc : IIncidentCreateFlowFunc
{
    public static IncidentCreateFlowFunc Create(IIncidentCreateFunc incidentCreateFunc, IncidentCreateFlowOption option)
    {
        _ = incidentCreateFunc ?? throw new ArgumentNullException(nameof(incidentCreateFunc));
        _ = option ?? throw new ArgumentNullException(nameof(option));

        return new(incidentCreateFunc, option);
    }

    private readonly IIncidentCreateFunc incidentCreateFunc;

    private readonly IncidentCreateFlowOption option;

    private IncidentCreateFlowFunc(IIncidentCreateFunc incidentCreateFunc, IncidentCreateFlowOption option)
    {
        this.incidentCreateFunc = incidentCreateFunc;
        this.option = option;
    }
}