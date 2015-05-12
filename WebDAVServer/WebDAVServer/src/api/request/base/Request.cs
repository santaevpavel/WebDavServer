using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request.@base {
    internal abstract class Request {
        protected RequestType RequestType;
        protected HttpListenerRequest MRequest;

        protected Request(HttpListenerRequest httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            MRequest = httpListenerRequest;
        }

        internal RequestType GetRequestType() {
            return RequestType;
        }

        internal abstract Task DoCommandAsync();

        internal abstract void DoCommand();

        internal abstract Response GetResponse();

        internal abstract bool IsAsync();

    }
}
