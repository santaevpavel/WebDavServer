using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class UnlockRequest : Request {

        private String _mFileName;
        private int _code;
        private readonly String _token;

        public UnlockRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Unlock;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            var tokens = httpListenerRequest.Headers.GetValues("Lock-Token");
            if (tokens != null && tokens.Length > 0) {
                _token = tokens[0].Substring(1, tokens[0].Length - 2);
            }
            Console.WriteLine("Parsed UNLOCK REQUEST " + ToString());
        }

        internal String GetFileName() {
            return _mFileName;
        }

        internal void SetFileName(String fileName) {
            _mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", _mFileName);
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void DoCommand() {
            _code = HttpStatusCodes.SuccessNoContent;
            var lockInfo = LockManager.GetInstanse().GetLockInfo(_mFileName);
            if (null != lockInfo || null != _token) {
                _code = LockManager.GetInstanse().Unlock(_mFileName, _token) ? 
                    HttpStatusCodes.SuccessNoContent : HttpStatusCodes.ClientErrorFailedDependency;
            } else {
                _code = HttpStatusCodes.ClientErrorFailedDependency;
            }
        }

        internal override Response GetResponse() {
            return new Response(_code);
        }

        internal override bool IsAsync() {
            return false;
        }
    }
}
