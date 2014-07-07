namespace Test.Shared.Extension
{
    using System;
    using System.Collections.Generic;

    using global::Shared.Extension;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ExtensionExceptionTest
    {
        #region Public Methods and Operators

        [Test]
        public void AddDumpObject_Lambda_Successful()
        {
            // ReSharper disable once ConvertToConstant.Local
            string dumpObject = "test";
            var exception = new Exception();
            exception.AddDumpObject(() => dumpObject);

            IDictionary<string, object> dumpObjects = exception.GetDumpObjects();
            object getDumpObject = dumpObjects["dumpObject"];
            Assert.AreEqual(dumpObjects.Keys.Count, 1);
            Assert.NotNull(getDumpObject);
            Assert.AreEqual(getDumpObject, dumpObject);
        }

        [Test]
        public void AddDumpObject_Parameter_Successful()
        {
            // ReSharper disable once ConvertToConstant.Local
            string dumpObject = "test";
            var exception = new Exception();
            exception.AddDumpObject("dumpObject", dumpObject);

            IDictionary<string, object> dumpObjects = exception.GetDumpObjects();
            object getDumpObject = dumpObjects["dumpObject"];
            Assert.AreEqual(dumpObjects.Keys.Count, 1);
            Assert.NotNull(getDumpObject);
            Assert.AreEqual(getDumpObject, dumpObject);
        }

        [Test]
        public void AddDumpObject_Type_Successful()
        {
            Type dumpObject = typeof(string);
            var exception = new Exception();
            exception.AddDumpObject(() => dumpObject);

            IDictionary<string, object> dumpObjects = exception.GetDumpObjects();
            object getDumpObject = dumpObjects["dumpObject"];
            Assert.AreEqual(dumpObjects.Keys.Count, 1);
            Assert.NotNull(getDumpObject);
            Assert.AreEqual(getDumpObject, dumpObject.FullName);
        }

        [Test]
        public void GetDumpObject_Multiple_Successful()
        {
            // ReSharper disable once ConvertToConstant.Local
            string dumpObject = "test";
            // ReSharper disable once ConvertToConstant.Local
            string dumpObject2 = "test2";
            var exception = new Exception();
            exception.AddDumpObject(() => dumpObject);
            exception.AddDumpObject(() => dumpObject2);

            IDictionary<string, object> dumpObjects = exception.GetDumpObjects();
            Assert.AreEqual(dumpObjects.Keys.Count, 2);
            Assert.AreEqual(dumpObjects["dumpObject"], dumpObject);
            Assert.AreEqual(dumpObjects["dumpObject2"], dumpObject2);
        }

        #endregion
    }
}