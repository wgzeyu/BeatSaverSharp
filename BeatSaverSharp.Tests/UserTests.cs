using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BeatSaverSharp.Tests.Utils;

namespace BeatSaverSharp.Tests
{
    [TestClass]
    public class UserTests
    {
        #region Equality Tests
        [TestMethod]
        public async Task UserEquality()
        {
            var a = await Client.User("5cff0b7398cc5a672c84efe4");
            var b = await Client.User("5cff0b7398cc5a672c84efe4");
            var c = await Client.User("5cff0b7598cc5a672c8531c7");

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, c);
            Assert.AreNotEqual(b, c);
        }
        #endregion
    }
}
