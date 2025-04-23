using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.ServiceBus.Senders;

public interface ICustomerEventSender
{
    Task CreateAndSendMessage(CustomerEvent eventDetails,
        RequestAction requestAction,
        DateTime sourceLastUpdatedDateUtc);
}