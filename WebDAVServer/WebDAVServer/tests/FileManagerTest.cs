using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class FileManagerTest {

        [TestMethod]
        public void GetFile() {
            FileManager.Init("");
            const string fileName = "test.txt";
            var file = File.Create(fileName);
            file.Close();
            var info = FileManager.GetInstanse().GetFileInfo(fileName);
            Assert.AreEqual(info.Exists, true);
            Assert.AreEqual(info.Name, fileName);
            Assert.AreEqual(info.Length, 0);
        }

        [TestMethod]
        public void GetDir() {
            FileManager.Init("");
            const string fileName = "test";
            Directory.CreateDirectory(fileName);
            var info = FileManager.GetInstanse().GetDirInfo(fileName);
            Assert.AreEqual(info.Exists, true);
            Assert.AreEqual(info.Name, fileName);
        }
    }
}
