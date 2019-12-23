using System;

namespace BeatSaverSharp.Exceptions
{
    /// <summary>
    /// Thrown when trying to populate an invalid partial Beatmap
    /// </summary>
    public class InvalidPartialException : Exception
    {
        /// <summary>
        /// </summary>
        public InvalidPartialException() : base() { }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public InvalidPartialException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when trying to populate an invalid partial Beatmap with an invalid Key
    /// </summary>
    public class InvalidPartialKeyException : InvalidPartialException
    {
        /// <summary>
        /// Invalid Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        public InvalidPartialKeyException(string key) : base()
        {
            Key = key;
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public InvalidPartialKeyException(string key, string message) : base(message)
        {
            Key = key;
        }
    }

    /// <summary>
    /// Thrown when trying to populate an invalid partial Beatmap with an invalid Hash
    /// </summary>
    public class InvalidPartialHashException : InvalidPartialException
    {
        /// <summary>
        /// Invalid Hash
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="hash"></param>
        public InvalidPartialHashException(string hash) : base()
        {
            Hash = hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="message"></param>
        public InvalidPartialHashException(string hash, string message) : base(message)
        {
            Hash = hash;
        }
    }
}
