namespace Implementation.Api.Implementation
{
    using System;

    using Model.Api.Interface;
    using Model.Persistence.Class;
    using Model.Persistence.Enum;
    using Model.Persistence.Extension;
    using Model.Persistence.Interface;

    using Shared.Exceptions;

    public class RegistrationService : IRegistrationService
    {
        #region Fields

        private readonly IUserService userService;

        #endregion

        #region Constructors and Destructors

        public RegistrationService(IUserService userService)
        {
            this.userService = userService;
        }

        #endregion

        #region Public Methods and Operators

        public IUser RegisterUser(string userName, string password, string tokenName, out IToken token)
        {
            IUser user = this.userService.GetUser(userName);
            if (user == default(IUser))
            {
                user = new User(userName, string.Empty).SetPassword(password);
                this.userService.PutUser(user);
            }
            else if (!user.IsPasswordValid(password))
            {
                throw new ConflictException("User exists.  Password invalid.");
            }

            token = new Token(tokenName, TokenType.Authentication);
            this.userService.AddToken(user.GetUserGuid(), token);
            return user;
        }

        public void UnregisterUser(Guid userGuid)
        {
            this.userService.DeleteUser(userGuid);
        }

        public void UserNameExists(string userName)
        {
            if (this.userService.GetUser(userName) == default(IUser))
            {
                throw new NotFoundException();
            }
        }

        #endregion
    }
}