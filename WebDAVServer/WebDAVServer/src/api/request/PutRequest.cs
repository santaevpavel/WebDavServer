using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebDAVServer.api.helpers;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class PutRequest : Request {

        private String mFileName;
        private readonly Stream fileStream;
        private int code;
        private readonly long size;

        public PutRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.PUT;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            fileStream = httpListenerRequest.InputStream;
            size = httpListenerRequest.ContentLength64;
            Console.WriteLine("Parsed PUT REQUEST " + ToString());
            Console.WriteLine("SIZE = " + httpListenerRequest.ContentLength64);
        }

        internal String getFileName() {
            return mFileName;
        }

        internal void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        internal override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {
            if (FileManager.getInstanse().getFileInfo(mFileName).Exists) {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
            }
            if (FileManager.getInstanse().getDirInfo(mFileName).Exists) {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
            }
            code = 201;
            var progress = new ProgressView(Console.BufferWidth);
            long sum = 0;
            using (var file = FileManager.getInstanse().createFile(mFileName)) {
                var buffer = new byte[1024 * 1024];
                try {
                    while (true) {
                        var i = fileStream.Read(buffer, 0, buffer.Length);
                        if (i > 0) {
                            sum += i;
                            file.Write(buffer, 0, i);
                            if (size > 10 * 1024 * 1024) {
                                progress.drawProgress((double) sum/size);
                            }
                        } else {
                            Console.WriteLine();
                            break;
                        }
                    }
                } catch (HttpListenerException e) {
                    Console.WriteLine(e.Message);
                }
                fileStream.Close();
                code = 201;
            }
        }

        internal override Task<Response> getResponse() {
            var task = new Task<Response>(() => {
                Response response;
                switch (code) {
                    case 201: {
                            response = new Response(code);
                            return response;
                        }
                    case 207: {
                            String str;
                            try {
                                str = FileManager.getInstanse().getDirInfo(mFileName).Exists
                                    ? PropFindHelper.getFilesPropInDir(mFileName, 0)
                                    : PropFindHelper.getFilesProp(mFileName);
                                response = new Response(207);
                            } catch (DirectoryNotFoundException) {
                                str = "";
                                response = new Response(404);
                            }
                            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
                            response.setContentLength(stream.Length);
                            response.setData(stream);
                            response.addHeaderValue("Content-Type", "application/xml; charset=\"utf-8\"");
                            return response;
                        }
                    default:
                        throw new Exception("Bad code " + code);
                }
            });
            task.Start();
            return task;
        }
    }
}
