using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class PutRequest : Request {

        private String mFileName;
        private readonly Stream fileStream;

        public PutRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            requestType = RequestType.GET;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            fileStream = httpListenerRequest.InputStream;
            Console.WriteLine("Parsed PUT REQUEST " + ToString());
        }

        public String getFileName() {
            return mFileName;
        }

        public void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        public override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private async void doCommand() {
            using (var file = FileManager.getInstanse().createFile(mFileName)) {
                var buffer = new byte[1024];
                int i;
                while (0 < (i = await fileStream.ReadAsync(buffer, 0, buffer.Length))) {
                    await file.WriteAsync(buffer, 0, i);
                }
                file.Close();
                fileStream.Close();
            }
        }

        public override Task<Response> getResponse() {
            var response = new Response(200); 
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

       
    }
}
