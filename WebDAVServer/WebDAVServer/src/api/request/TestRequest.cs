using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class TestRequest : Request {

        private String mFileName;

        public TestRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            //requestType = RequestType.GET;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed TEST " + httpListenerRequest.HttpMethod + " REQUEST " + ToString());
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        public override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {

        }

        public override Task<Response> getResponse() {
            var response = new Response(200); 
            /*var file = FileManager.getInstanse().getFile(mFileName);
            if (null != file) {
                response.setContentLength(file.Length);
                response.setData(file);
            }*/
            response.setContentLength(0);
            response.setData(null);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

       
    }
}
