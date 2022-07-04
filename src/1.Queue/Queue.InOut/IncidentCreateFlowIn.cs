using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

public readonly record struct IncidentJsonCreateFlowIn
{
    [JsonPropertyName("ownerId")]
    public Guid OwnerId { get; init; }

    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; init; }

    [JsonPropertyName("contactId")]
    public Guid? ContactId { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("caseTypeCode")]
    public IncidentCaseTypeCode CaseTypeCode { get; init; }

    [JsonPropertyName("priorityCode")]
    public IncidentPriorityCode PriorityCode { get; init; }

    [JsonPropertyName("callerUserId")]
    public Guid? CallerUserId { get; init; }
}