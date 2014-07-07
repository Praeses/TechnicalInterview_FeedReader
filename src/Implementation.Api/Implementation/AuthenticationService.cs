namespace Implementation.Api.Implementation
{
    using System;

    using Model.Api.Interface;
    using Model.Persistence.Class;
    using Model.Persistence.Enum;
    using Model.Persistence.Extension;
    using Model.Persistence.Interface;

    using Shared.Exceptions;

    public class AuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IUserService userService;

        #endregion

        #region Constructors and Destructors

        public AuthenticationService(IUserService userService)
        {
            this.userService = userService;
        }

        #endregion

        #region Public Methods and Operators

        public IUser AuthenticateRequest(Guid tokenGuid, out IToken token)
        {
            return this.userService.GetUser(tokenGuid, out token);
        }

        public IUser Login(string userName, string password, string tokenName, out IToken token)
        {
            IUser user = this.userService.GetUser(userName);
            if (user == default(IUser))
            {
                throw new NotFoundException();
            }
            if (!user.IsPasswordValid(password))
            {
                throw new UnauthorizedException("Password invalid.");
            }

            token = new Token(tokenName, TokenType.Authentication);
            this.userService.AddToken(user.GetUserGuid(), token);
            return user;
        }

        public void Logout(Guid tokenGuid)
        {
            this.userService.RemoveToken(tokenGuid);
        }

        #endregion
    }
}