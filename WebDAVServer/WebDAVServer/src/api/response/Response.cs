using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NLog;
using WebDAVServer.api.helpers;

namespace WebDAVServer.api.response {
    internal class Response {

        private readonly int mCode;
        private Stream mData;
        private long contentLength;
        private readonly Dictionary<String, String> headers;
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

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
            response.ContentLength64 = contentLength;
            response.StatusCode = mCode;
            var keys = headers.Keys;
            foreach (var key in keys) {
                var val = headers[key];
                if (key.Equals("allow")) {
                    response.AddHeader(key, "OPTIONS,GET,HEAD,POST,DELETE,PROPFIND,PROPPATCH,COPY,MOVE,LOCK,UNLOCK");
                } else {
                    response.AppendHeader(key, val);
                }
            }
            if (null == mData) {
                response.OutputStream.Close();
                return;
            }
            var buffer = new byte[ProgramCostants.DEFAUT_BUFFER_SIZE];
            var progress = new ProgressView(Console.BufferWidth);
            long sum = 0;
            using (var output = response.OutputStream) {
                try {
                    while (true) {
                        var count = await mData.ReadAsync(buffer, 0, buffer.Length);
                        if (count > 0) {
                            sum += count;
                            await output.WriteAsync(buffer, 0, count);
                            if (contentLength > ProgramCostants.MIN_PROGRESS_VIEWING_SIZE) {
                                progress.drawProgress((double)sum / contentLength);
                            }
                        } else {
                            Console.WriteLine();
                            break;
                        }
                        if (sum == contentLength) {
                            break;
                        }
                    }
                } catch (HttpListenerException e) {
                    // error
                    LOGGER.Error(e.Message);
                }
                mData.Close();
            }
            response.Close();
        }
    }
}
