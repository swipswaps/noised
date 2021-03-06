using System;
using System.Collections.Generic;
using Noised.Core.Commands;
using Noised.Core.Plugins;
using Noised.Core.Plugins.Audio;
using Noised.Core.Service;
using Noised.Logging;

namespace Noised.Core.Media
{
    public class MediaManager : IMediaManager
    {
        private static readonly object locker = new object();
        private readonly IPluginLoader pluginLoader;
        private readonly ILogging logger;
        private readonly ICore core;
        private IAudioPlugin currentAudioOutput;
        private readonly IServiceConnectionManager connectionManager;
        private MediaItem currentMediaItem;
        private volatile bool shuffle;
        private volatile RepeatMode repeat;
        private int volume;

        /// <summary>
        ///		Constructor
        /// </summary>
        /// <param name="pluginLoader">Pluginloader</param>
        /// <param name="logger">The logger</param>
        /// <param name="core">The core</param>
        /// <param name="connectionManager">The connection manager</param>
        public MediaManager(ILogging logger,
            IPluginLoader pluginLoader,
            ICore core,
            IServiceConnectionManager connectionManager)
        {
            this.logger = logger;
            this.pluginLoader = pluginLoader;
            this.core = core;
            this.connectionManager = connectionManager;
            this.volume = 50;

            foreach (IAudioPlugin audio in pluginLoader.GetPlugins<IAudioPlugin>())
                audio.SongFinished += OnSongFinished;

            repeat = RepeatMode.None;
        }

        private void OnSongFinished(Object sender, AudioEventArgs args)
        {
            logger.Info("Finished playing " + args.MediaItem.Uri);
            lock (locker)
            {
                currentMediaItem = null;
            }
            ProcessNext();
        }

        private IAudioPlugin GetAudioOutputForItem(MediaItem item)
        {
            foreach (IAudioPlugin audio in
                    pluginLoader.GetPlugins<IAudioPlugin>())
            {
                foreach (string protocol in audio.SupportedProtocols)
                {
                    if (item.Uri != null &&
                        item.Uri.ToString().StartsWith(protocol, StringComparison.Ordinal))
                    {
                        return audio;
                    }
                }
            }

            return null;
        }

        private void BroadcastMessage(ResponseMetaData message)
        {
            connectionManager.SendBroadcast(message);
        }

        #region IMediaManager

        public MediaItem CurrentMediaItem
        {
            get
            {
                lock (locker)
                {
                    return currentMediaItem;
                }
            }
        }

        public bool Shuffle
        {
            get
            {
                return shuffle;
            }

            set
            {
                shuffle = value;
            }
        }

        public RepeatMode Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                repeat = value;
            }
        }

        public void Play(MediaItem item)
        {
            lock (locker)
            {
                //Getting an appropriated plugin for the 
                IAudioPlugin audio = GetAudioOutputForItem(item);
                if (audio == null)
                {
                    throw new CoreException(
                        "Could not find an audio plugin supporting playback for " +
                        item.Uri);
                }

                //Stopping if another plugin is already playing
                if (currentAudioOutput != null)
                {
                    logger.Debug(String.Format("Stopping current audio playback " +
                            "through plugin {0}",
                            currentAudioOutput.GetMetaData().Name));
                    currentAudioOutput.Stop();
                }

                //Setting current audio plugin and play the song
                logger.Debug(String.Format("Using audio plugin {0} to play item {1}",
                        audio.GetMetaData().Name,
                        item.Uri));
                currentAudioOutput = audio;
                currentMediaItem = item;
            }

            currentAudioOutput.Volume = Volume;
            currentAudioOutput.Play(item);

            var broadcastMessage = new ResponseMetaData
            {
                Name = "Noised.Commands.Core.Play",
                Parameters = new List<Object>
                {
                    item
                }
            };
            BroadcastMessage(broadcastMessage);
        }

        public void Stop()
        {
            ResponseMetaData broadcastMessage = null;
            lock (locker)
            {
                if (currentAudioOutput != null)
                {
                    currentAudioOutput.Stop();
                    broadcastMessage = new ResponseMetaData
                    {
                        Name = "Noised.Commands.Core.Stop",
                        Parameters = new List<Object>
                        {
                            currentMediaItem
                        }
                    };
                }
                currentMediaItem = null;
                currentAudioOutput = null;
            }

            if (broadcastMessage != null)
            {
                BroadcastMessage(broadcastMessage);
            }
        }

        public void Pause()
        {
            ResponseMetaData broadcastMessage = null;
            lock (locker)
            {
                if (currentAudioOutput != null)
                {
                    currentAudioOutput.Pause();
                    broadcastMessage = new ResponseMetaData
                    {
                        Name = "Noised.Commands.Core.Pause",
                        Parameters = new List<Object>
                        {
                            currentMediaItem
                        }
                    };
                }
            }
            if (broadcastMessage != null)
            {
                BroadcastMessage(broadcastMessage);
            }
        }

        public void Resume()
        {
            ResponseMetaData broadcastMessage = null;
            lock (locker)
            {
                if (currentAudioOutput != null)
                {
                    currentAudioOutput.Resume();
                    broadcastMessage = new ResponseMetaData
                    {
                        Name = "Noised.Commands.Core.Play",
                        Parameters = new List<Object>
                        {
                            currentMediaItem
                        }
                    };
                }
            }
            if (broadcastMessage != null)
            {
                BroadcastMessage(broadcastMessage);
            }
        }

        public void ProcessNext()
        {
            core.ExecuteCommandAsync(new ProcessNextMusicCommand());
        }

        public int Volume
        {
            get
            {
                lock (locker)
                    return volume;
            }
            set
            {
                IAudioPlugin audio;

                lock (locker)
                {
                    audio = currentAudioOutput;

                    if (value <= 100 && value > 0)
                        volume = value;
                    else if (value < 0)
                        volume = 0;
                    else if (value > 100)
                        volume = 100;
                }

                if (audio != null)
                    audio.Volume = value;
            }
        }

        #endregion
    };
}
