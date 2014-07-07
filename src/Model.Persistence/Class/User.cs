namespace Model.Persistence.Class
{
    using System;

    using Model.Persistence.Interface;

    using Shared.Class;

    public class User : IUser
    {
        #region Constructors and Destructors

        public User(string userName, string hashedPassword)
        {
            this.HashedPassword = hashedPassword;
            this.UserName = userName;
        }

        public User(Guid userGuid, string userName, string hashedPassword)
        {
            this.HashedPassword = hashedPassword;
            this.UserGuid = userGuid;
            this.UserName = userName;
        }

        #endregion

        #region Public Properties

        public string HashedPassword { get; private set; }

        public Guid? UserGuid { get; set; }

        public string UserName { get; private set; }

        #endregion

        #region Public Methods and Operators

        public Guid GetUserGuid()
        {
            if (this.UserGuid.HasValue)
            {
                return this.UserGuid.Value;
            }

            throw new NullReferenceException();
        }

        public bool IsPasswordValid(string password)
        {
            return HashPassword.Verify(password, this.HashedPassword);
        }

        public IUser SetPassword(string password)
        {
            this.HashedPassword = HashPassword.Hash(password);
            return this;
        }

        #endregion
    }
}