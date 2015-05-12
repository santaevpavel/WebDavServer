using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class GetRequest : Request {

        private String _mFileName;

        public GetRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Get;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed GET REQUEST " + ToString());
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
            var response = new Response(HttpStatusCodes.SuccessOk);
            FileStream file;
            try {
                file = FileManager.GetInstanse().GetFileForRead(_mFileName);
            } catch (FileNotFoundException) {
                response = new Response(HttpStatusCodes.ClientErrorNotFound);
                return response;
            }
            if (null == file) {
                return response;
            }
            response.SetContentLength(file.Length);
            response.SetData(file);
            return response;
        }

        internal override bool IsAsync() {
            return false;
        }
    }
}
