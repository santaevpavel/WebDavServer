using System;

namespace WebDAVServer.api.request.@base {
    internal sealed class RequestType {
        internal static readonly RequestType GET = new RequestType(Type.GetType("WebDAVServer.api.request.GetRequest", true), "GET");
        internal static readonly RequestType PUT = new RequestType(Type.GetType("WebDAVServer.api.request.PutRequest", true), "PUT");
        internal static readonly RequestType MKCOL = new RequestType(Type.GetType("WebDAVServer.api.request.MkColRequest", true), "MKCOL");
        internal static readonly RequestType COPY = new RequestType(Type.GetType("WebDAVServer.api.request.CopyRequest", true), "COPY");
        internal static readonly RequestType MOVE = new RequestType(Type.GetType("WebDAVServer.api.request.MoveRequest", true), "MOVE");
        internal static readonly RequestType LOCK = new RequestType(Type.GetType("WebDAVServer.api.request.LockRequest", true), "LOCK");
        internal static readonly RequestType UNLOCK = new RequestType(Type.GetType("WebDAVServer.api.request.UnlockRequest", true), "UNLOCK");
        internal static readonly RequestType PROPFIND = new RequestType(Type.GetType("WebDAVServer.api.request.PropFindRequest", true), "PROPFIND");
        internal static readonly RequestType PROPPATCH = new RequestType(Type.GetType("WebDAVServer.api.request.PropPatchRequest", true), "PROPPATCH");
        internal static readonly RequestType OPTIONS = new RequestType(Type.GetType("WebDAVServer.api.request.OptionsRequest", true), "OPTIONS");
        internal static readonly RequestType DELETE = new RequestType(Type.GetType("WebDAVServer.api.request.DeleteRequest", true), "DELETE");

        private readonly Type classType;
        private readonly String mHttpMethod;

        private RequestType(Type type, String httpMethod) {
            classType = type;
            mHttpMethod = httpMethod;
        }

        internal Type getType() {
            return classType;
        }

        internal String getHttpMethod() {
            return mHttpMethod;
        }

        internal static RequestType[] values() {
            return new[]{GET, PROPFIND, PUT, MKCOL, OPTIONS, COPY, LOCK, DELETE, MOVE, PROPPATCH, UNLOCK};
        }
    }
}
