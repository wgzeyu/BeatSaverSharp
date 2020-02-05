using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeatSaverSharp.Exceptions;

namespace BeatSaverSharp
{
    /// <summary>
    /// BeatSaver Beatmap
    /// </summary>
    public sealed class Beatmap : IEquatable<Beatmap>
    {
        #region Constructors
        /// <summary>
        /// Instantite a blank Beatmap
        /// </summary>
        [JsonConstructor]
        public Beatmap() { }

        /// <summary>
        /// Instantiate a partial Beatmap
        /// </summary>
        /// <param name="client">BeatSaver Client</param>
        /// <param name="key">Hex Key</param>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="name">Beatmap Name</param>
        public Beatmap(BeatSaver client = null, string key = null, string hash = null, string name = null)
        {
            if (key == null && hash == null)
            {
                throw new ArgumentException("Key and Hash cannot both be null");
            }

            Client = client ?? BeatSaver.Client;
            if (key != null) Key = key;
            if (hash != null) Hash = hash;
            if (name != null) Name = name;

            Partial = true;
        }
        #endregion

        #region JSON Properties
        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonProperty("_id")]
        public string ID { get; private set; }

        /// <summary>
        /// Hex Key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Multiline description. Can be null.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; private set; }

        /// <summary>
        /// User who uploaded this beatmap
        /// </summary>
        [JsonProperty("uploader")]
        public User Uploader { get; private set; }

        /// <summary>
        /// Timestamp when this map was uploaded
        /// </summary>
        [JsonProperty("uploaded")]
        public DateTime Uploaded { get; private set; }

        /// <summary>
        /// Metadata for the Beatmap .dat file
        /// </summary>
        [JsonProperty("metadata")]
        public Metadata Metadata { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("stats")]
        public Stats Stats { get; private set; }

        /// <summary>
        /// Direct Download URL. Skips the download counter.
        /// </summary>
        [JsonProperty("directDownload")]
        public string DirectDownload { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("downloadURL")]
        public string DownloadURL { get; private set; }

        /// <summary>
        /// URL for the Cover Art
        /// </summary>
        [JsonProperty("coverURL")]
        public string CoverURL { get; private set; }

