using System;

namespace WebDAVServer.api.request {
    class PropFindResponse : Request{

        private String mPath;
        private int mDepth;

        public PropFindResponse() {
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
    }
}
