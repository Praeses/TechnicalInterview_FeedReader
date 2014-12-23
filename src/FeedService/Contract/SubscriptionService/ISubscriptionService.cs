using System.ServiceModel;

namespace FeedService.Contract.SubscriptionService
{
    [ServiceContract]
    public interface ISubscriptionService
    {
        [OperationContract]
        LoadSubscriptionsResult LoadSubscriptions(int accountId);

        [OperationContract]
        Result Subscribe(NewSubscription subscription);

        [OperationContract]
        Result Unsubscribe(UnsubscribeRequest request);

        Result Search();
        Result Share();
        Result Notify();
    }
}
