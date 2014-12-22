using System.Runtime.Serialization;

namespace FeedService.Contract.AccountService
{
    [DataContract]
    public class AddOptionResult : Result
    {
        public int ServiceOptionId { get; set; }
    }
}