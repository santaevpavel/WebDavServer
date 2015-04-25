using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WebDAVServer.api.response {
    internal class Response {

        private readonly int mCode;
        private Stream mData;
        private long contentLength;
        private readonly Dictionary<String, String> headers;

        internal Response(int code) {
            mCode = code;
            headers = new Dictionary<string, string>();
        }

        internal void setData(Stream data) {
            mData = data;
        }

        internal void setContentLength(long length) {
            contentLength = length;
        }

        internal void addHeaderValue(String key, String value) {
            headers.Add(key, value);
        }
        
        internal virtual async void setResponse(HttpListenerResponse response) {
            if (null == response) {
                throw new ArgumentNullException("response");
            }
            response.StatusCode = mCode;
            response.ContentLength64 = contentLength;

            var keys = headers.Keys;
            foreach (var key in keys) {
                var val = headers[key];
                response.AppendHeader(key, val);
            }
            if (null == mData) {
                response.OutputStream.Close();
                return;
            }
            var buffer = new byte[1024];
            using (var output = response.OutputStream) {
                while (true) {
                    var i = await mData.ReadAsync(buffer, 0, buffer.Length);
                    if (i > 0) {
                        await output.WriteAsync(buffer, 0, i);
                    } else {
                        break;
                    }
                }
                mData.Close();
            }
        }
    }
}
