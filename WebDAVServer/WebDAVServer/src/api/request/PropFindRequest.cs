using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class PropFindRequest : Request {

        private const String DEPTH_NAME = "depth";

        private String mPath;
        private int mDepth;

        public PropFindRequest(HttpListenerRequest httpListenerRequest) : base(httpListenerRequest) {
            requestType = RequestType.PROPFIND;
            mPath = httpListenerRequest.Url.ToString();
            var depth = httpListenerRequest.Headers.Get(DEPTH_NAME);
            mDepth = Int32.Parse(depth);
            Console.WriteLine("Parsed PROPFIND REQUEST " + ToString());
        }

        public String getPath() {
            return mPath;
        }

        public void setPath(String path) {
            mPath = path;
        }

        public int getDepth() {
            return mDepth;
        }

        public void setDepth(int depth) {
            mDepth = depth;
        }

        public override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {

        }

        public override Task<Response> getResponse() {
            var response = new Response(207);
            var file = FileManager.getInstanse().getFile("/propfind.txt");
            response.setContentLength(file.Length);
            response.setData(file);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

        public override string ToString() {
            return string.Format("mDepth: {0}, mPath: {1}", mDepth, mPath);
        }
    }
}
