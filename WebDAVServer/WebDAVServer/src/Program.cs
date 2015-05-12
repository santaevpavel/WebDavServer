using System;
using System.Net;
using System.Net.Sockets;
using NLog;
using WebDAVServer.core;

namespace WebDAVServer {
    class Program {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args) {
            Logger.Info("Program is launched");
            var root = "../../res";
            if (args.Length > 0) {
                root = args[0];
            }
            var port = 27998;//FreeTcpPort();
            if (args.Length > 1) {
                try {
                    port = Int32.Parse(args[1]);
                } catch (Exception) {
                    Console.WriteLine("Can't parse port " + port + "\n" + "Port is choosed by server");
                    port = FreeTcpPort();
                }
            }
            using (var server = new Server(root, port)) {
                try {
                    server.Start();
                } catch (HttpListenerException) {
                    Console.WriteLine("Can't bind to port " + port);
                    Console.ReadKey();
                } catch (Exception e) {
                    Console.WriteLine("Server error: " + e);
                }
                Logger.Info("Program is finished");    
            }
        }

        private static int FreeTcpPort() {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
