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
            throw new Exception("Call sync doCommand");
        }

        internal override void doCommand() {
            try {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
                code = HttpStatusCodes.SUCCESS_NO_CONTENT;
            } catch (FileNotFoundException) {
                code = HttpStatusCodes.CLIENT_ERROR_NOT_FOUND;
            } catch (DirectoryNotFoundException) {
                code = HttpStatusCodes.CLIENT_ERROR_NOT_FOUND;
            } catch (UnauthorizedAccessException) {
                code = HttpStatusCodes.CLIENT_ERROR_LOCKED;
            }
        }

        internal override Response getResponse() {
            return new Response(code);
        }

        internal override bool isAsync() {
            return false;
        }
    }
}
