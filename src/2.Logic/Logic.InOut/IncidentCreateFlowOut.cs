using System;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

public sealed record class IncidentCreateFlowOut
{
    public IncidentCreateFlowOut(string title, string url)
    {
        Title = title.OrEmpty();
        Url = url.OrEmpty();
    }

    public string Title { get; }

    public string Url { get; }
}