using System;

namespace WebDAVServer.api.request.@base {
    internal sealed class RequestType {
        public static readonly RequestType GET = new RequestType(Type.GetType("WebDAVServer.api.request.GetRequest", true), "GET");
        public static readonly RequestType PUT = new RequestType(Type.GetType("WebDAVServer.api.request.PutRequest", true), "PUT");
        public static readonly RequestType MKCOL = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "MKCOL");
        public static readonly RequestType COPY = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "COPY");
        public static readonly RequestType MOVE = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "MOVE");
        public static readonly RequestType LOCK = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "LOCK");
        public static readonly RequestType UNLOCK = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "UNLOCK");
        public static readonly RequestType PROPFIND = new RequestType(Type.GetType("WebDAVServer.api.request.PropFindRequest", true), "PROPFIND");
        public static readonly RequestType PROPPATCH = new RequestType(Type.GetType("WebDAVServer.api.request.TestRequest", true), "PROPPATCH");
        public static readonly RequestType OPTIONS = new RequestType(Type.GetType("WebDAVServer.api.request.OptionsRequest", true), "OPTIONS");

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
            return new[]{GET, PROPFIND, PUT};
        }
    }
}
