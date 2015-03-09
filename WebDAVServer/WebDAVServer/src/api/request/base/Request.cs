using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request.@base {
    internal abstract class Request {
        protected RequestType requestType;
        protected HttpListenerRequest mRequest;

        protected Request(HttpListenerRequest httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            mRequest = httpListenerRequest;
        }

        public RequestType getRequestType() {
            return requestType;
        }

        public abstract Task doCommandAsync();

        public abstract Task<Response> getResponse();
    }
}
