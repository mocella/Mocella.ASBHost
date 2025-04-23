using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.ResponseObjects;

public class MocellaCustomerResponse : MocellaResponse
{
    public CustomerEvent? CustomerEvent { get; set; }
    
    public MocellaCustomerResponse(){}
    
    public MocellaCustomerResponse(CustomerEvent? customerEvent,
        RequestAction requestAction,
        DateTime requestDateUtc,
        DateTime requestSourceLastUpdatedDateUtc,
        DateTime responseDateUtc,
        string sourceSystem) : base(requestAction, requestDateUtc, requestSourceLastUpdatedDateUtc, responseDateUtc, sourceSystem)
    {
        CustomerEvent = customerEvent;
    }
}