using System;

namespace WebDAVServer.api.request.@base {
    internal sealed class RequestType {
        internal static readonly RequestType Get = new RequestType(Type.GetType("WebDAVServer.api.request.GetRequest", true), "GET");
        internal static readonly RequestType Put = new RequestType(Type.GetType("WebDAVServer.api.request.PutRequest", true), "PUT");
        internal static readonly RequestType Mkcol = new RequestType(Type.GetType("WebDAVServer.api.request.MkColRequest", true), "MKCOL");
        internal static readonly RequestType Copy = new RequestType(Type.GetType("WebDAVServer.api.request.CopyRequest", true), "COPY");
        internal static readonly RequestType Move = new RequestType(Type.GetType("WebDAVServer.api.request.MoveRequest", true), "MOVE");
        internal static readonly RequestType Lock = new RequestType(Type.GetType("WebDAVServer.api.request.LockRequest", true), "LOCK");
        internal static readonly RequestType Unlock = new RequestType(Type.GetType("WebDAVServer.api.request.UnlockRequest", true), "UNLOCK");
        internal static readonly RequestType Propfind = new RequestType(Type.GetType("WebDAVServer.api.request.PropFindRequest", true), "PROPFIND");
        internal static readonly RequestType Proppatch = new RequestType(Type.GetType("WebDAVServer.api.request.PropPatchRequest", true), "PROPPATCH");
        internal static readonly RequestType Options = new RequestType(Type.GetType("WebDAVServer.api.request.OptionsRequest", true), "OPTIONS");
        internal static readonly RequestType Delete = new RequestType(Type.GetType("WebDAVServer.api.request.DeleteRequest", true), "DELETE");
        internal static readonly RequestType Head = new RequestType(Type.GetType("WebDAVServer.api.request.HeadRequest", true), "HEAD");

        private readonly Type _classType;
        private readonly String _mHttpMethod;

        private RequestType(Type type, String httpMethod) {
            _classType = type;
            _mHttpMethod = httpMethod;
        }

        internal Type GetRequestType() {
            return _classType;
        }

        internal String GetHttpMethod() {
            return _mHttpMethod;
        }

        internal static RequestType[] Values() {
            return new[]{Get, Propfind, Put, Mkcol, Options, Copy, Lock, Delete, Move, Proppatch, Unlock, Head};
        }
    }
}
