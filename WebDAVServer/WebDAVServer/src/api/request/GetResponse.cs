using System;

namespace WebDAVServer.api.request {
    class GetResponse : Request {

        private String mFileName;

        public GetResponse() {
            requestType = RequestType.GET;
        }

        public String getFileName() {
            return mFileName;
        }

        public void setFileName(String fileName) {
            mFileName = fileName;
        }
    }
}
