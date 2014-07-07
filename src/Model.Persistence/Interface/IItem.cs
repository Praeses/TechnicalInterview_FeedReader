namespace Model.Persistence.Interface
{
    using System;

    public interface IItem
    {
        #region Public Properties

        string Description { get; }

        Guid? ItemGuid { get; set; }

        Uri Link { get; }

        int Sequence { get; set; }

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        Guid GetItemGuid();

        #endregion
    }
}