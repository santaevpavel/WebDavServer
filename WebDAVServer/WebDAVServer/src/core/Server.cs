using System;
using System.Net;
using NLog;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.core {
    internal sealed class Server : IDisposable{

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly int mPort;
        private readonly HttpListener mListener = new HttpListener();
        private bool isFinished;

        internal Server(String path, int port) {
            if (null != path) {
                FileManager.init(path);
            }
            mPort = port;
        }

        internal void start() {
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            lock (mListener) {
                mListener.Prefixes.Add(String.Format("http://localhost:{0}/", mPort));
                mListener.Start();
                Console.WriteLine("Listening on port " + mPort + "...");
                while (true) {
                    var context = mListener.GetContext();
                    readAndReply(context);
                    if (isFinished) {
                        break;
                    }
                }
            }
        }

        private static async void readAndReply(HttpListenerContext context) {
            var isFailed = false;
            var request = context.Request;
            logRequest(request);
            var requestObj = await RequestParser.parseRequestAsync(request);
            if (requestObj.getRequestType().Equals(RequestType.PUT)) {
                context.Response.StatusCode = 100;
                context.Response.ContentLength64 = 0;
            }
            Response response = null;
            try {
                await requestObj.doCommandAsync();
                response = await requestObj.getResponse();
            } catch (Exception e) {
                LOGGER.Error(e.Message);
                isFailed = true;
            }
            
            if (!isFailed) {
                response.setResponse(context.Response);
            } else {
                context.Response.StatusCode = 500;
                context.Response.ContentLength64 = 0;
                context.Response.OutputStream.Close();
            }
            logResponse(context.Response);
        }

        private static void logRequest(HttpListenerRequest request) {
            if (null == request) {
                throw new ArgumentNullException("request");
            }
            LOGGER.Trace("HTTP " + request.ProtocolVersion + " REQUEST " + request.HttpMethod + " " + request.Url);
            var keys = request.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = request.Headers.GetValues(key);
                if (strings != null) {
                    LOGGER.Trace("  " + key + " -> " + strings[0]);
                }
            }
        }

        private static void logResponse(HttpListenerResponse response) {
            if (null == response) {
                throw new ArgumentNullException("response");
            }
            LOGGER.Trace("RESPONSE " + response.StatusCode + " " + response.StatusDescription);
            var keys = response.Headers.AllKeys;
            foreach (var key in keys) {
                var strings = response.Headers.GetValues(key);
                if (strings != null) {
                    LOGGER.Trace("  " + key + " -> " + strings[0]);
                }
            }
        }

        public void Dispose() {
            isFinished = true;
            mListener.Stop();
            mListener.Close();
        }
    }
}
