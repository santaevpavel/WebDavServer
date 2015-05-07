using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class OptionsRequest : Request {

        private String mPath;

        public OptionsRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.PROPFIND;
            mPath = httpListenerRequest.Url.ToString();
            Console.WriteLine("Parsed OPTION REQUEST " + ToString());
            var keys = httpListenerRequest.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = httpListenerRequest.Headers.GetValues(key);
                if (strings != null) {
                    Console.WriteLine(key + "->" + strings[0]);
                }
            }
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
        private static void doCommand() {

        }

        internal override Task<Response> getResponse() {
            var response = new Response(200);
            response.addHeaderValue("allow", "OPTIONS,GET,HEAD,POST,DELETE,PROPFIND,PROPPATCH,COPY,MOVE,LOCK,UNLOCK");
            response.addHeaderValue("DAV", "1,2");
            response.setContentLength(0);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

        public override string ToString() {
            return string.Format("mPath: {0}", mPath);
        }
    }
}
