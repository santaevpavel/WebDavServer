using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class OptionsRequest : Request {

        private String _mPath;

        public OptionsRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Propfind;
            _mPath = httpListenerRequest.Url.ToString();
            Console.WriteLine("Parsed OPTION REQUEST " + ToString());
            var keys = httpListenerRequest.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = httpListenerRequest.Headers.GetValues(key);
                if (strings != null) {
                    Console.WriteLine(key + "->" + strings[0]);
                }
            }
        }

        internal String GetPath() {
            return _mPath;
        }

        internal void SetPath(String path) {
            _mPath = path;
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }
        internal override void DoCommand() {
        }

        internal override Response GetResponse() {
            var response = new Response(HttpStatusCodes.SuccessOk);
            response.AddHeaderValue("allow", "OPTIONS,GET,HEAD,POST,DELETE,PROPFIND,PROPPATCH,COPY,MOVE,LOCK,UNLOCK");
            response.AddHeaderValue("DAV", "1,2");
            response.SetContentLength(0);
            return response;
        }

        internal override bool IsAsync() {
            return false;
        }

        public override string ToString() {
            return string.Format("mPath: {0}", _mPath);
        }
    }
}
