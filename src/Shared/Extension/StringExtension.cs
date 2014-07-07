namespace Shared.Extension
{
    public static class StringExtension
    {
        #region Public Methods and Operators

        public static string FormatWith(this string @this, params object[] args)
        {
            return string.Format(@this, args);
        }

        public static string ToFirstCharacterLower(this string @this)
        {
            return (string.IsNullOrEmpty(@this) || char.IsLower(@this, 0))
                ? @this
                : char.ToLowerInvariant(@this[0]) + @this.Substring(1);
        }

        public static string ToFirstCharacterUpper(this string @this)
        {
            return (string.IsNullOrEmpty(@this) || char.IsUpper(@this, 0))
                ? @this
                : char.ToUpperInvariant(@this[0]) + @this.Substring(1);
        }

        #endregion
    }
}