namespace Model.Persistence.Class
{
    using System;

    using Model.Persistence.Enum;
    using Model.Persistence.Interface;

    public class Token : IToken
    {
        #region Constructors and Destructors

        public Token(string tokenName, TokenType tokenType)
        {
            this.TokenName = tokenName;
            this.TokenType = tokenType;
        }

        public Token(DateTimeOffset created, Guid tokenGuid, string tokenName, TokenType tokenType)
        {
            this.Created = created;
            this.TokenGuid = tokenGuid;
            this.TokenName = tokenName;
            this.TokenType = tokenType;
        }

        #endregion

        #region Public Properties

        public DateTimeOffset? Created { get; set; }

        public Guid? TokenGuid { get; set; }

        public string TokenName { get; private set; }

        public TokenType TokenType { get; private set; }

        #endregion

        #region Public Methods and Operators

        public Guid GetTokenGuid()
        {
            if (this.TokenGuid.HasValue)
            {
                return this.TokenGuid.Value;
            }

            throw new NullReferenceException();
        }

        #endregion
    }
}