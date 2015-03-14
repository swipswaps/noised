using System.Collections.Generic;
using System.Threading;
using Noised.Core.Commands;
using Noised.Logging;
using Noised.Core.IOC;

namespace Noised.Core
{
	/// <summary>
	///		The noised core processing all incoming commands
	/// </summary>
	internal class Core : ICore
	{
		#region Fields
		
		private readonly Queue<AbstractCommand> commandQueue;
		private bool isRunning;
		private ILogging logging;
		
		#endregion

		#region Constructor
		
		/// <summary>
		///		Constructor
		/// </summary>
		public Core()
		{
			commandQueue = new Queue<AbstractCommand>();
			logging = IocContainer.Get<ILogging>();
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		///		Internal method to process the core's command queue
		/// </summary>
		private void ProcessCommands()
		{
			isRunning = true;
			logging.Debug("noised core started");
			while(isRunning)
			{
				AbstractCommand cmd = null;
				lock(commandQueue)
				{
					if(commandQueue.Count > 0)
						cmd = commandQueue.Dequeue();
				}

				if(cmd != null)
				{
					cmd.ExecuteCommand();
					logging.Debug("Executed: " + cmd);
				}

				Thread.Sleep(1);
			}
			logging.Debug("noised core stopped");
		}

		#endregion

		#region ICore
		
		public bool IsRunning
		{
			get
			{
				lock(this)
				{
					return isRunning;
				}
			}
		}

		public void Start()
		{
			lock(this)
			{
				if(isRunning)
					throw new CoreException(
						"Cannot start noised Core because it's still running");

				Thread coreThread = new Thread(ProcessCommands);
				coreThread.Start();
				logging.Debug("Starting noised core...");
			}
		}

		public void Stop()
		{
			lock(this)
			{
				isRunning = false;
				logging.Debug("Stopping noised core...");
			}
		}

		public void AddCommand(AbstractCommand command)
		{
			lock(commandQueue)	
			{
				commandQueue.Enqueue(command);
			}
		}
		
		#endregion
	};
}