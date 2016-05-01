using System;
using Noised.Logging;
using Noised.Core.Config;
using Noised.Core.DB;
using Noised.Core.IOC;
using Noised.Core.Media;
using Noised.Core.Plugins;
using Noised.Core.Service;
using Noised.Core.UserManagement;

namespace Noised.Core
{
    /// <summary>
    ///	    Starts the noised core
    /// </summary>
    public class CoreStarter
    {
        private readonly string[] args;

        /// <summary>
        ///	Constructor
        /// </summary>
        /// <param name="args">Commandline arguments to handle for start</param>
        public CoreStarter(string[] args)
        {
            this.args = args;
        }

        /// <summary>
        ///	Internal method to handle noised command line arguments
        /// </summary>
        private int HandleArgs(string[] arguments)
        {
            //TODO: Refactor: Put Parsing in it's own class; Put Argument handling in it's own class
            ILogging logger = IoC.Get<ILogging>();

            if (arguments.Length == 1)
            {
                if (arguments[0].StartsWith("CreateUser=", StringComparison.Ordinal))
                {
                    string realArgs = arguments[0].Split('=')[1];

                    if (string.IsNullOrWhiteSpace(realArgs))
                    {
                        logger.Error("CreateUser command was invoked without a parameter");
                        return -2;
                    }

                    string[] realArgArr = realArgs.Split(',');

                    IoC.Get<IUserManager>().CreateUser(realArgArr[0], realArgArr[1]);

                    logger.Info("\"" + realArgArr[0] + "\" can now rock this noised!\nPress Enter to continue!");
                    Console.ReadLine();

                    return 0;
                }
            }

            logger.Error("NoisedServer was invoked with an invalid number of arguments or an unknown argument");
            return -1;
        }

        /// <summary>
        ///	Starting the noised core 
        /// </summary>
        public int Start()
        {
            IoC.Build();
            ILogging logger = IoC.Get<ILogging>();
            logger.AddLogger(new ConsoleLogger());

            if (args != null && args.Length != 0)
                return HandleArgs(args);

            logger.Info("Hello I am Noised - your friendly music player daemon");

            //Creating the DB or update if neccessary
            var db = IoC.Get<IDB>();
            db.CreateOrUpdate();

            //installing new plugins
            var pluginInstaller = IoC.Get<IPluginInstaller>();
            pluginInstaller.InstallAll("./plugins");

            //Loading configuration
            var config = IoC.Get<IConfig>();
            config.Load(IoC.Get<IConfigurationLoader>());

            //loading plugins
            logger.Info("Loading plugins:");
            var pluginLoader = IoC.Get<IPluginLoader>();
            int pluginCount = pluginLoader.LoadPlugins();
            logger.Info(pluginCount + " plugins loaded ");

            //Add a factory and create a ping command
            var core = IoC.Get<ICore>();
            core.Start();

            //Starting services
            var serviceConnectionManager = IoC.Get<IServiceConnectionManager>();
            serviceConnectionManager.StartServices();

            // Refreshing music
            logger.Info("Refreshing music...");
            var sourceAccumulator = IoC.Get<IMediaSourceAccumulator>();
            sourceAccumulator.Refresh();
            logger.Info("Done refreshing music.");

            logger.Info("Noised has been started.");

            return 0;
        }
    };
}
