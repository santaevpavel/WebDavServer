using System;
using System.Net;
using System.Text;

namespace WebDAVServer.api.response {
    internal class Response {
        private int mCode;

        public Response(int code) {
            mCode = code;

        }

        public async void setResponse(HttpListenerResponse response) {
            response.StatusCode = mCode;
            const string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream) {
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();    
            }
        }
    }
}
