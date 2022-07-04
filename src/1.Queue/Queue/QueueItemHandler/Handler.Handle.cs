using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

partial class IncidentCreateQueueItemHandler
{
    public ValueTask<Result<QueueItemOut<FlowActivity>, QueueItemFailure>> HandleAsync(
        QueueItemIn<FlowMessage<IncidentJsonCreateFlowIn>> input, CancellationToken cancellationToken = default)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => @in.Message.Message)
        .Pipe(
            static message => new IncidentCreateFlowIn(
                ownerId: message.OwnerId,
                customerId: message.CustomerId,
                contactId: message.ContactId,
                title: message.Title.OrEmpty(),
                description: message.Description,
                caseTypeCode: message.CaseTypeCode,
                priorityCode: message.PriorityCode,
                callerUserId: message.CallerUserId))
        .PipeValue(
            incidentCreateFunc.InvokeAsync)
        .MapSuccess(
            CreateSuccessAttachment)
        .MapSuccess(
            input.Message.CreateAttachmentActivity)
        .Recover(
            failure => ProcessFailure(input, failure))
        .MapSuccess(
            static activity => new QueueItemOut<FlowActivity>(activity));

    private static FlowAttachment CreateSuccessAttachment(IncidentCreateFlowOut incident)
        =>
        new FlowHeroCard
        {
            Title = "Инцидент был создан успешно",
            Buttons = new FlowCardAction[]
            {
                new(FlowCardActionType.OpenUrl)
                {
                    Title = incident.Title,
                    Value = incident.Url
                }
            }
        };

    private Result<FlowActivity, QueueItemFailure> ProcessFailure(
        QueueItemIn<FlowMessage<IncidentJsonCreateFlowIn>> input, Failure<IncidentCreateFlowFailureCode> failure)
    {
        var retry = (input.MaxRetryCount - input.RetryCount) > 0;
        if (retry && failure.FailureCode is not IncidentCreateFlowFailureCode.NotAllowed)
        {
            return new QueueItemFailure(failure.FailureMessage, returnToQueue: true);
        }

        logger?.LogError("An error occured when queue item {messageId} was been processed: {errorMessage}", input.Id, failure.FailureMessage);

        return failure.FailureCode switch
        {
            IncidentCreateFlowFailureCode.NotAllowed => input.Message.CreateTextActivity(NotAllowedFailureMessage),
            _ => input.Message.CreateTextActivity(UnexpectedFailureMessage)
        };
    }
}