using System;
using Noised.Core.Plugins;
using Noised.Core.Plugins.Audio;
namespace Noised.Plugins.Audio.Dummy
{
	/// <summary>
	///		A dummy audio plugin which does not play any sound
	/// </summary>
	/// <remarks> This is for testing purpose</remarks>
	public class DummyAudioPlugin  : IAudioPlugin
	{
		private bool isPlaying;

		#region Constructor
		
		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="pluginInitializer">initalizer</param>
		public DummyAudioPlugin(PluginInitializer pluginInitializer) { }
		
		#endregion

		#region IDisposable
		
		public void Dispose() { }
		
		#endregion

		#region IPlugin
		
		public String Name
		{
			get
			{
				return "DummyAudioPlugin";
			}
		}

		public String Description
		{
			get
			{
				return "Just a dummy audio plugin which does not " + 
					   "play any sound(for testing purpose only)";
			}
		}

		public String AuthorName
		{
			get
			{
				return "Benjamin Grüdelbach";
			}
		}

		public String AuthorContact
		{
			get
			{
				return "nocontact@availlable.de";
			}
		}

		public Version Version
		{
			get
			{
				return new Version(1,0);
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return DateTime.Parse("01.03.2015");
			}
		}
		
		#endregion

		#region IAudioPlugin
		
		public void Play(String fileName)
		{
			Play(fileName,0);
		}

		public void Play(String fileName, int pos)
		{
			isPlaying = true;
			Console.WriteLine( String.Format( "Playing song {0} from position {1}",
											  fileName, 
											  pos));
		}

		public void Stop()
		{
			isPlaying = false;
			Console.WriteLine("Playback stopped");
		}
		public void Pause()
		{
			Console.WriteLine("Playback paused");
		}

		public void Resume()
		{
			Console.WriteLine("Playback resumed");
		}

		public bool IsPlaying
		{
			get{ return isPlaying;}
		}

		public int Position{get;set;}

		public int Length
		{
			get{ return 100;}
		}

		public int Volume { get;set; }
		
		#endregion
	};
}
