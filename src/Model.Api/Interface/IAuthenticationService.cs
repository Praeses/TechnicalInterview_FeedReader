namespace Model.Api.Interface
{
    using System;

    using Model.Persistence.Interface;

    public interface IAuthenticationService
    {
        #region Public Methods and Operators

        IUser AuthenticateRequest(Guid tokenGuid, out IToken token);

        IUser Login(string userName, string password, string tokenName, out IToken token);

        void Logout(Guid tokenGuid);

        #endregion
    }
}