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
            throw new Exception("Call sync doCommand");
        }

        internal override void doCommand() {
        }


        internal override Response getResponse() {
            if (FileManager.getInstanse().getDirInfo(mFileName).Exists
                || FileManager.getInstanse().getFileInfo(mFileName).Exists) {
                return new Response(HttpStatusCodes.SUCCESS_NO_CONTENT);
            }
            return new Response(HttpStatusCodes.CLIENT_ERROR_NOT_FOUND);
        }

        internal override bool isAsync() {
            return false;
        }
    }
}
