using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class OptionsRequest : Request {

        private String mPath;

        public OptionsRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            requestType = RequestType.PROPFIND;
            mPath = httpListenerRequest.Url.ToString();
            Console.WriteLine("Parsed OPTION REQUEST " + ToString());
        }

        public String getPath() {
            return mPath;
        }

        public void setPath(String path) {
            mPath = path;
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
            var file = FileManager.getInstanse().getFile("/options.txt");
            response.setContentLength(file.Length);
            response.setData(file);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

        public override string ToString() {
            return string.Format("mPath: {0}", mPath);
        }
    }
}
