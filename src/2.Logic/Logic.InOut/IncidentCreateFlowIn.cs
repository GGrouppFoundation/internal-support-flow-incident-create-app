using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

public sealed record class IncidentCreateFlowIn
{
    private readonly string? title, description;

    public IncidentCreateFlowIn(
        Guid ownerId,
        Guid customerId,
        Guid? contactId,
        string title,
        [AllowNull] string description,
        IncidentCaseTypeCode caseTypeCode,
        IncidentPriorityCode priorityCode,
        Guid? callerUserId)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        ContactId = contactId;
        this.title = title.OrNullIfEmpty();
        this.description = description.OrNullIfEmpty();
        CaseTypeCode = caseTypeCode;
        PriorityCode = priorityCode;
        CallerUserId = callerUserId;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public Guid? ContactId { get; }

    public string Title => title.OrEmpty();

    public string Description => description.OrEmpty();

    public IncidentCaseTypeCode CaseTypeCode { get; }

    public IncidentPriorityCode PriorityCode { get; }

    public Guid? CallerUserId { get; }
}