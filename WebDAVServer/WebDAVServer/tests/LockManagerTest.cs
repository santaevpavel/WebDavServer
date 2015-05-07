using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.request;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class LockManagerTest {

        [TestMethod]
        public void lockAndUnlock() {
            const string uri = "a.txt";
            const string token = "aaaa";
            const string user = "me";
            var info = new LockInfo(uri, LockType.WRITE, LockScope.EXCLUSIVE, 0, user, token);
            LockManager.getInstanse().lockFile(uri, info);
            Assert.AreEqual(info, LockManager.getInstanse().getLockInfo(uri));
            LockManager.getInstanse().unlock(uri, token);
            Assert.AreNotEqual(info, LockManager.getInstanse().getLockInfo(uri));
        }

        [TestMethod]
        public void lockAndBadUnlock() {
            const string uri = "a.txt";
            const string token = "aaaa";
            const string badToken = "aaaa2";
            const string user = "me";
            var info = new LockInfo(uri, LockType.WRITE, LockScope.EXCLUSIVE, 0, user, token);
            LockManager.getInstanse().lockFile(uri, info);
            Assert.AreEqual(info, LockManager.getInstanse().getLockInfo(uri));
            var unlockRes = LockManager.getInstanse().unlock(uri, badToken);
            Assert.IsFalse(unlockRes);
            Assert.AreEqual(info, LockManager.getInstanse().getLockInfo(uri));
            LockManager.getInstanse().unlock(uri, token);
            Assert.AreNotEqual(info, LockManager.getInstanse().getLockInfo(uri));
        }
    }
}
