using System;
using System.Collections.Generic;

namespace BeatSaverSharp.Extensions
{
    /// <summary>
    /// BeatSaver class extensions
    /// </summary>
    public static class BeatSaverExtensions
    {
        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Latest
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> LatestIterator(this BeatSaver beatsaver, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Latest(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Heat
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> HotIterator(this BeatSaver beatsaver, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Hot(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Rating
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> RatingIterator(this BeatSaver beatsaver, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Rating(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Downloads
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> DownloadsIterator(this BeatSaver beatsaver, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Downloads(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Plays
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> PlaysIterator(this BeatSaver beatsaver, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Plays(page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, using a text search
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> SearchIterator(this BeatSaver beatsaver, string query, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.Search(query, page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }

        /// <summary>
        /// Get an async iterator for beatmaps, using an advanced search
        /// </summary>
        /// <param name="beatsaver"></param>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Beatmap> SearchAdvancedIterator(this BeatSaver beatsaver, string query, uint page = 0)
        {
            while (true)
            {
                var hot = await beatsaver.SearchAdvanced(query, page);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }
    }
}
