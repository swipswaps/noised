using System;
using System.Linq;
using Noised.Logging;
using Noised.Core.Plugins;
using Noised.Core.Plugins.Audio;
namespace Noised.Core.Media
{
	public class MediaManager : IMediaManager
	{
		private static object locker = new object();
		private IPluginLoader pluginLoader;
		private IAudioPlugin currentAudioOutput;
		private ILogging logger;

		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="pluginLoader">Pluginloader</param>
		/// <param name="logger">The logger</param>
		public MediaManager (ILogging logger, IPluginLoader pluginLoader)
		{
			this.logger = logger;
			this.pluginLoader = pluginLoader;
		}

		private IAudioPlugin GetAudioOutputForItem(MediaItem item)
		{
			foreach (IAudioPlugin audio in 
					pluginLoader.GetPlugins<IAudioPlugin>())
			{
				foreach (string protocol in audio.SupportedProtocols)
				{
					if(item.Uri != null && 
					   item.Uri.ToString().StartsWith(protocol))
					{
						return audio;
					}
				}
			}

			return null;
		}

		#region IMediaManager
		
		public bool Shuffle {get;set;}
		
		public bool Repeat {get;set;}
		
		public void Play(MediaItem item)
		{
			lock(locker)
			{
				//Getting an appropriated plugin for the 
				IAudioPlugin audio = GetAudioOutputForItem(item);
				if(audio == null)
				{
					throw new CoreException(
						"Could not find an audio plugin supporting playback for " + 
						item.Uri.ToString());
				}

				//Stopping if another plugin is already playing
				if(currentAudioOutput != null)
				{
					logger.Debug(String.Format("Stopping current audio playback " + 
								               "through plugin {0}",
												currentAudioOutput.Name));
					currentAudioOutput.Stop();
				}

				//Setting current audio plugin and play the song
				logger.Debug(String.Format("Using audio plugin {0} to play item {1}",
								            audio.Name,
											item.Uri.ToString()));
				currentAudioOutput = audio;
			}
			currentAudioOutput.Play(item);
		}

		public void Stop()
		{
			lock(locker)
			{
				if(currentAudioOutput != null)
				{
					currentAudioOutput.Stop();
				}
				currentAudioOutput = null;
			}
		}

		public void Pause()
		{
			lock(locker)
			{
				if(currentAudioOutput != null)
				{
					currentAudioOutput.Pause();
				}
			}
		}

		public void Resume()
		{
			lock(locker)
			{
				if(currentAudioOutput != null)
				{
					currentAudioOutput.Resume();
				}
			}
		}
		
		#endregion
	};
}