using System;
using System.Threading;
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

        #region Internal Method Tests
        [TestMethod]
        public async Task WithCancellationToken()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            var map = await Client.Key("4c19", token);
            CheckOvercooked(map);
        }

        [TestMethod]
        public async Task WithCancellationTokenCancelled()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.Cancel();

            Beatmap map = null;
            try
            {
                map = await Client.Key("4c19", token: token);
            }
            catch (TaskCanceledException)
            {
                // Pass
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.IsNull(map);
        }

        [TestMethod]
        public async Task WithProgress()
        {
            int updates = 0;
            double prog = 0;
            Progress<double> progress = new Progress<double>(p =>
            {
                updates += 1;
                prog = p;

                Assert.IsTrue(p >= 0);
                Assert.IsTrue(p <= 1);
            });

            var map = await Client.Key("4c19", progress: progress);
            CheckOvercooked(map);

            Assert.IsTrue(updates > 0);
            Assert.IsTrue(prog == 1);
        }
        #endregion

        #region Method Tests
        #region Key
        [TestMethod]
        public async Task ValidKey()
        {
            var map = await Client.Key("17f9");
            CheckTowerOfHeaven(map);
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
            CheckTowerOfHeaven(map);
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
