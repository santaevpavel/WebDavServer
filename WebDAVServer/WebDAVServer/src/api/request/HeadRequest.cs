using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class HeadRequest : Request {

        private String mFileName;

        public HeadRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.HEAD;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed HEAD REQUEST " + ToString());
        }

        internal String getFileName() {
            return mFileName;
        }

        internal void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        internal override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private static void doCommand() {

        }

        internal override Task<Response> getResponse() {
            var task = new Task<Response>(() => {
                if (FileManager.getInstanse().getDirInfo(mFileName).Exists
                    || FileManager.getInstanse().getFileInfo(mFileName).Exists) {
                    return new Response(204);   
                }
                return new Response(404);
            });
            task.Start();
            return task;
        }


    }
}
