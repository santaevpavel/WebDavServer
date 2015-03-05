using System.Threading;
using WebDAVServer.core;

namespace WebDAVServer {
    class Program {
        static void Main(string[] args) {
            var server = new Server("", 80);
            server.start();
            Thread.Sleep(9999999);
        }
    }
}
