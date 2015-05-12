using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class MkColRequest : Request {

        private String _mDirName;
        private int _code;

        public MkColRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Mkcol;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mDirName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed MKCOL REQUEST " + ToString());
        }

        internal String GetDirName() {
            return _mDirName;
        }

        internal void SetDirName(String fileName) {
            _mDirName = fileName;
        }

        public override string ToString() {
            return string.Format("mDirName: {0}", _mDirName);
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void DoCommand() {
            var dir = FileManager.GetInstanse().GetDirInfo(_mDirName);
            if (dir.Exists) {
                _code = HttpStatusCodes.SuccessCreated;
                return;
            }
            try {
                dir.Create();
            } catch (Exception) {
                _code = HttpStatusCodes.ClientErrorMethodNotAllowed;
                return;
            }
            _code = HttpStatusCodes.SuccessCreated;
        }

        internal override Response GetResponse() {
            return new Response(_code);
        }

        internal override bool IsAsync() {
            return false;
        }
    }
}
