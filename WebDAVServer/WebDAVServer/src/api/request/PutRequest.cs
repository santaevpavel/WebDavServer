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

        internal override async Task doCommandAsync() {
            if (FileManager.getInstanse().getFileInfo(mFileName).Exists) {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
            }
            if (FileManager.getInstanse().getDirInfo(mFileName).Exists) {
                FileManager.getInstanse().deleteFileOrDir(mFileName);
            }
            code = HttpStatusCodes.SUCCESS_CREATED;
            var progress = new ProgressView(Console.BufferWidth);
            long sum = 0;
            using (var file = FileManager.getInstanse().createFile(mFileName)) {
                var buffer = new byte[ProgramCostants.DEFAUT_BUFFER_SIZE];
                try {
                    while (true) {
                        var count = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                        if (count > 0) {
                            sum += count;
                            await file.WriteAsync(buffer, 0, count);
                            if (size > ProgramCostants.MIN_PROGRESS_VIEWING_SIZE) {
                                progress.drawProgress((double)sum / size);
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
                code = HttpStatusCodes.SUCCESS_CREATED;
            }
        }

        internal override void doCommand() {
            throw new Exception("Call async doCommandAsync");
        }

        internal override Response getResponse() {
                Response response;
                switch (code) {
                    case HttpStatusCodes.SUCCESS_CREATED: {
                            response = new Response(code);
                            return response;
                        }
                    case HttpStatusCodes.SUCCESS_MULTISTATUS: {
                            String str;
                            try {
                                str = FileManager.getInstanse().getDirInfo(mFileName).Exists
                                    ? PropFindHelper.getFilesPropInDir(mFileName, 0)
                                    : PropFindHelper.getFilesProp(mFileName);
                                response = new Response(HttpStatusCodes.SUCCESS_MULTISTATUS);
                            } catch (DirectoryNotFoundException) {
                                str = "";
                                response = new Response(HttpStatusCodes.CLIENT_ERROR_NOT_FOUND);
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
        }

        internal override bool isAsync() {
            return true;
        }
    }
}
