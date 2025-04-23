using System.Text.Json.Serialization;
using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.RequestObjects;

public class MocellaCustomerRequest : MocellaRequest
{
    [JsonConstructor]
    public MocellaCustomerRequest(){}
    
    public MocellaCustomerRequest(CustomerEvent customerEvent, 
        RequestAction requestAction, 
        DateTime requestDateUtc, 
        DateTime sourceLastUpdatedDateUtc,
        string sourceSystem) : base(requestAction, requestDateUtc, sourceLastUpdatedDateUtc, sourceSystem)
    {
        CustomerEvent = customerEvent;
    }

    public CustomerEvent CustomerEvent { get; init; } = null!;
}