using System;

namespace WebDAVServer.api.request.@base {
    internal sealed class RequestType {
        public static readonly RequestType GET = new RequestType(Type.GetType("WebDAVServer.api.request.GetRequest", true), "GET");
        public static readonly RequestType PUT;
        public static readonly RequestType MLCOL;
        public static readonly RequestType COPY;
        public static readonly RequestType MOVE;
        public static readonly RequestType LOCK;
        public static readonly RequestType UNLOCK;
        public static readonly RequestType PROPFIND;
        public static readonly RequestType PROPPATCH;

        private readonly Type classType;
        private readonly String mHttpMethod;

        private RequestType(Type type, String httpMethod) {
            classType = type;
            mHttpMethod = httpMethod;
        }

        public Type getType() {
            return classType;
        }

        public String getHttpMethod() {
            return mHttpMethod;
        }

        public static RequestType[] values() {
            return new[]{GET};
        }
    }
}
