using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class DeleteRequest : Request {

        private String mFileName;
        private int code;

        public DeleteRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.DELETE;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed DELETE REQUEST " + ToString());
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
        private void doCommand() {
            try {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
                code = 200;
            } catch (FileNotFoundException) {
                code = 404;
            } catch (DirectoryNotFoundException) {
                code = 404;
            }
        }

        internal override Task<Response> getResponse() {
            var response = new Response(code);
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }


    }
}
