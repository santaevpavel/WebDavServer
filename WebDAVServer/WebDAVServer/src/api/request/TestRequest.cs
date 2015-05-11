using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class TestRequest : Request {

        private readonly String mFileName;

        public TestRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            requestType = RequestType.GET;
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed TEST " + httpListenerRequest.HttpMethod + " REQUEST " + ToString());
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        internal override Task doCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void doCommand() {
        }

        internal override Response getResponse() {
            var response = new Response(HttpStatusCodes.SUCCESS_OK);
            response.setContentLength(0);
            response.setData(null);
            return response;
        }

        internal override bool isAsync() {
            return false;
        }
    }
}
