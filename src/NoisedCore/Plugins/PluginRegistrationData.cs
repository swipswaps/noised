using System;
using System.Collections.Generic;

namespace Noised.Core.Plugins
{
	public class PluginRegistrationData
	{
		/// <summary>
		///		The unique plugin ID
		/// </summary>
		public Guid Guid{get;set;}
	
		/// <summary>
		///		The Plugin Version
		/// </summary>
		public String Version{get;set;}
	
		/// <summary>
		///		The name of the plugin
		/// </summary>
		public String Name{get;set;}
	};
}