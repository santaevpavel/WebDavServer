using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class MkColRequest : Request {

        private String mDirName;
        private int code;

        public MkColRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.MKCOL;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mDirName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed MKCOL REQUEST " + ToString());
        }

        internal String getDirName() {
            return mDirName;
        }

        internal void setDirName(String fileName) {
            mDirName = fileName;
        }

        public override string ToString() {
            return string.Format("mDirName: {0}", mDirName);
        }

        internal override Task doCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void doCommand() {
            var dir = FileManager.getInstanse().getDirInfo(mDirName);
            if (dir.Exists) {
                code = HttpStatusCodes.SUCCESS_CREATED;
                return;
            }
            try {
                dir.Create();
            } catch (Exception) {
                code = HttpStatusCodes.CLIENT_ERROR_METHOD_NOT_ALLOWED;
                return;
            }
            code = HttpStatusCodes.SUCCESS_CREATED;
        }

        internal override Response getResponse() {
            return new Response(code);
        }

        internal override bool isAsync() {
            return false;
        }
    }
}
