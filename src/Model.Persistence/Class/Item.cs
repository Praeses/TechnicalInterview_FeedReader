namespace Model.Persistence.Class
{
    using System;

    using Model.Persistence.Interface;

    public class Item : IItem
    {
        #region Constructors and Destructors

        public Item(string description, Uri link, string title)
        {
            this.Description = description;
            this.Link = link;
            this.Title = title;
        }

        public Item(Guid itemGuid, string description, Uri link, int sequence, string title)
        {
            this.Description = description;
            this.ItemGuid = itemGuid;
            this.Link = link;
            this.Sequence = sequence;
            this.Title = title;
        }

        #endregion

        #region Public Properties

        public string Description { get; private set; }

        public Guid? ItemGuid { get; set; }

        public Uri Link { get; private set; }

        public int Sequence { get; set; }

        public string Title { get; private set; }

        #endregion

        #region Public Methods and Operators

        public Guid GetItemGuid()
        {
            if (this.ItemGuid.HasValue)
            {
                return this.ItemGuid.Value;
            }

            throw new NullReferenceException();
        }

        #endregion
    }
}