using System;
using System.Collections.Generic;
using System.Linq;
using Noised.Core.Commands;
using Noised.Core.IOC;
using Noised.Core.Plugins;
using Noised.Core.Plugins.Service;
using Noised.Core.Service.Protocols;
using Noised.Logging;

namespace Noised.Core.Service
{
    /// <summary>
    ///		Handles incoming service connections
    /// </summary>
    public class ServiceConnectionManager : IServiceConnectionManager
    {
        #region Fields

        private readonly ILogging logging;
        private ICore core;
        private List<ServiceConnectionContext> connections;
        private readonly Object locker = new Object();

        #endregion

        #region Constructor

        /// <summary>
        ///	Constructor
        /// </summary>
        public ServiceConnectionManager()
        {
            this.logging = IoC.Get<ILogging>();
            this.core = IoC.Get<ICore>();
            this.connections = new List<ServiceConnectionContext>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///	Internal method to handle an incoming service connection
        /// </summary>
        private void ClientConnected(object sender, ServiceEventArgs eventArgs)
        {
            lock (locker)
            {
                connections.Add(
                    new ServiceConnectionContext(core,
                        eventArgs.Connection,
                        IoC.Get<IProtocol>()));
            }
            logging.Debug(String.Format("A new client connected. {0} clients connected.", connections.Count));
        }

        /// <summary>
        ///     Internal method to handle a service disconnection
        /// </summary>
        private void ClientDisconnected(object sender, ServiceEventArgs eventArgs)
        {
            lock (locker)
            {
                connections.RemoveAll(c => c.Connection == eventArgs.Connection);
            }
            logging.Debug(String.Format("A client disconnected. {0} clients connected.", connections.Count));
        }

        #endregion

        #region IServiceConnectionManager

        public void StartServices()
        {
            //Getting all loaded service plugins
            var services = IoC.Get<IPluginLoader>().GetPlugins<IService>();
            int serviceCount = services.Count();
            if (serviceCount > 0)
            {
                logging.Debug("Starting " + serviceCount + " services...");
            }
            else
            {
                logging.Warning("No services found :-(.");
            }

            //Starting all services
            foreach (IService service in services)
            {
                try
                {
                    logging.Debug("Starting service " + service.GetMetaData().Name + "...");
                    service.Start();
                    service.ClientConnected += ClientConnected;
                    service.ClientDisconnected += ClientDisconnected;
                    logging.Debug("Service " + service.GetMetaData().Name + " started");
                }
                catch (Exception e)
                {
                    logging.Error("Error starting service " + service.GetMetaData().Name + ": " + e.Message);
                    logging.Debug(e.StackTrace);
                }
            }
        }

        public void SendBroadcast(ResponseMetaData response)
        {
            try
            {
                logging.Debug("Sending broadcast response " + response);
                List<ServiceConnectionContext> activeConnections;
                lock (locker)
                {
                    activeConnections = connections;
                }
                foreach (var connection in activeConnections)
                {
                    connection.SendResponse(response);
                }
            }
            catch (Exception e)
            {
                logging.Error(e.Message);
                throw;
            }
        }

        #endregion
    };
}
