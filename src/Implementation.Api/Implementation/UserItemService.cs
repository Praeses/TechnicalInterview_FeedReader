namespace Implementation.Api.Implementation
{
    using System;

    using Model.Api.Interface;
    using Model.Persistence.Extension;
    using Model.Persistence.Interface;

    public class UserItemService : IUserItemService
    {
        #region Fields

        private readonly IUserService userService;

        #endregion

        #region Constructors and Destructors

        public UserItemService(IUserService userService)
        {
            this.userService = userService;
        }

        #endregion

        #region Public Methods and Operators

        public void PutUserItem(Guid userGuid, Guid itemGuid, bool read)
        {
            this.userService.PutUserItem(userGuid, itemGuid, read);
        }

        #endregion
    }
}