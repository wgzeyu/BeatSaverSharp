using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BeatSaverSharp
{
    /// <summary>
    /// </summary>
    public struct Metadata
    {
        /// <summary>
        /// </summary>
        [JsonProperty("songName")]
        public string SongName { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("songSubName")]
        public string SongSubName { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("songAuthorName")]
        public string SongAuthorName { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("levelAuthorName")]
        public string LevelAuthorName { get; private set; }

        /// <summary>
        /// Duration of the Audio File (in seconds)
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; private set; }

        /// <summary>
        /// Beats per Minute
        /// </summary>
        [JsonProperty("bpm")]
        public float BPM { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("difficulties")]
        public Difficulties Difficulties { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("characteristics")]
        public ReadOnlyCollection<BeatmapCharacteristic> Characteristics { get; private set; }
    }

    /// <summary>
    /// Available Difficulties
    /// </summary>
    public struct Difficulties
    {
        /// <summary>
        /// </summary>
        [JsonProperty("easy")]
        public bool Easy { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("normal")]
        public bool Normal { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("hard")]
        public bool Hard { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("expert")]
        public bool Expert { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("expertPlus")]
        public bool ExpertPlus { get; private set; }
    }

    /// <summary>
    /// </summary>
    public struct BeatmapCharacteristic
    {
        /// <summary>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("difficulties")]
        public ReadOnlyDictionary<string, BeatmapCharacteristicDifficulty?> Difficulties { get; private set; }
    }

    /// <summary>
    /// </summary>
    public struct BeatmapCharacteristicDifficulty
    {
        /// <summary>
        /// Length of the beatmap (in beats)
        /// </summary>
        [JsonProperty("duration")]
        public float Duration { get; private set; }

        /// <summary>
        /// Length of the beatmap (in seconds)
        /// </summary>
        [JsonProperty("length")]
        public int Length { get; private set; }

        /// <summary>
        /// Bomb Count
        /// </summary>
        [JsonProperty("bombs")]
        public int Bombs { get; private set; }

        /// <summary>
        /// Note Count
        /// </summary>
        [JsonProperty("notes")]
        public int Notes { get; private set; }

        /// <summary>
        /// Obstacle Count
        /// </summary>
        [JsonProperty("obstacles")]
        public int Obstacles { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("njs")]
        public float NoteJumpSpeed { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("njsOffset")]
        public float NoteJumpSpeedOffset { get; private set; }
    }

    /// <summary>
    /// </summary>
    public struct Stats
    {
        /// <summary>
        /// </summary>
        [JsonProperty("downloads")]
        public int Downloads { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("plays")]
        public int Plays { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("upVotes")]
        public int UpVotes { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("downVotes")]
        public int DownVotes { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("rating")]
        public float Rating { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("heat")]
        public float Heat { get; private set; }
    }
}
