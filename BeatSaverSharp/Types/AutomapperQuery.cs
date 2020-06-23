namespace BeatSaverSharp
{
    /// <summary>
    /// </summary>
    public enum AutomapperQuery
    {
        /// <summary>
        /// Do not include automapped beatmaps in search results
        /// </summary>
        None,

        /// <summary>
        /// Only include automapped beatmaps in search results
        /// </summary>
        Only = -1,

        /// <summary>
        /// Include all beatmaps in search results
        /// </summary>
        All = 1,
    }
}
