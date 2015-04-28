using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using NLog;
using WebDAVServer.api.request.@base;
using WebDAVServer.file;

namespace WebDAVServer.core {
    internal sealed class Server {

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly int mPort;
        private readonly HttpListener mListener = new HttpListener();

        internal Server(String path, int port) {
            if (null != path) {
                FileManager.init(path);
            }
            mPort = port;
        }

        internal static void restartAsAdmin() {
            var startInfo = new ProcessStartInfo("WebDAVServer.exe") { Verb = "runas" };
            Process.Start(startInfo);
            Environment.Exit(0);
        }

        internal void start() {
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            lock (mListener) {
                mListener.Prefixes.Add(String.Format("http://localhost:{0}/", mPort));
                mListener.Start();
                Console.WriteLine("Listening...");
                while (true) {
                    readAndReply();
                    Monitor.Wait(mListener);
                }
            }
        }

        private async void readAndReply() {
            var isFailed = false;
            var context = await mListener.GetContextAsync();
            var request = context.Request;
            logRequest(request);
            var requestObj = await RequestParser.parseRequestAsync(request);
            if (requestObj.getRequestType().Equals(RequestType.PUT)) {
                context.Response.StatusCode = 100;
                context.Response.ContentLength64 = 0;
            }
            try {
                await requestObj.doCommandAsync();
            } catch (Exception e) {
                LOGGER.Error(e.Message);
                isFailed = true;
            }
            var response = await requestObj.getResponse();
            if (!isFailed) {
                response.setResponse(context.Response);
            } else {
                context.Response.StatusCode = 500;
                context.Response.ContentLength64 = 0;
                context.Response.OutputStream.Close();
            }
            logResponse(context.Response);
            //context.Response.Close();
            lock (mListener) {
                Monitor.Pulse(mListener);
            }
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
    }
}
