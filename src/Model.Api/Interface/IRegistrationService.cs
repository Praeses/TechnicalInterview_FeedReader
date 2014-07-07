namespace Model.Api.Interface
{
    using System;

    using Model.Persistence.Interface;

    public interface IRegistrationService
    {
        #region Public Methods and Operators

        IUser RegisterUser(string userName, string password, string tokenName, out IToken token);

        void UnregisterUser(Guid userGuid);

        void UserNameExists(string userName);

        #endregion
    }
}