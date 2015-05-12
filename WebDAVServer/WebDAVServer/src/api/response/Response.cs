using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NLog;
using WebDAVServer.api.helpers;

namespace WebDAVServer.api.response {
    internal class Response {

        private readonly int _mCode;
        private Stream _mData;
        private long _contentLength;
        private readonly Dictionary<String, String> _headers;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal Response(int code) {
            _mCode = code;
            _headers = new Dictionary<string, string>();
        }

        internal void SetData(Stream data) {
            _mData = data;
        }

        internal void SetContentLength(long length) {
            _contentLength = length;
        }

        internal void AddHeaderValue(String key, String value) {
            _headers.Add(key, value);
        }
        
        internal virtual async void SetResponse(HttpListenerResponse response) {
            if (null == response) {
                throw new ArgumentNullException("response");
            }
            response.ContentLength64 = _contentLength;
            response.StatusCode = _mCode;
            var keys = _headers.Keys;
            foreach (var key in keys) {
                var val = _headers[key];
                if (key.Equals("allow")) {
                    response.AddHeader(key, "OPTIONS,GET,HEAD,POST,DELETE,PROPFIND,PROPPATCH,COPY,MOVE,LOCK,UNLOCK");
                } else {
                    response.AppendHeader(key, val);
                }
            }
            if (null == _mData) {
                response.OutputStream.Close();
                return;
            }
            var buffer = new byte[ProgramCostants.DefautBufferSize];
            var progress = new ProgressView(Console.BufferWidth);
            long sum = 0;
            using (var output = response.OutputStream) {
                try {
                    while (true) {
                        var count = await _mData.ReadAsync(buffer, 0, buffer.Length);
                        if (count > 0) {
                            sum += count;
                            await output.WriteAsync(buffer, 0, count);
                            if (_contentLength > ProgramCostants.MinProgressViewingSize) {
                                progress.DrawProgress((double)sum / _contentLength);
                            }
                        } else {
                            Console.WriteLine();
                            break;
                        }
                        if (sum == _contentLength) {
                            break;
                        }
                    }
                } catch (HttpListenerException e) {
                    // error
                    Logger.Error(e.Message);
                }
                _mData.Close();
            }
            response.Close();
        }
    }
}
