using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WebDAVServer.api.helpers;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class PropPatchRequest : Request {

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private String mPath;
        private readonly Stream inStream;
        private String content;
        private String responseText;

        public PropPatchRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.PROPPATCH;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mPath = url.Remove(0, host.Length);
            inStream = httpListenerRequest.InputStream;
            Console.WriteLine("Parsed PROPPATCH REQUEST " + ToString());
        }

        internal String getPath() {
            return mPath;
        }

        internal void setPath(String path) {
            mPath = path;
        }

        internal override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {
            var buffer = new byte[1024 * 1024];
            var offset = 0;
            try {
                int count;
                while (0 < (count = inStream.Read(buffer, offset, buffer.Length - offset))) {
                    offset += count;
                }
            } catch (HttpListenerException e) {
                Console.WriteLine(e.Message);
            }
            content = Encoding.UTF8.GetString(buffer, 0, offset);
            LOGGER.Trace(content);
            responseText = PropPatchHelper.parsePropPatchContent(mPath, content);
        }

        internal override Task<Response> getResponse() {
            var task = new Task<Response>(() => {
                var response = new Response(207);
                if (null == responseText) {
                    return new Response(200);
                }
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseText));
                response.setContentLength(stream.Length);
                response.setData(stream);
                return response;
            });
            task.Start();
            return task;
        }

        public override string ToString() {
            return string.Format("mPath: {0}", mPath);
        }
    }
}
