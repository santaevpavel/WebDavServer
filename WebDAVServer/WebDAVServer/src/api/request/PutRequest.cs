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

        private String _mFileName;
        private readonly Stream _fileStream;
        private int _code;
        private readonly long _size;

        public PutRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Put;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            _fileStream = httpListenerRequest.InputStream;
            _size = httpListenerRequest.ContentLength64;
            Console.WriteLine("Parsed PUT REQUEST " + ToString());
            Console.WriteLine("SIZE = " + httpListenerRequest.ContentLength64);
        }

        internal String GetFileName() {
            return _mFileName;
        }

        internal void SetFileName(String fileName) {
            _mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", _mFileName);
        }

        internal override async Task DoCommandAsync() {
            if (FileManager.GetInstanse().GetFileInfo(_mFileName).Exists) {
                FileManager.GetInstanse().DeleteFileOrDir(_mFileName);
            }
            if (FileManager.GetInstanse().GetDirInfo(_mFileName).Exists) {
                FileManager.GetInstanse().DeleteFileOrDir(_mFileName);
            }
            _code = HttpStatusCodes.SuccessCreated;
            var progress = new ProgressView(Console.BufferWidth);
            long sum = 0;
            using (var file = FileManager.GetInstanse().CreateFile(_mFileName)) {
                var buffer = new byte[ProgramCostants.DefautBufferSize];
                try {
                    while (true) {
                        var count = await _fileStream.ReadAsync(buffer, 0, buffer.Length);
                        if (count > 0) {
                            sum += count;
                            await file.WriteAsync(buffer, 0, count);
                            if (_size > ProgramCostants.MinProgressViewingSize) {
                                progress.DrawProgress((double)sum / _size);
                            }
                        } else {
                            Console.WriteLine();
                            break;
                        }
                    }
                } catch (HttpListenerException e) {
                    Console.WriteLine(e.Message);
                }
                _fileStream.Close();
                _code = HttpStatusCodes.SuccessCreated;
            }
        }

        internal override void DoCommand() {
            throw new Exception("Call async doCommandAsync");
        }

        internal override Response GetResponse() {
                Response response;
                switch (_code) {
                    case HttpStatusCodes.SuccessCreated: {
                            response = new Response(_code);
                            return response;
                        }
                    case HttpStatusCodes.SuccessMultistatus: {
                            String str;
                            try {
                                str = FileManager.GetInstanse().GetDirInfo(_mFileName).Exists
                                    ? PropFindHelper.GetFilesPropInDir(_mFileName, 0)
                                    : PropFindHelper.GetFilesProp(_mFileName);
                                response = new Response(HttpStatusCodes.SuccessMultistatus);
                            } catch (DirectoryNotFoundException) {
                                str = "";
                                response = new Response(HttpStatusCodes.ClientErrorNotFound);
                            }
                            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
                            response.SetContentLength(stream.Length);
                            response.SetData(stream);
                            response.AddHeaderValue("Content-Type", "application/xml; charset=\"utf-8\"");
                            return response;
                        }
                    default:
                        throw new Exception("Bad code " + _code);
                }
        }

        internal override bool IsAsync() {
            return true;
        }
    }
}
