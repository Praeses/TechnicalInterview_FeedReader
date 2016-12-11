namespace Model.Api.Interface
{
    using System;

    using Model.Persistence.Interface;

    public interface IRegistrationService
    {
        #region Public Methods and Operators

        IUser Register(string userName, string password, string tokenName, out IToken token);

        void Unregister(Guid userGuid);

        void UserNameExists(string userName);

        #endregion
    }
}