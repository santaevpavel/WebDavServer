using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class TestRequest : Request {

        private readonly String _mFileName;

        public TestRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            RequestType = RequestType.Get;
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed TEST " + httpListenerRequest.HttpMethod + " REQUEST " + ToString());
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", _mFileName);
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void DoCommand() {
        }

        internal override Response GetResponse() {
            var response = new Response(HttpStatusCodes.SuccessOk);
            response.SetContentLength(0);
            response.SetData(null);
            return response;
        }

        internal override bool IsAsync() {
            return false;
        }
    }
}
