using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class CopyRequest : Request {

        private String mFileName;
        private readonly String mDstFileName;
        private int code;
        private readonly bool overwrite;

        public CopyRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.COPY;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            var values = httpListenerRequest.Headers.GetValues("Destination");
            if (null != values) {
                host = new Uri(values[0]).GetLeftPart(UriPartial.Authority);
                mDstFileName = values[0].Remove(0, host.Length);
            } else {
                throw new Exception("Bad request");
            }
            var overwriteValue = httpListenerRequest.Headers.GetValues("Overwrite");
            if (null != overwriteValue) {
                overwrite = "T" == overwriteValue[0];
            }
            Console.WriteLine("Parsed COPY REQUEST " + ToString());
        }

        internal String getFileName() {
            return mFileName;
        }

        internal void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}, mDstFileName: {0}", mDstFileName);
        }

        internal override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {
            if (mDstFileName.Equals(mFileName)) {
                code = 403;
                return;
            }
            try {
                var res = FileManager.getInstanse().copyFile(mFileName, mDstFileName, overwrite);
                code = res ? 201 : 204;
            } catch (Exception) {
                code = 412;
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
