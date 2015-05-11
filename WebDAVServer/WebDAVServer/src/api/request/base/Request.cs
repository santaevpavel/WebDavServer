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

        internal RequestType getRequestType() {
            return requestType;
        }

        internal abstract Task doCommandAsync();

        internal abstract void doCommand();

        internal abstract Response getResponse();

        internal abstract bool isAsync();

    }
}
