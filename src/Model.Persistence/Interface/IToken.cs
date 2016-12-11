namespace Model.Persistence.Interface
{
    using System;

    using Model.Persistence.Enum;

    public interface IToken
    {
        #region Public Properties

        DateTimeOffset? Created { get; set; }

        Guid? TokenGuid { get; set; }

        string TokenName { get; }

        TokenType TokenType { get; }

        #endregion

        #region Public Methods and Operators

        Guid GetTokenGuid();

        #endregion
    }
}