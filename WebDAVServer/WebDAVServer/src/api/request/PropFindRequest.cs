using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebDAVServer.api.helpers;
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
            //mPath = httpListenerRequest.Url.ToString();
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mPath = url.Remove(0, host.Length);
            var depth = httpListenerRequest.Headers.Get(DEPTH_NAME);
            mDepth = Int32.Parse(depth);
            Console.WriteLine("Parsed PROPFIND REQUEST " + ToString());
            var keys = httpListenerRequest.Headers.AllKeys;
            foreach (var key in keys) {
                Console.WriteLine(key + "->" + httpListenerRequest.Headers.GetValues(key)[0]);    
            }

            /*byte[] bytes = new byte[1024 * 10];
            int size = httpListenerRequest.InputStream.Read(bytes, 0, bytes.Length);
            var str = System.Text.Encoding.UTF8.GetString(bytes, 0, size);
            Console.WriteLine(str);*/
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
            //var file = FileManager.getInstanse().getFile("/propfind.xml");
            var str = PropFindHelper.getFilesPropInDir(mPath);
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            response.setContentLength(stream.Length);
            response.setData(stream);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

        public override string ToString() {
            return string.Format("mDepth: {0}, mPath: {1}", mDepth, mPath);
        }
    }
}
