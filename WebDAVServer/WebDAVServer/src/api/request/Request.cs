namespace WebDAVServer.api.request {
    abstract class Request {
        protected RequestType requestType;

        public RequestType getType() {
            return requestType;
        }

    }
}
