using FluentValidation.Results;
using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.ResponseObjects;

public class MocellaResponse
{
    public DateTime ResponseDateUtc { get; set; }
    public string RequestAction { get; set; }
    public DateTime RequestDateUtc { get; set; }
    public DateTime RequestSourceLastUpdatedDateUtc { get; set; }
    public string SourceSystem { get; set; } = null!;
    public List<ValidationFailure> Errors { get; set; } = [];
    public bool HasErrors => Errors.Any();
    
    public MocellaResponse()
    {
        RequestAction = Configuration.RequestAction.Add.ToString();
    }
    
    public MocellaResponse(RequestAction requestAction, 
        DateTime requestDateUtc,
        DateTime requestSourceLastUpdatedDateUtc,
        DateTime responseDateUtc,
        string sourceSystem)
    {
        RequestAction = requestAction.ToString();
        RequestDateUtc = requestDateUtc;
        ResponseDateUtc = responseDateUtc;
        RequestSourceLastUpdatedDateUtc = requestSourceLastUpdatedDateUtc;
        SourceSystem = sourceSystem;
    }
}