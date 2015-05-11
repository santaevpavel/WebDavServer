using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class GetRequest : Request {

        private String mFileName;

        public GetRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.GET;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed GET REQUEST " + ToString());
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
            var response = new Response(HttpStatusCodes.SUCCESS_OK);
            FileStream file;
            try {
                file = FileManager.getInstanse().getFileForRead(mFileName);
            } catch (FileNotFoundException) {
                response = new Response(HttpStatusCodes.CLIENT_ERROR_NOT_FOUND);
                return response;
            }
            if (null == file) {
                return response;
            }
            response.setContentLength(file.Length);
            response.setData(file);
            return response;
        }

        internal override bool isAsync() {
            return false;
        }
    }
}
