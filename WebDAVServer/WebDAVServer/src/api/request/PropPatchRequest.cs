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

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private String _mPath;
        private readonly Stream _inStream;
        private String _content;
        private String _responseText;

        public PropPatchRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Proppatch;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mPath = url.Remove(0, host.Length);
            _inStream = httpListenerRequest.InputStream;
            Console.WriteLine("Parsed PROPPATCH REQUEST " + ToString());
        }

        internal String GetPath() {
            return _mPath;
        }

        internal void SetPath(String path) {
            _mPath = path;
        }

        internal override async Task DoCommandAsync() {
            var buffer = new byte[ProgramCostants.DefautBufferSize];
            var offset = 0;
            try {
                int count;
                while (0 < (count = await _inStream.ReadAsync(buffer, offset, buffer.Length - offset))) {
                    offset += count;
                }
            } catch (HttpListenerException e) {
                Console.WriteLine(e.Message);
            }
            _content = Encoding.UTF8.GetString(buffer, 0, offset);
            Logger.Trace(_content);
            _responseText = PropPatchHelper.ParsePropPatchContent(_mPath, _content);
        }

        internal override void DoCommand() {
            throw new Exception("Call async doCommandAsync");
        }

        internal override Response GetResponse() {
            var response = new Response(HttpStatusCodes.SuccessMultistatus);
            if (null == _responseText) {
                return new Response(HttpStatusCodes.SuccessOk);
            }
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(_responseText));
            response.SetContentLength(stream.Length);
            response.SetData(stream);
            return response;
        }

        internal override bool IsAsync() {
            return true;
        }

        public override string ToString() {
            return string.Format("mPath: {0}", _mPath);
        }
    }
}
