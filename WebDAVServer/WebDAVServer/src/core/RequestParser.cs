using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request;
using WebDAVServer.api.request.@base;

namespace WebDAVServer.core {
    internal sealed class RequestParser {

        public static Task<Request> parseRequestAsync(HttpListenerRequest request) {
            var res = new Task<Request>(() => parseRequest(request));
            res.Start();
            return res;
        }

        private static Request parseRequest(HttpListenerRequest request) {
            var types = RequestType.values().Where(type => type.getHttpMethod().Equals(request.HttpMethod));
            var requestTypes = types as RequestType[] ?? types.ToArray();
            if (0 == requestTypes.Length) {
                Console.WriteLine("Getted unknown request " + request.HttpMethod);
                return new TestRequest(request);
            }
            if (null != requestTypes[0]) {
                var type = requestTypes[0].getType();
                var constructorInfo = type.GetConstructors()[0];
                if (constructorInfo != null) {
                    var req = (Request)constructorInfo.Invoke(new object[] { request });
                    return req;
                }
            }
            return null;
        }
    }
}
