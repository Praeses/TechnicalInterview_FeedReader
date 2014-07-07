namespace Test.Shared.Extension
{
    using global::Shared.Extension;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class StringExtensionTest
    {
        #region Public Methods and Operators

        [Test]
        public void FormatWith_String_Successful()
        {
            string str = "x{0}x".FormatWith("Y");
            Assert.AreEqual(str, "xYx");
        }

        [Test]
        public void ToFirstCharacterLower_Lower_Successful()
        {
            string str = "test".ToFirstCharacterLower();
            Assert.AreEqual(str, "test");
        }

        [Test]
        public void ToFirstCharacterLower_Upper_Successful()
        {
            string str = "Test".ToFirstCharacterLower();
            Assert.AreEqual(str, "test");
        }

        [Test]
        public void ToFirstCharacterUpper_Lower_Successful()
        {
            string str = "test".ToFirstCharacterUpper();
            Assert.AreEqual(str, "Test");
        }

        [Test]
        public void ToFirstCharacterUpper_Upper_Successful()
        {
            string str = "Test".ToFirstCharacterUpper();
            Assert.AreEqual(str, "Test");
        }

        #endregion
    }
}