        /// <summary>
        /// SHA1 Hash
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }
        #endregion

        #region Properties
        [JsonIgnore]
        internal BeatSaver Client { get; set; }

        /// <summary>
        /// File name for the Cover Art
        /// </summary>
        [JsonIgnore]
        public string CoverFilename
        {
            get => Path.GetFileName(CoverURL);
        }

        /// <summary>
        /// Beatmap contains partial data
        /// </summary>
        [JsonIgnore]
        public bool Partial { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Populate a partial Beatmap with data
        /// </summary>
        /// <returns></returns>
        public async Task Populate()
        {
            if (Key == null && Hash == null)
            {
                throw new InvalidPartialException("Key and Hash cannot both be null");
            }

            if (Partial == false) return;

            Beatmap map = Hash != null ? await Client.Hash(Hash) : await Client.Key(Key);
            if (map == null)
            {
                if (Hash != null) throw new InvalidPartialHashException(Hash);
                else if (Key != null) throw new InvalidPartialKeyException(Key);
                else throw new InvalidPartialException();
            }

            ID = map.ID;
            Key = map.Key;
            Name = map.Name;
            Description = map.Description;
            Uploader = map.Uploader;
            Uploaded = map.Uploaded;
            Metadata = map.Metadata;
            Stats = map.Stats;
            DirectDownload = map.DirectDownload;
            DownloadURL = map.DownloadURL;
            CoverURL = map.CoverURL;
            Hash = map.Hash;

            Partial = false;
        }

        /// <summary>
        /// Fetch latest values
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            Beatmap b = await Client.Hash(Hash);

            Name = b.Name;
            Description = b.Description;
            Stats = b.Stats;
        }

        /// <summary>
        /// Fetch latest stats
        /// </summary>
        /// <returns></returns>
        public async Task RefreshStats()
        {
            Beatmap b = await Client.Hash(Hash);
            Stats = b.Stats;
        }

        private enum VoteDirection : short
        {
            Up = 1,
            Down = -1,
        }

        private struct VotePayload
        {
            [JsonProperty("steamID")]
            public string SteamID { get; set; }

            [JsonProperty("ticket")]
            public string Ticket { get; set; }

            [JsonProperty("direction")]
            public string Direction { get; set; }

            public VotePayload(VoteDirection direction, string steamID, byte[] authTicket)
            {
                SteamID = steamID;
                Ticket = string.Concat(Array.ConvertAll(authTicket, x => x.ToString("X2")));
                Direction = ((short)direction).ToString();
            }

            public VotePayload(VoteDirection direction, string steamID, string authTicket)
            {
                SteamID = steamID;
                Ticket = authTicket;
                Direction = ((short)direction).ToString();
            }
        }

        private async Task<bool> Vote(VoteDirection direction, string steamID, byte[] authTicket)
        {
            VotePayload payload = new VotePayload(direction, steamID, authTicket);
            return await Vote(payload);
        }

        private async Task<bool> Vote(VoteDirection direction, string steamID, string authTicket)
        {
            VotePayload payload = new VotePayload(direction, steamID, authTicket);
            return await Vote(payload);
        }

        private async Task<bool> Vote(VotePayload payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await Client.HttpClient.PostAsync($"vote/steam/{Key}", content);
            if (resp.IsSuccessStatusCode)
            {
                using Stream s = await resp.Content.ReadAsStreamAsync();
                using StreamReader sr = new StreamReader(s);
                using JsonReader reader = new JsonTextReader(sr);

                Beatmap updated = Http.Serializer.Deserialize<Beatmap>(reader);
                Stats = updated.Stats;

                return true;
            }

            RestError error;
            using (Stream s = await resp.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                error = Http.Serializer.Deserialize<RestError>(reader);
            }

            if (error.Identifier == "ERR_INVALID_STEAM_ID") throw new InvalidSteamIDException(payload.SteamID);
            if (error.Identifier == "ERR_STEAM_ID_MISMATCH") throw new InvalidSteamIDException(payload.SteamID);
            if (error.Identifier == "ERR_INVALID_TICKET") throw new InvalidTicketException();
            if (error.Identifier == "ERR_BAD_TICKET") throw new InvalidTicketException();

            return false;
        }

        /// <summary>
        /// Submit an Upvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket</param>
        /// <returns></returns>
        public async Task<bool> VoteUp(string steamID, byte[] authTicket) => await Vote(VoteDirection.Up, steamID, authTicket);
        /// <summary>
        /// Submit an Upvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket (Hex String)</param>
        /// <returns></returns>
        public async Task<bool> VoteUp(string steamID, string authTicket) => await Vote(VoteDirection.Up, steamID, authTicket);

        /// <summary>
        /// Submit a Downvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket</param>
        /// <returns></returns>
        public async Task<bool> VoteDown(string steamID, byte[] authTicket) => await Vote(VoteDirection.Down, steamID, authTicket);
        /// <summary>
        /// Submit a Downvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket (Hex String)</param>
        /// <returns></returns>
        public async Task<bool> VoteDown(string steamID, string authTicket) => await Vote(VoteDirection.Down, steamID, authTicket);

        /// <summary>
        /// Download the Beatmap Zip as a byte array
        /// </summary>
        /// <param name="direct">If true, will skip counting the download request</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadZip(bool direct = false, IProgress<double> progress = null) => await DownloadZip(direct, CancellationToken.None, progress);
        /// <summary>
        /// Download the Beatmap Zip as a byte array
        /// </summary>
        /// <param name="direct">If true, will skip counting the download request</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadZip(bool direct, CancellationToken token, IProgress<double> progress = null)
        {
            string url = direct ? DirectDownload : DownloadURL;
            var resp = await Client.HttpInstance.GetAsync(url, token, progress).ConfigureAwait(false);

            return resp.Bytes();
        }

        /// <summary>
        /// Fetch the Beatmap's Cover Image as a byte array
        /// </summary>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<byte[]> FetchCoverImage(IProgress<double> progress = null) => await FetchCoverImage(CancellationToken.None, progress);
        /// <summary>
        /// Fetch the Beatmap's Cover Image as a byte array
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public async Task<byte[]> FetchCoverImage(CancellationToken token, IProgress<double> progress = null)
        {
            string url = $"{BeatSaver.BaseURL}{CoverURL}";
            var resp = await Client.HttpInstance.GetAsync(url, token, progress).ConfigureAwait(false);

            return resp.Bytes();
        }
        #endregion

        #region Equality
        /// <summary>
        /// Check for value equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as Beatmap);

        /// <summary>
        /// Check for value equality
        /// </summary>
        /// <param name="b">Beatmap to compare against</param>
        /// <returns></returns>
        public bool Equals(Beatmap b)
        {
            if (b is null) return false;
            if (ReferenceEquals(this, b)) return true;
            if (this.GetType() != b.GetType()) return false;

            return (ID == b.ID) && (Key == b.Key) && (Hash == b.Hash);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (ID is null ? 0 : ID.GetHashCode());
                hash = (hash * HashingMultiplier) ^ (Key is null ? 0 : Key.GetHashCode());
                hash = (hash * HashingMultiplier) ^ (Hash is null ? 0 : Hash.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Beatmap lhs, Beatmap rhs)
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
        public static bool operator !=(Beatmap lhs, Beatmap rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
