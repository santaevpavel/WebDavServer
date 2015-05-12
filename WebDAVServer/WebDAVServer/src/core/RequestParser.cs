using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request;
using WebDAVServer.api.request.@base;

namespace WebDAVServer.core {
    internal sealed class RequestParser {

        internal static Task<Request> ParseRequestAsync(HttpListenerRequest request) {
            var res = new Task<Request>(() => ParseRequest(request));
            res.Start();
            return res;
        }

        private static Request ParseRequest(HttpListenerRequest request) {
            var types = RequestType.Values().Where(type => type.GetHttpMethod().Equals(request.HttpMethod));
            var requestTypes = types as RequestType[] ?? types.ToArray();
            if (0 == requestTypes.Length) {
                Console.WriteLine("Getted unknown request " + request.HttpMethod);
                return new TestRequest(request);
            }
            if (null == requestTypes[0]) {
                return null;
            }
            var reqType = requestTypes[0].GetRequestType();
            var constructorInfo = reqType.GetConstructors()[0];
            if (constructorInfo == null) {
                return null;
            }
            var req = (Request)constructorInfo.Invoke(new object[] { request });
            return req;
        }
    }
}
