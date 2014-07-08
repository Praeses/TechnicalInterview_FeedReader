namespace Model.Persistence.Class
{
    using System;

    using Model.Persistence.Interface;

    public class UserItem : Item, IUserItem
    {
        #region Constructors and Destructors

        public UserItem(string description, Uri link, string title)
            : base(description, link, title)
        {
        }

        public UserItem(Guid itemGuid, string description, Uri link, int sequence, string title, bool read)
            : base(itemGuid, description, link, sequence, title)
        {
            this.Read = read;
        }

        #endregion

        #region Public Properties

        public bool Read { get; set; }

        #endregion
    }
}