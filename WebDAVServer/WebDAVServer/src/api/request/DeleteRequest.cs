using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class DeleteRequest : Request {

        private String _mFileName;
        private int _code;

        public DeleteRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Delete;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed DELETE REQUEST " + ToString());
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
            try {
                FileManager.GetInstanse().DeleteFileOrDir(_mFileName);
                _code = HttpStatusCodes.SuccessNoContent;
            } catch (FileNotFoundException) {
                _code = HttpStatusCodes.ClientErrorNotFound;
            } catch (DirectoryNotFoundException) {
                _code = HttpStatusCodes.ClientErrorNotFound;
            } catch (UnauthorizedAccessException) {
                _code = HttpStatusCodes.ClientErrorLocked;
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
