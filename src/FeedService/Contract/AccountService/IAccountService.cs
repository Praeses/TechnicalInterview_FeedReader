using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FeedService.Model;

namespace FeedService.Contract.AccountService
{
    [ServiceContract]
    public interface IAccountService
    {
        [OperationContract]
        RegisterResult Register(Account newAccount);
        [OperationContract]
        FindAccountResult FindAccount(int accountId);
        [OperationContract]
        Result Deactivate(int accountId);
        [OperationContract]
        AddOptionResult AddOption(ServiceOption option);
        [OperationContract]
        Result RemoveOption(int serviceOptionId);
    }

}
