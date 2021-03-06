namespace Noised.Core.Media
{
    /// <summary>
    ///		Media manager handles playback of media items
    /// </summary>
    public interface IMediaManager
    {
        /// <summary>
        ///		The current playback item or null if no item is playbacked
        /// </summary>
        MediaItem CurrentMediaItem { get; }

        /// <summary>
        ///		Shuffle mode 
        /// </summary>
        bool Shuffle { get; set; }

        /// <summary>
        /// Repeat mode 
        /// </summary>
        RepeatMode Repeat { get; set; }

        /// <summary>
        ///		Plays a media item 
        /// </summary>
        void Play(MediaItem item);

        /// <summary>
        ///		Stops playback
        /// </summary>
        void Stop();

        /// <summary>
        ///		Pauses playback
        /// </summary>
        void Pause();

        /// <summary>
        ///		Resumes playback
        /// </summary>
        void Resume();

        /// <summary>
        ///		Processes next music from queue or current playlist
        /// </summary>
        void ProcessNext();

        /// <summary>
        /// Sets or Gets the Volume
        /// </summary>
        int Volume { get; set; }
    }
}
