using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class GetRequest : Request {

        private String mFileName;

        public GetRequest(HttpListenerRequest httpListenerRequest) : base(httpListenerRequest) {
            requestType = RequestType.GET;
            mFileName = httpListenerRequest.Url.ToString();
            Console.WriteLine("Parsed GET REQUEST " + ToString());
        }

        public String getFileName() {
            return mFileName;
        }

        public void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        public override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }

        public override Task<Response> getResponse() {
            var task = new Task<Response>(() => new Response(200));
            task.Start();
            return task;
        }

        private void doCommand() {
            
        }
    }
}
