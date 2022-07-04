using System;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IIncidentCreateFlowFunc = IAsyncValueFunc<IncidentCreateFlowIn, Result<IncidentCreateFlowOut, Failure<IncidentCreateFlowFailureCode>>>;

public static class IncidentCreateFlowDependency
{
    public static Dependency<IIncidentCreateFlowFunc> UseIncidentCreateFlowLogic(
        this Dependency<IIncidentCreateFunc> dependency, Func<IServiceProvider, IncidentCreateFlowOption> optionResolver)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));

        return dependency.With(optionResolver).Fold<IIncidentCreateFlowFunc>(IncidentCreateFlowFunc.Create);
    }
}