using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class MoveRequest : Request {

        private String _mFileName;
        private readonly String _mDstFileName;
        private int _code;
        private readonly bool _overwrite;

        public MoveRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Move;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            var values = httpListenerRequest.Headers.GetValues("Destination");
            if (null != values) {
                host = new Uri(values[0]).GetLeftPart(UriPartial.Authority);
                _mDstFileName = values[0].Remove(0, host.Length);
            } else {
                throw new Exception("Bad request");
            }
            var overwriteValue = httpListenerRequest.Headers.GetValues("Overwrite");
            if (null != overwriteValue) {
                _overwrite = "T" == overwriteValue[0];
            }
            Console.WriteLine("Parsed MOVE REQUEST " + ToString());
        }

        internal String GetFileName() {
            return _mFileName;
        }

        internal void SetFileName(String fileName) {
            _mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}, mDstFileName: {1}", _mFileName, _mDstFileName);
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }
        internal override void DoCommand() {
            if (_mDstFileName.Equals(_mFileName)) {
                _code = HttpStatusCodes.ClientErrorForbidden;
                return;
            }
            try {
                var res = FileManager.GetInstanse().MoveFile(_mFileName, _mDstFileName, _overwrite);
                _code = res ? HttpStatusCodes.SuccessCreated : HttpStatusCodes.SuccessNoContent;
            } catch (Exception) {
                _code = HttpStatusCodes.ClientErrorPreconditionFailed;
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
