namespace FeedReader.Class
{
    using System.Security.Principal;

    using Model.Persistence.Interface;

    public class Identity : IIdentity
    {
        #region Constructors and Destructors

        public Identity()
        {
        }

        public Identity(IToken token, IUser user)
        {
            this.AuthenticationType = null;
            this.IsAuthenticated = true;
            this.Name = user.UserName;
            this.User = user;
            this.Token = token;
        }

        #endregion

        #region Public Properties

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public string Name { get; private set; }

        public IToken Token { get; private set; }

        public IUser User { get; private set; }

        #endregion
    }
}