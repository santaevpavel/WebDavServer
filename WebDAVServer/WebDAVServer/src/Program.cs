using System;
using System.Net;
using System.Net.Sockets;
using NLog;
using WebDAVServer.core;

namespace WebDAVServer {
    class Program {

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        // имя метода не может быть main
        // ReSharper disable once InconsistentNaming
        public static void Main(string[] args) {
            LOGGER.Info("Program is launched");
            var root = "../../res";
            if (args.Length > 0) {
                root = args[0];
            }
            var port = freeTcpPort();
            if (args.Length > 1) {
                try {
                    port = Int32.Parse(args[1]);
                } catch (Exception) {
                    Console.WriteLine("Can't parse port " + port + "\n" + "Port is choosed by server");
                    port = freeTcpPort();
                }
            }
            using (var server = new Server(root, port)) {
                try {
                    server.start();
                } catch (HttpListenerException) {
                    Console.WriteLine("Can't bind to port " + port);
                    Console.ReadKey();
                } catch (Exception e) {
                    Console.WriteLine("Server error: " + e);
                }
                LOGGER.Info("Program is finished");    
            }
        }

        private static int freeTcpPort() {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
