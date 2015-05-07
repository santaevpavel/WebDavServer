using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class UnlockRequest : Request {

        private String mFileName;
        private int code;
        private readonly String token;

        public UnlockRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.UNLOCK;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            var tokens = httpListenerRequest.Headers.GetValues("Lock-Token");
            if (tokens != null && tokens.Length > 0) {
                token = tokens[0].Substring(1, tokens[0].Length - 2);
            }
            Console.WriteLine("Parsed UNLOCK REQUEST " + ToString());
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
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {
            code = 204;
            var lockInfo = LockManager.getInstanse().getLockInfo(mFileName);
            if (null != lockInfo || null != token) {
                code = LockManager.getInstanse().unlock(mFileName, token) ? 204 : 424;
            } else {
                code = 424;
            }
        }

        internal override Task<Response> getResponse() {
            var task = new Task<Response>(() => {
                var response = new Response(code);
                return response;
            });
            task.Start();
            return task;
        }


    }
}
