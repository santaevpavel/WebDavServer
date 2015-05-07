using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class GetRequest : Request {

        private String mFileName;

        public GetRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.GET;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed GET REQUEST " + ToString());
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
                var response = new Response(200);
                FileStream file;
                try {
                    file = FileManager.getInstanse().getFileForRead(mFileName);
                } catch (FileNotFoundException) {
                    response = new Response(404);
                    return response;
                }
                if (null == file) {
                    return response;
                }
                response.setContentLength(file.Length);
                response.setData(file);
                return response;
            });
            task.Start();
            return task;
        }


    }
}
