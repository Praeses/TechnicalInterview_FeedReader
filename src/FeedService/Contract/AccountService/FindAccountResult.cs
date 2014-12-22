using FeedService.Model;
using System.Runtime.Serialization;

namespace FeedService.Contract.AccountService
{
    [DataContract]
    public class FindAccountResult : Result
    {
        [DataMember]
        public Account Account { get; set; }
    }
}