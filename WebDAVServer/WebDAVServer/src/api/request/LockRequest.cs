using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WebDAVServer.api.helpers;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {

    internal enum LockType {
        Write
    }

    internal enum LockScope {
        Exclusive,
        Shared
    }

    internal sealed class LockRequest : Request {

        private String _mFileName;
        private String _content;
        private readonly Stream _inputStream;
        private int _mDepth;
        private int _code;
        private LockScope _mLockScope;
        private String _mOwner;
        private LockType _mLockType;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LockRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            RequestType = RequestType.Lock;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            _mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed LOCK REQUEST " + ToString());
            _inputStream = httpListenerRequest.InputStream;
        }

        internal String GetFileName() {
            return _mFileName;
        }

        internal void SetFileName(String fileName) {
            _mFileName = fileName;
        }

        internal void SetOwner(String owner) {
            _mOwner = owner;
        }

        internal void SetDepth(int depth) {
            _mDepth = depth;
        }

        internal void SetLockScope(LockScope lockScope) {
            _mLockScope = lockScope;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", _mFileName);
        }

        internal override async Task DoCommandAsync() {
            _code = HttpStatusCodes.SuccessOk;
            var buffer = new byte[ProgramCostants.DefautBufferSize];
            var offset = 0;
            try {
                int count;
                while (0 < (count = await _inputStream.ReadAsync(buffer, offset, buffer.Length - offset))) {
                    offset += count;
                }
            } catch (HttpListenerException e) {
                Console.WriteLine(e.Message);
            }
            _content = Encoding.UTF8.GetString(buffer, 0, offset);
            Logger.Trace(_content);
            _inputStream.Close();
            if (!_content.Any()) {
                return;
            }
            LockHelper.ParseLockContent(_content, this);
            LockManager.GetInstanse()
                .LockFile(_mFileName, new LockInfo(_mFileName, _mLockType, _mLockScope, _mDepth, _mOwner, _mFileName));
        }

        internal override void DoCommand() {
            throw new Exception("Call async doCommandAsync");
        }


        internal void SetLockType(LockType lockType) {
            _mLockType = lockType;
        }

        internal override Response GetResponse() {
            var response = new Response(_code);
            var lockInfo = LockManager.GetInstanse().GetLockInfo(_mFileName);
            if (null == lockInfo) {
                return response;
            }
            var xmlContent = PropFindHelper.GetLockDiscovery(lockInfo);
            Logger.Trace("RESPONSE " + xmlContent);
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            response.SetContentLength(stream.Length);
            response.SetData(stream);
            return response;
        }

        internal override bool IsAsync() {
            return true;
        }
    }
}
