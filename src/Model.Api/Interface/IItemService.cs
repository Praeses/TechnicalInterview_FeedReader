namespace Model.Api.Interface
{
    using System;
    using System.Collections.Generic;

    using Model.Persistence.Interface;

    public interface IItemService
    {
        #region Public Methods and Operators

        IEnumerable<IItem> EnumerateItemsAfter(Guid channelGuid, int limit, Guid? itemGuid);

        #endregion
    }
}