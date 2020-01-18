using System;
using System.Collections.Generic;

namespace BeatSaverSharp.Extensions
{
    /// <summary>
    /// User class extensions
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Get an async iterator for beatmaps from this user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> BeatmapsIterator(this User user, uint page = 0)
        {
            while (true)
            {
                var hot = await user.Beatmaps(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }
    }
}
