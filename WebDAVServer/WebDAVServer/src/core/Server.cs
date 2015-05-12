using System;
using System.Net;
using NLog;
using WebDAVServer.api.request;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.core {
    internal sealed class Server : IDisposable {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _mPort;
        private readonly HttpListener _mListener = new HttpListener();
        private bool _isFinished;

        internal Server(String path, int port) {
            if (null != path) {
                FileManager.Init(path);
            }
            _mPort = port;
        }

        internal void Start() {
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            _mListener.Prefixes.Add(String.Format("http://localhost:{0}/", _mPort));
            _mListener.Start();
            Console.WriteLine("Listening on port " + _mPort + "...");
            while (true) {
                var context = _mListener.GetContext();
                ReadAndReply(context);
                if (_isFinished) {
                    break;
                }
            }
        }

        private static async void ReadAndReply(HttpListenerContext context) {
            var isFailed = false;
            var request = context.Request;
            LogRequest(request);
            var requestObj = await RequestParser.ParseRequestAsync(request) ?? new TestRequest(request);
            if (requestObj.GetRequestType().Equals(RequestType.Put)) {
                context.Response.StatusCode = HttpStatusCodes.InfoContinue;
                context.Response.ContentLength64 = 0;
            }
            Response response = null;
            try {
                if (requestObj.IsAsync()) {
                    await requestObj.DoCommandAsync();
                } else {
                    requestObj.DoCommand();
                }
                response = requestObj.GetResponse();
            } catch (Exception e) {
                Logger.Error(e.Message);
                isFailed = true;
            }

            if (!isFailed) {
                response.SetResponse(context.Response);
            } else {
                context.Response.StatusCode = HttpStatusCodes.ServerErrorInternalError;
                context.Response.ContentLength64 = 0;
                context.Response.OutputStream.Close();
            }
            LogResponse(context.Response);
        }

        private static void LogRequest(HttpListenerRequest request) {
            if (null == request) {
                throw new ArgumentNullException("request");
            }
            Logger.Trace("HTTP " + request.ProtocolVersion + " REQUEST " + request.HttpMethod + " " + request.Url);
            var keys = request.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = request.Headers.GetValues(key);
                if (strings != null) {
                    Logger.Trace("  " + key + " -> " + strings[0]);
                }
            }
        }

        private static void LogResponse(HttpListenerResponse response) {
            if (null == response) {
                throw new ArgumentNullException("response");
            }
            Logger.Trace("RESPONSE " + response.StatusCode + " " + response.StatusDescription);
            var keys = response.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = response.Headers.GetValues(key);
                if (strings != null) {
                    Logger.Trace("  " + key + " -> " + strings[0]);
                }
            }
        }

        public void Dispose() {
            _isFinished = true;
            _mListener.Stop();
            _mListener.Close();
        }
    }
}
