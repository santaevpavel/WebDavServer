using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.request;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class LockManagerTest {

        [TestMethod]
        public void LockAndUnlock() {
            const string uri = "a.txt";
            const string token = "aaaa";
            const string user = "me";
            var info = new LockInfo(uri, LockType.Write, LockScope.Exclusive, 0, user, token);
            LockManager.GetInstanse().LockFile(uri, info);
            Assert.AreEqual(info, LockManager.GetInstanse().GetLockInfo(uri));
            LockManager.GetInstanse().Unlock(uri, token);
            Assert.AreNotEqual(info, LockManager.GetInstanse().GetLockInfo(uri));
        }

        [TestMethod]
        public void LockAndBadUnlock() {
            const string uri = "a.txt";
            const string token = "aaaa";
            const string badToken = "aaaa2";
            const string user = "me";
            var info = new LockInfo(uri, LockType.Write, LockScope.Exclusive, 0, user, token);
            LockManager.GetInstanse().LockFile(uri, info);
            Assert.AreEqual(info, LockManager.GetInstanse().GetLockInfo(uri));
            var unlockRes = LockManager.GetInstanse().Unlock(uri, badToken);
            Assert.IsFalse(unlockRes);
            Assert.AreEqual(info, LockManager.GetInstanse().GetLockInfo(uri));
            LockManager.GetInstanse().Unlock(uri, token);
            Assert.AreNotEqual(info, LockManager.GetInstanse().GetLockInfo(uri));
        }
    }
}
