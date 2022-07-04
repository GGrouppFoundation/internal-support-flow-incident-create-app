using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

partial class IncidentCreateFlowFunc
{
    public ValueTask<Result<IncidentCreateFlowOut, Failure<IncidentCreateFlowFailureCode>>> InvokeAsync(
        IncidentCreateFlowIn input, CancellationToken cancellationToken = default)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new IncidentCreateIn(
                ownerId: @in.OwnerId,
                customerId: @in.CustomerId,
                contactId: @in.ContactId,
                title: @in.Title,
                description: @in.Description,
                caseTypeCode: @in.CaseTypeCode,
                priorityCode: @in.PriorityCode,
                callerUserId: @in.CallerUserId))
        .PipeValue(
            incidentCreateFunc.InvokeAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
           incident => new IncidentCreateFlowOut(
                title: incident.Title,
                url: string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, incident.Id)));

    private static IncidentCreateFlowFailureCode MapFailureCode(IncidentCreateFailureCode failureCode)
        =>
        failureCode switch
        {
            IncidentCreateFailureCode.NotFound => IncidentCreateFlowFailureCode.NotFound,
            IncidentCreateFailureCode.NotAllowed => IncidentCreateFlowFailureCode.NotAllowed,
            IncidentCreateFailureCode.TooManyRequests => IncidentCreateFlowFailureCode.TooManyRequests,
            _ => default
        };
}