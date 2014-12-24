using System.Runtime.Serialization;

namespace FeedService.Contract.AccountService
{
    [DataContract]
    public class RegisterResult : Result
    {
        [DataMember]
        public int AccountId { get; set; }
    }
}