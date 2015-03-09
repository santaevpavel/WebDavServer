using System;
using System.IO;

namespace WebDAVServer.file {
    internal sealed class FileManager {

        private static FileManager instance;

        private readonly String mRoot;

        public static void init(String root) {
            instance = new FileManager(root);
        }

        public static FileManager getInstanse() {
            if (null == instance) {
                throw new Exception("You should init before get instance");
            } 
            return instance;
        }

        private FileManager(String root) {
            mRoot = root;
        }

        public FileStream getFile(String url) {
            var path = mRoot + url;
            return File.Open(path, FileMode.Open);
        }

        public FileStream createFile(String url) {
            var path = mRoot + url;
            return File.Open(path, FileMode.Create);
        }
    }
}
