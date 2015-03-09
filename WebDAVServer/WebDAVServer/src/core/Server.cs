using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using WebDAVServer.file;

namespace WebDAVServer.core {
    internal sealed class Server {
        private readonly int mPort;
        private readonly HttpListener mListener = new HttpListener();

        public Server(String path, int port) {
            if (null != path) {
                FileManager.init(path);
            }
            mPort = port;
        }

        static void RestartAsAdmin() {
            var startInfo = new ProcessStartInfo("WebDAVServer.exe") { Verb = "runas" };
            Process.Start(startInfo);
            Environment.Exit(0);
        }

        public void start() {
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
            var context = await mListener.GetContextAsync();
            var request = context.Request;
            var requestObj = await RequestParser.parseRequestAsync(request);
            await requestObj.doCommandAsync();
            var response = await requestObj.getResponse();
            response.setResponse(context.Response);
            lock (mListener) {
                Monitor.Pulse(mListener);
            }
        }
    }
}
