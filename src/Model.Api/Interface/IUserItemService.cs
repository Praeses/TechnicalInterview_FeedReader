namespace Model.Api.Interface
{
    using System;

    public interface IUserItemService
    {
        #region Public Methods and Operators

        void PutUserItem(Guid userGuid, Guid itemGuid, bool read);

        #endregion
    }
}