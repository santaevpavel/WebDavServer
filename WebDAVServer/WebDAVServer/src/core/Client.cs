using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebDAVServer.core {
    class Client : IDisposable{

        private const int BUFFER_SIZE = 1024*100;
        private readonly TcpClient mTcpClient;
        private byte[] mBuffer;

        public Client(TcpClient tcpClient) {
            if (null == tcpClient) {
                throw new ArgumentNullException("tcpClient");
            }
            mTcpClient = tcpClient;
            mBuffer = new byte[BUFFER_SIZE];
        }

        public TcpClient getTcpClient() {
            return mTcpClient;
        }

        public async Task readAndReply() {
            using (var networkStream = mTcpClient.GetStream()) {
                Console.WriteLine("[Server] Reading from client");
                var byteCount = await networkStream.ReadAsync(mBuffer, 0, mBuffer.Length);
                var request = Encoding.UTF8.GetString(mBuffer, 0, byteCount);
                Console.WriteLine("[Server] Client wrote {0}", request);
                reply(networkStream);
            }
        }

        private async void reply(NetworkStream networkStream) {
            string Html = "<html><body><h1>It works!</h1></body></html>";
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            await networkStream.WriteAsync(Buffer, 0, Buffer.Length);
            Console.WriteLine("[Server] Response has been written");
        }

        public void Dispose() {
            mTcpClient.Close();
        }

        private void parseRequest(String request) {
            /*HttpWebRequest HttpWReq = WebRequest.CreateHttp(request);
            var wr = WebResponse.
            // Insert code that uses the response object.
            HttpWResp.Close();

            HttpListenerRequest listenerRequest = new HttpListenerRequest;

            WebRequest webRequest = WebRequest.Create(listenerRequest.Url);
            webRequest.Method = listenerRequest.HttpMethod;
            webRequest.Headers.Add(listenerRequest.Headers);
            byte[] body = new byte[listenerRequest.InputStream.Length];
            listenerRequest.InputStream.Read(body, 0, body.Length);
            webRequest.GetRequestStream().Write(body, 0, body.Length);

            WebResponse webResponse = webRequest.GetResponse();*/
        }
    }
}
