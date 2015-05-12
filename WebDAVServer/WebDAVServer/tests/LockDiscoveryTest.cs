using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.helpers;
using WebDAVServer.api.request;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class LockDiscoveryTest {

        [TestMethod]
        public void LockRequest() {
            const string uri = "a.txt";
            const string token = "aaaa";
            const string user = "me";
            var info = new LockInfo(uri, LockType.Write, LockScope.Exclusive, 0, user, token);
            var res = PropFindHelper.GetLockDiscovery(info);
            var pos = res.IndexOf("<d:href>" + token +"</d:href>", StringComparison.Ordinal);
            Assert.IsTrue(pos > 0);
            pos = res.IndexOf("<d:depth>" + 0 + "</d:depth>", StringComparison.Ordinal);
            Assert.IsTrue(pos > 0);
            pos = res.IndexOf("<d:href>" + user + "</d:href>", StringComparison.Ordinal);
            Assert.IsTrue(pos > 0);
        }

    }
}
