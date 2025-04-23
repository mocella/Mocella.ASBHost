using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.RequestObjects;

public class MocellaRequest
{
    public string Action { get; set; }
    public DateTime RequestDateUtc { get; set; }
    public DateTime SourceLastUpdatedDateUtc { get; set; }
    public string SourceSystem { get; set; } = null!;

    public MocellaRequest()
    {
        Action = RequestAction.Add.ToString();
    }

    public MocellaRequest(RequestAction requestAction, 
        DateTime requestDateUtc, 
        DateTime sourceLastUpdatedDateUtc,
        string sourceSystem)
    {
        Action = requestAction.ToString();
        RequestDateUtc = requestDateUtc;
        SourceLastUpdatedDateUtc = sourceLastUpdatedDateUtc;
        SourceSystem = sourceSystem;
    }
}