using System.ServiceModel;
using System.Threading.Tasks;

namespace FeedService.Contract.ContentService
{
    [ServiceContract]
    public interface IContentService
    {
        [OperationContract]
        LoadFeedResult LoadItemFeed(LoadFeedRequest request);
        [OperationContract]
        Task<Result> SynchronizeSubscriptions(int accountId);
    }
}
