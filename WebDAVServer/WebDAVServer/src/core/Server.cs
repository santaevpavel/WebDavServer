using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebDAVServer.core {
    class Server {

        private readonly String mPath = "";
        private readonly int mPort;
        private readonly HttpListener mListener = new HttpListener();
        //private List<Client> clients = new List<Client>();

        public Server(String path, int port) {
            if (null != path) {
                mPath = path;
            }
            mPort = port;
        }

        static void RestartAsAdmin() {
            var startInfo = new ProcessStartInfo("WebDAVServer.exe") { Verb = "runas" };
            Process.Start(startInfo);
            Environment.Exit(0);
        }

        public void start() {
            //RestartAsAdmin();
           // SimpleListenerExample();
            //return;
            /*lock (mListener) {
                mListener = new TcpListener(IPAddress.Any, mPort);
            }
            mListener.Start();
            lock (mListener) {
                while (true) {
                    acceptClients();
                    Monitor.Wait(mListener);    
                }
            }*/
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            mListener.Prefixes.Add("http://localhost:27999/");
            mListener.Start();
            Console.WriteLine("Listening...");
            lock (mListener) {
                while (true) {
                    readAndReply();
                    Monitor.Wait(mListener);
                }
            }
        }

        private async void readAndReply() {
            /*using (var client = new Client(await mListener.AcceptTcpClientAsync())) {
                Console.Write("Accepting");
                clients.Add(client);
                Console.WriteLine("[Server] Client has connected");
                await client.readAndReply();
                clients.Remove(client);
                lock (mListener) {
                    Monitor.Pulse(mListener);
                }
            }*/

            var context = await mListener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            Console.WriteLine(request.HttpMethod);
            using (var output = response.OutputStream) {
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();    
            }
            lock (mListener) {
                Monitor.Pulse(mListener);
            }
        }

        private async void acceptClients() {
            /*using (var client = new Client(await mListener.AcceptTcpClientAsync())) {
                Console.Write("Accepting");
                clients.Add(client);
                Console.WriteLine("[Server] Client has connected");
                await client.readAndReply();
                clients.Remove(client);
                lock (mListener) {
                    Monitor.Pulse(mListener);
                }
            }*/
        }
    }
}
