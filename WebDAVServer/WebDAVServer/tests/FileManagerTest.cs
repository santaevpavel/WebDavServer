using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.file;

namespace WebDAVServer.tests {
    [TestClass]
    public class FileManagerTest {

        [TestMethod]
        public void getFile() {
            FileManager.init("");
            const string fileName = "test.txt";
            var file = File.Create(fileName);
            file.Close();
            var info = FileManager.getInstanse().getFileInfo(fileName);
            Assert.AreEqual(info.Exists, true);
            Assert.AreEqual(info.Name, fileName);
            Assert.AreEqual(info.Length, 0);
        }

        [TestMethod]
        public void getDir() {
            FileManager.init("");
            const string fileName = "test";
            Directory.CreateDirectory(fileName);
            var info = FileManager.getInstanse().getDirInfo(fileName);
            Assert.AreEqual(info.Exists, true);
            Assert.AreEqual(info.Name, fileName);
        }
    }
}
