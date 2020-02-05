using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeatSaverSharp.Tests
{
    internal class Utils
    {
        private static readonly HttpOptions Options = new HttpOptions()
        {
            ApplicationName = "BeatSaverSharpTests",
            Version = new Version(1, 0, 0),
        };

        internal static readonly BeatSaver Client = new BeatSaver(Options);

        internal static void CheckTestMap(Beatmap map)
        {
            Assert.AreEqual(map.ID, "5cff621048229f7d88fc724c");
            Assert.AreEqual(map.Key, "17f9");
            Assert.AreEqual(map.Hash, "108c239db3c0596f1ba7426353af1b4cc4fd8b08");

            Assert.AreEqual(map.Uploader.ID, "5cff0b7498cc5a672c850786");
            Assert.AreEqual(map.Uploader.Username, "zorowo");
        }
    }
}
