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
        WRITE
    }

    internal enum LockScope {
        EXCLUSIVE,
        SHARED
    }

    internal sealed class LockRequest : Request {

        private String mFileName;
        private String content;
        private readonly Stream inputStream;
        private int mDepth;
        private int code;
        private LockScope mLockScope;
        private String mOwner;
        private LockType mLockType;
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public LockRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.LOCK;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed LOCK REQUEST " + ToString());
            inputStream = httpListenerRequest.InputStream;
        }

        internal String getFileName() {
            return mFileName;
        }

        internal void setFileName(String fileName) {
            mFileName = fileName;
        }

        internal void setOwner(String owner) {
            mOwner = owner;
        }

        internal void setDepth(int depth) {
            mDepth = depth;
        }

        internal void setLockScope(LockScope lockScope) {
            mLockScope = lockScope;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        internal override async Task doCommandAsync() {
            code = HttpStatusCodes.SUCCESS_OK;
            var buffer = new byte[ProgramCostants.DEFAUT_BUFFER_SIZE];
            var offset = 0;
            try {
                int count;
                while (0 < (count = await inputStream.ReadAsync(buffer, offset, buffer.Length - offset))) {
                    offset += count;
                }
            } catch (HttpListenerException e) {
                Console.WriteLine(e.Message);
            }
            content = Encoding.UTF8.GetString(buffer, 0, offset);
            LOGGER.Trace(content);
            inputStream.Close();
            if (!content.Any()) {
                return;
            }
            LockHelper.parseLockContent(content, this);
            LockManager.getInstanse()
                .lockFile(mFileName, new LockInfo(mFileName, mLockType, mLockScope, mDepth, mOwner, mFileName));
        }

        internal override void doCommand() {
            throw new Exception("Call async doCommandAsync");
        }


        internal void setLockType(LockType lockType) {
            mLockType = lockType;
        }

        internal override Response getResponse() {
            var response = new Response(code);
            var lockInfo = LockManager.getInstanse().getLockInfo(mFileName);
            if (null == lockInfo) {
                return response;
            }
            var xmlContent = PropFindHelper.getLockDiscovery(lockInfo);
            LOGGER.Trace("RESPONSE " + xmlContent);
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            response.setContentLength(stream.Length);
            response.setData(stream);
            return response;
        }

        internal override bool isAsync() {
            return true;
        }
    }
}
