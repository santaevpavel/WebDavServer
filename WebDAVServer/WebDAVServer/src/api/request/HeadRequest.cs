using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class HeadRequest : Request {

        private String _mFileName;

        public HeadRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Head;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed HEAD REQUEST " + ToString());
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
        }


        internal override Response GetResponse() {
            if (FileManager.GetInstanse().GetDirInfo(_mFileName).Exists
                || FileManager.GetInstanse().GetFileInfo(_mFileName).Exists) {
                return new Response(HttpStatusCodes.SuccessNoContent);
            }
            return new Response(HttpStatusCodes.ClientErrorNotFound);
        }

        internal override bool IsAsync() {
            return false;
        }
    }
}
