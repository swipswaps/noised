using Noised.Core.Commands;
using Noised.Core.IOC;
using Noised.Core.Media;
using Noised.Core.Service;

namespace Noised.Plugins.Commands.CoreCommands
{
    public class Stop : AbstractCommand
    {
        /// <summary>
        ///		Constructor
        /// </summary>
        /// <param name="context">Connection context</param>
        public Stop(ServiceConnectionContext context)
            : base(context) { }

        #region AbstractCommand

        protected override void Execute()
        {
            var mediaManager = IocContainer.Get<IMediaManager>();
            mediaManager.Stop();
        }

        #endregion
    };
}
