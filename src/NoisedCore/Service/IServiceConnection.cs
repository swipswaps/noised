using Noised.Core.Plugins.Service;

namespace Noised.Core.Service
{
    /// <summary>
    ///		A connection established to a network service INetworkService
    /// </summary>
    public interface IServiceConnection
    {
        #region Events

        /// <summary>
        ///		Fired if data was received for the connection
        /// </summary>
        event ServiceEventHandler DataReceived;
		
        /// <summary>
        ///		Fired if the connection has been closed
        /// </summary>
        event ServiceEventHandler Closed;

        #endregion

        #region Properties

        /// <summary>
        ///		The service related to the connection
        /// </summary>
        IService Service{ get; }

        #endregion

        #region Methods

        /// <summary>
        ///		Send data
        /// </summary>
        /// <param name="data">The data to send</param>
        void Send(byte[] data);
		
        /// <summary>
        ///		Closes the connection
        /// </summary>
        void Close();

        #endregion
    };
}
