namespace Model.Persistence.Interface
{
    public interface IUserItem : IItem
    {
        #region Public Properties

        bool Read { get; set; }

        #endregion
    }
}