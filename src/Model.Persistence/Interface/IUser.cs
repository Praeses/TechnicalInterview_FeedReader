namespace Model.Persistence.Interface
{
    using System;

    public interface IUser
    {
        #region Public Properties

        string HashedPassword { get; }

        Guid? UserGuid { get; set; }

        string UserName { get; }

        #endregion

        #region Public Methods and Operators

        Guid GetUserGuid();

        bool IsPasswordValid(string password);

        IUser SetPassword(string password);

        #endregion
    }
}