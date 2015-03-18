using WebDAVServer.core;

namespace WebDAVServer {
    class Program {
        static void Main(string[] args) {
            var server = new Server("../../res", 27999);
            server.start();
        }
    }
}
