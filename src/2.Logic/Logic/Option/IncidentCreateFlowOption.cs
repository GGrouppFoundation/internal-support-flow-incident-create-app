using System;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

public sealed record class IncidentCreateFlowOption
{
    public IncidentCreateFlowOption(string incidentCardUrlTemplate)
        =>
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();

    public string IncidentCardUrlTemplate { get; }
}