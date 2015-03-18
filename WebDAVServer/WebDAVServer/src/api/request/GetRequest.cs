﻿using System;
using System.Net;
using System.Threading.Tasks;
using WebDAVServer.api.request.@base;
using WebDAVServer.api.response;
using WebDAVServer.file;

namespace WebDAVServer.api.request {
    internal sealed class GetRequest : Request {

        private String mFileName;

        public GetRequest(HttpListenerRequest httpListenerRequest) : base(httpListenerRequest) {
            requestType = RequestType.GET;
            var url = httpListenerRequest.Url.ToString();
            var host = httpListenerRequest.Url.GetLeftPart(UriPartial.Authority);
            mFileName = url.Remove(0, host.Length);
            Console.WriteLine("Parsed GET REQUEST " + ToString());
        }

        public String getFileName() {
            return mFileName;
        }

        public void setFileName(String fileName) {
            mFileName = fileName;
        }

        public override string ToString() {
            return string.Format("mFileName: {0}", mFileName);
        }

        public override Task doCommandAsync() {
            var task = new Task(doCommand);
            task.Start();
            return task;
        }
        private void doCommand() {

        }

        public override Task<Response> getResponse() {
            var response = new Response(200); 
            //const string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            //var buffer = Encoding.UTF8.GetBytes(responseString);
            //response.setContentLength(buffer.Length);
            var file = FileManager.getInstanse().getFile(mFileName);
            if (null != file) {
                response.setContentLength(file.Length);
                response.setData(file);
            }
            var task = new Task<Response>(() => response);
            task.Start();
            return task;
        }

       
    }
}
