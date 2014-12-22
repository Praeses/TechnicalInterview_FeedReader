using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FeedService.Contract;
using FeedService.Contract.AccountService;
using FeedService.Model;

namespace FeedService
{
    public class AccountService : IAccountService
    {
        public RegisterResult Register(Account newAccount)
        {
            var result = new RegisterResult();
            try
            {
                using (var context = new AccountEntities())
                {                    
                    context.Accounts.Add(newAccount);
                    context.SaveChanges();
                    result.AccountId = newAccount.AccountId;
                    result.Code = ResultCode.Success;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }
        public FindAccountResult FindAccount(int accountId)
        {
            var result = new FindAccountResult();

            try
            {
                using (var context = new AccountEntities())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    var account = context.Accounts.Where(tmp => tmp.AccountId == accountId).FirstOrDefault();
                    result.Account = account;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }

        public Result Deactivate(int accountId)
        {
            var result = new Result();
            try
            {
                using (var context = new AccountEntities())
                {
                    var account = context.Accounts.Where(tmp => tmp.AccountId == accountId).FirstOrDefault();
                    if (account != null)
                    {
                        context.Accounts.Remove(account);
                        context.SaveChanges();
                        result.Code = ResultCode.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }

        public AddOptionResult AddOption(ServiceOption option)
        {
            var result = new AddOptionResult();
            try
            {
                using (var context = new AccountEntities())
                {
                    context.ServiceOptions.Add(option);
                    context.SaveChanges();
                    result.ServiceOptionId = option.ServiceOptionId;
                    result.Code = ResultCode.Success;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;

        }

        public Result RemoveOption(int serviceOptionId)
        {
            var result = new Result();
            try
            {
                using (var context = new AccountEntities())
                {
                    var option = context.ServiceOptions.Where(tmp => tmp.ServiceOptionId == serviceOptionId).FirstOrDefault();
                    if (option != null)
                    {
                        context.ServiceOptions.Remove(option);
                        context.SaveChanges();
                        result.Code = ResultCode.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;

        }
    }
}
