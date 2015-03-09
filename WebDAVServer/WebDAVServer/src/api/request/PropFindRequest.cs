using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;

namespace WebDAVServer.api.request {
    internal sealed class PropFindRequest : Request {

        private String mPath;
        private int mDepth;

        public PropFindRequest(HttpListenerRequest httpListenerRequest) : base(httpListenerRequest) {
            requestType = RequestType.PROPFIND;
        }

        public String getPath() {
            return mPath;
        }

        public void setPath(String path) {
            mPath = path;
        }

        public int getDepth() {
            return mDepth;
        }

        public void setDepth(int depth) {
            mDepth = depth;
        }

        public override Task doCommandAsync() {
            throw new NotImplementedException();
        }

        public override Task<Response> getResponse() {
            throw new NotImplementedException();
        }
    }
}
