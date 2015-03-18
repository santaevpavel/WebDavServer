using System.IO;
using System.Net;

namespace WebDAVServer.api.response {
    internal class Response {

        private readonly int mCode;
        private Stream mData;
        private long contentLength;

        public Response(int code) {
            mCode = code;
        }

        public void setData(Stream data) {
            mData = data;
        }

        public void setContentLength(long length) {
            contentLength = length;
        }

        public virtual async void setResponse(HttpListenerResponse response) {
            response.StatusCode = mCode;
            response.ContentLength64 = contentLength;
            if (null == mData) {
                response.OutputStream.Close();
                return;
            }
            var buffer = new byte[1024];
            using (var output = response.OutputStream) {
                int i;
                while (0 < (i = await mData.ReadAsync(buffer, 0, buffer.Length))) {
                    await output.WriteAsync(buffer, 0, i);
                }
                output.Close();
                mData.Close();
            }
        }
    }
}
