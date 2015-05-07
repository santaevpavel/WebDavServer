using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.helpers;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class PropFindHelperTest {

        [TestMethod]
        public void getProp() {
            FileManager.init("");
            const string fileName = "test.txt";
            var file = File.Create(fileName);
            file.Close();
            var curRes = PropFindHelper.getFilesProp(fileName);
            //<d:status>HTTP/1.1 200 OK</d:status>
            var pos = curRes.IndexOf("<d:status>HTTP/1.1 200 OK</d:status>", StringComparison.Ordinal);
            Assert.AreEqual(true, pos > 0);
        }


    }
}
