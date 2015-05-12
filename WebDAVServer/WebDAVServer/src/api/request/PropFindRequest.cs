using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WebDAVServer.api.helpers;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class PropFindRequest : Request {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const String DepthName = "depth";

        private String _mPath;
        private int _mDepth;

        public PropFindRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Propfind;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mPath = url.Remove(0, host.Length);
            var depth = httpListenerRequest.Headers.Get(DepthName);
            _mDepth = Int32.Parse(depth);
            Console.WriteLine("Parsed PROPFIND REQUEST " + ToString());
        }

        internal String GetPath() {
            return _mPath;
        }

        internal void SetPath(String path) {
            _mPath = path;
        }

        internal int GetDepth() {
            return _mDepth;
        }

        internal void SetDepth(int depth) {
            _mDepth = depth;
        }

        internal override Task DoCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void DoCommand() {
        }

        internal override Response GetResponse() {
            Response response;
            String str;
            try {
                str = FileManager.GetInstanse().GetDirInfo(_mPath).Exists ?
                    PropFindHelper.GetFilesPropInDir(_mPath, _mDepth) : PropFindHelper.GetFilesProp(_mPath);
                response = new Response(HttpStatusCodes.SuccessMultistatus);
                Logger.Trace(str);
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

        internal override bool IsAsync() {
            return false;
        }

        public override string ToString() {
            return string.Format("mDepth: {0}, mPath: {1}", _mDepth, _mPath);
        }
    }
}
