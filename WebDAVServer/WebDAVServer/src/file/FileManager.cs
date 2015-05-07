using System;
using System.IO;

namespace WebDAVServer.file {
    internal sealed class FileManager {

        private static FileManager instance;

        private readonly String mRoot;

        internal static void init(String root) {
            instance = new FileManager(root);
        }

        internal static FileManager getInstanse() {
            if (null == instance) {
                throw new Exception("You should init before get instance");
            } 
            return instance;
        }

        private FileManager(String root) {
            mRoot = root;
        }

        internal FileStream getFile(String url) {
            var path = mRoot + url;
            return File.Open(path, FileMode.Open);
        }

        internal FileStream getFileForRead(String url) {
            var path = mRoot + url;
            return File.OpenRead(path);
        }

        internal FileInfo getFileInfo(String url) {
            var path = mRoot + url;
            return new FileInfo(path);
        }

        internal DirectoryInfo getDirInfo(String url) {
            var path = mRoot + url;
            return new DirectoryInfo(path);
        }

        internal FileStream createFile(String url) {
            var path = mRoot + url;
            return File.Open(path, FileMode.Create);
        }

        internal bool copyFile(String src, String dst, bool overwrite) {
            var res = !getFileInfo(dst).Exists;
            File.Copy(mRoot + src, mRoot + dst, overwrite);
            return res;
        }

        internal bool moveFile(String src, String dst, bool overwrite) {
            var res = !getFileInfo(dst).Exists;
            if (overwrite) {
                File.Delete(mRoot + dst);
            }
            File.Move(mRoot + src, mRoot + dst);
            return res;
        }

        internal void deleteFileOrDir(String url) {
            var path = mRoot + url;
            var file = new FileInfo(path);
            if (file.Exists) {
                File.Delete(path);
            } else {
                Directory.Delete(path);
            }
            
        }
        
    }
}
