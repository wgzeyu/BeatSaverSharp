using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BeatSaverSharp.Tests.Utils;

[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.MethodLevel)]

namespace BeatSaverSharp.Tests
{
    [TestClass]
    public class BeatSaverTests
    {
        #region Constructor Tests
        [TestMethod]
        public void DefaultOptions()
        {
            _ = new BeatSaver();

            var options = new HttpOptions();
            _ = new BeatSaver(options);
        }

        [TestMethod]
        public void ApplicationNameAndVersion()
        {
            var options = new HttpOptions()
            {
                ApplicationName = "TestApp",
                Version = new Version(1, 0),
            };

            _ = new BeatSaver(options);
        }

        [TestMethod]
        public void ApplicationNameWithoutVersion()
        {
            var options = new HttpOptions()
            {
                ApplicationName = "TestApp",
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _ = new BeatSaver(options);
            });
        }
        #endregion

        #region Method Tests
        #region Key
        [TestMethod]
        public async Task ValidKey()
        {
            var map = await Client.Key("17f9");
            CheckTestMap(map);
        }

        [TestMethod]
        public async Task InvalidKey()
        {
            var map = await Client.Key("map");
            Assert.IsNull(map);
        }

        [TestMethod]
        public async Task NullKey()
        {
            var map = await Client.Key(null);
            Assert.IsNull(map);
        }
        #endregion

        #region Hash
        [TestMethod]
        public async Task ValidHash()
        {
            var map = await Client.Hash("108c239db3c0596f1ba7426353af1b4cc4fd8b08");
            CheckTestMap(map);
        }

        [TestMethod]
        public async Task InvalidHash()
        {
            var map = await Client.Hash("map");
            Assert.IsNull(map);
        }

        [TestMethod]
        public async Task NullHash()
        {
            var map = await Client.Hash(null);
            Assert.IsNull(map);
        }
        #endregion
        #endregion
    }
}
