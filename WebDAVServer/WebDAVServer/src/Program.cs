using WebDAVServer.core;

namespace WebDAVServer {
    class Program {
        static void Main(string[] args) {
            var server = new Server("", 27999);
            server.start();
        }
    }
}
