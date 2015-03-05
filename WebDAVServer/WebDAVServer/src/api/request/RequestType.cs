namespace WebDAVServer.api {
    class RequestType {
        public static readonly RequestType GET;
        public static readonly RequestType PUT;
        public static readonly RequestType MLCOL;
        public static readonly RequestType COPY;
        public static readonly RequestType MOVE;
        public static readonly RequestType LOCK;
        public static readonly RequestType UNLOCK;
        public static readonly RequestType PROPFIND;
        public static readonly RequestType PROPPATCH;

        private RequestType() {
            
        }
    }
}
