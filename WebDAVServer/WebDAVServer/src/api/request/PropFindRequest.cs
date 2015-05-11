﻿using System;
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

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private const String DEPTH_NAME = "depth";

        private String mPath;
        private int mDepth;

        public PropFindRequest(HttpListenerRequest httpListenerRequest)
            : base(httpListenerRequest) {
            if (null == httpListenerRequest) {
                throw new ArgumentNullException("httpListenerRequest");
            }
            requestType = RequestType.PROPFIND;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mPath = url.Remove(0, host.Length);
            var depth = httpListenerRequest.Headers.Get(DEPTH_NAME);
            mDepth = Int32.Parse(depth);
            Console.WriteLine("Parsed PROPFIND REQUEST " + ToString());
        }

        internal String getPath() {
            return mPath;
        }

        internal void setPath(String path) {
            mPath = path;
        }

        internal int getDepth() {
            return mDepth;
        }

        internal void setDepth(int depth) {
            mDepth = depth;
        }

        internal override Task doCommandAsync() {
            throw new Exception("Call sync doCommand");
        }

        internal override void doCommand() {
        }

        internal override Response getResponse() {
            Response response;
            String str;
            try {
                str = FileManager.getInstanse().getDirInfo(mPath).Exists ?
                    PropFindHelper.getFilesPropInDir(mPath, mDepth) : PropFindHelper.getFilesProp(mPath);
                response = new Response(HttpStatusCodes.SUCCESS_MULTISTATUS);
                LOGGER.Trace(str);
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

        internal override bool isAsync() {
            return false;
        }

        public override string ToString() {
            return string.Format("mDepth: {0}, mPath: {1}", mDepth, mPath);
        }
    }
}
