using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if NETSTANDARD2_1
using System.Collections.Generic;
#endif

namespace BeatSaverSharp
{
    /// <summary>
    /// Beat Saver API Methods
    /// </summary>
    public sealed class BeatSaver
    {
        /// <summary>
        /// BeatSaver Base URL
        /// </summary>
        public const string BaseURL = "https://beatsaver.wgzeyu.top";

        /// <summary>
        /// Default API Instance
        /// </summary>
        public static readonly BeatSaver Client = new BeatSaver();

        #region Constructors
        /// <summary>
        /// Instantiate a new BeatSaver API Client
        /// </summary>
        /// <param name="options">HTTP Options</param>
        public BeatSaver(HttpOptions options = new HttpOptions())
        {
            HttpInstance = new Http(options);
        }
        #endregion

        #region Properties
        internal Http HttpInstance { get; private set; }

        internal HttpClient HttpClient
        {
            get => HttpInstance?.Client;
        }
        #endregion

        #region Internal Methods
        internal async Task<Page> FetchPaged(string url, CancellationToken token, IProgress<double> progress = null)
        {
            var resp = await HttpInstance.GetAsync(url, token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            Page p = resp.JSON<Page>();
            p.Client = this;

            foreach (Beatmap b in p.Docs)
            {
                b.Client = this;
                b.Uploader.Client = this;
            }

            return p;
        }

        internal async Task<Beatmap> FetchSingle(string url, CancellationToken token, IProgress<double> progress = null)
        {
            var resp = await HttpInstance.GetAsync(url, token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            Beatmap b = resp.JSON<Beatmap>();
            b.Client = this;
            b.Uploader.Client = this;

            return b;
        }

        internal async Task<Page> FetchMapsPage(string type, uint page, CancellationToken token, IProgress<double> progress = null, AutomapperQuery automappers = AutomapperQuery.None)
        {
            string pageURI = $"maps/{type}";
            string url = $"{pageURI}/{page}";
            string query = null;

            if (automappers == AutomapperQuery.All || automappers == AutomapperQuery.Only) {
                query = $"automapper={(int)automappers}";
                url += $"?{query}";
            }

            Page p = await FetchPaged(url, token, progress).ConfigureAwait(false);
            p.PageURI = pageURI;
            if (query != null) p.Query = query;

            return p;
        }

        internal async Task<Page> FetchSearchPage(string searchType, string query, uint page, CancellationToken token, IProgress<double> progress = null)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), "Query string cannot be null");
            }

            string pageURI = $"search/{searchType}";
            string q = $"q={Uri.EscapeUriString(query)}";

            string url = $"{pageURI}/{page}?{q}";
            Page p = await FetchPaged(url, token, progress).ConfigureAwait(false);

            p.Query = q;
            p.PageURI = pageURI;

            return p;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Latest(
            uint page = 0,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Latest, page, CancellationToken.None, progress, automappers).ConfigureAwait(false);
        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Latest(
            uint page, CancellationToken token,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Latest, page, token, progress, automappers).ConfigureAwait(false);

        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Hot(
            uint page = 0,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Hot, page, CancellationToken.None, progress, automappers).ConfigureAwait(false);
        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Hot(
            uint page,
            CancellationToken token,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Hot, page, token, progress, automappers).ConfigureAwait(false);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Rating(
            uint page = 0,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Rating, page, CancellationToken.None, progress, automappers).ConfigureAwait(false);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Rating(
            uint page,
            CancellationToken token,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Rating, page, token, progress, automappers).ConfigureAwait(false);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Downloads(
            uint page = 0,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Downloads, page, CancellationToken.None, progress, automappers).ConfigureAwait(false);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Downloads(
            uint page,
            CancellationToken token,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Downloads, page, token, progress, automappers).ConfigureAwait(false);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Plays(
            uint page = 0,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Plays, page, CancellationToken.None, progress, automappers).ConfigureAwait(false);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <param name="automappers">Include automappers in results</param>
        /// <returns></returns>
        public async Task<Page> Plays(
            uint page,
            CancellationToken token,
            IProgress<double> progress = null,
            AutomapperQuery automappers = AutomapperQuery.None
        ) => await FetchMapsPage(PageType.Plays, page, token, progress, automappers).ConfigureAwait(false);

        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Beatmap> Key(string key, IProgress<double> progress = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await FetchSingle($"maps/{SingleType.Key}/{key}", CancellationToken.None, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Beatmap> Key(string key, CancellationToken token, IProgress<double> progress = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await FetchSingle($"maps/{SingleType.Key}/{key}", token, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Beatmap> Hash(string hash, IProgress<double> progress = null)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            return await FetchSingle($"maps/{SingleType.Hash}/{hash}", CancellationToken.None, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Beatmap> Hash(string hash, CancellationToken token, IProgress<double> progress = null)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            return await FetchSingle($"maps/{SingleType.Hash}/{hash}", token, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Page> Search(string query, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Text, query, page, CancellationToken.None, progress).ConfigureAwait(false);
        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Page> Search(string query, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Text, query, page, token, progress).ConfigureAwait(false);

        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Page> SearchAdvanced(string query, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Advanced, query, page, CancellationToken.None, progress).ConfigureAwait(false);
        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Page> SearchAdvanced(string query, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Advanced, query, page, token, progress).ConfigureAwait(false);

        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<User> User(string id, IProgress<double> progress = null) => await User(id, CancellationToken.None, progress).ConfigureAwait(false);
        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<User> User(string id, CancellationToken token, IProgress<double> progress = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var resp = await HttpInstance.GetAsync($"users/find/{id}", token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            User u = resp.JSON<User>();
            u.Client = this;

            return u;
        }
        #endregion

        #region Extensions
#if NETSTANDARD2_1
        /// <summary>
        /// Get an async iterator for beatmaps, ordered by Latest
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> LatestIterator(uint page = 0)
        {
            while (true)
            {
                var hot = await Latest(page).ConfigureAwait(false);
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
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> HotIterator(uint page = 0)
        {
            while (true)
            {
                var hot = await Hot(page).ConfigureAwait(false);
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
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> RatingIterator(uint page = 0)
        {
            while (true)
            {
                var hot = await Rating(page).ConfigureAwait(false);
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
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> DownloadsIterator(uint page = 0)
        {
            while (true)
            {
                var hot = await Downloads(page).ConfigureAwait(false);
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
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> PlaysIterator(uint page = 0)
        {
            while (true)
            {
                var hot = await Plays(page).ConfigureAwait(false);
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
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> SearchIterator(string query, uint page = 0)
        {
            while (true)
            {
                var hot = await Search(query, page).ConfigureAwait(false);
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
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Beatmap> SearchAdvancedIterator(string query, uint page = 0)
        {
            while (true)
            {
                var hot = await SearchAdvanced(query, page).ConfigureAwait(false);
                foreach (var map in hot.Docs)
                {
                    yield return map;
                }

                if (hot.NextPage == null) yield break;
            }
        }
#endif
        #endregion
    }
}
