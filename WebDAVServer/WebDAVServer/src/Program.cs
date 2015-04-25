using System;
using System.Net;
using NLog;
using WebDAVServer.core;

namespace WebDAVServer {
    class Program {

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args) {
            LOGGER.Info("Program is launched");
            var root = "../../res";
            if (args.Length > 0) {
                root = args[0];
            }
            const int port = ProgramCostants.DEFAUT_SERVER_PORT;
            var server = new Server(root, port);
            try {
                server.start();
            } catch (HttpListenerException) {
                Console.WriteLine("Can't bind to port " + port);
                Console.ReadKey();
            }
            LOGGER.Info("Program is finished");
        }
    }
}
