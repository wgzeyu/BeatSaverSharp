using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatSaverSharp
{
    /// <summary>
    /// BeatSaver User
    /// </summary>
    public sealed class User : IEquatable<User>
    {
        #region JSON Properties
        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonProperty("_id")]
        public string ID { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Fetch all Beatmaps uploaded by this user
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        public async Task<Page> Beatmaps(uint page = 0, IProgress<double> progress = null) => await Beatmaps(page, CancellationToken.None, progress);
        /// <summary>
        /// Fetch all Beatmaps uploaded by this user
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<Page> Beatmaps(uint page, CancellationToken token, IProgress<double> progress = null)
        {
            string pageURI = $"maps/{PageType.Uploader}/{ID}";
            string url = $"{pageURI}/{page}";

            Page p = await BeatSaver.FetchPaged(url, token, progress);
            p.PageURI = pageURI;

            return p;
        }
        #endregion

        #region Equality
        /// <summary>
        /// Check for value equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as User);

        /// <summary>
        /// Check for value equality
        /// </summary>
        /// <param name="u">User to compare against</param>
        /// <returns></returns>
        public bool Equals(User u)
        {
            if (u is null) return false;
            if (ReferenceEquals(this, u)) return true;
            if (this.GetType() != u.GetType()) return false;

            return (ID == u.ID);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => ID.GetHashCode();

        /// <summary>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(User lhs, User rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                else return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(User lhs, User rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
