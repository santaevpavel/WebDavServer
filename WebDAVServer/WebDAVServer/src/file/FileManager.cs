using System;
using System.IO;

namespace WebDAVServer.file {
    internal sealed class FileManager {

        private static FileManager instance;

        private readonly String mRoot;

        internal static void init(String root) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            instance = new FileManager(root);
        }

        internal static FileManager getInstanse() {
            if (null == instance) {
                throw new Exception("You should init before get instance");
            } 
            return instance;
        }

        private FileManager(String root) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            mRoot = root;
        }

        internal FileStream getFile(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = mRoot + url;
            return File.Open(path, FileMode.Open);
        }

        internal FileStream getFileForRead(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = mRoot + url;
            return File.OpenRead(path);
        }

        internal FileInfo getFileInfo(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = mRoot + url;
            return new FileInfo(path);
        }

        internal DirectoryInfo getDirInfo(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = mRoot + url;
            return new DirectoryInfo(path);
        }

        internal FileStream createFile(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = mRoot + url;
            return File.Open(path, FileMode.Create);
        }

        internal bool copyFile(String src, String dst, bool overwrite) {
            if (null == src) {
                throw new ArgumentNullException("src");
            }
            if (null == dst) {
                throw new ArgumentNullException("dst");
            }
            var res = !getFileInfo(dst).Exists;
            File.Copy(mRoot + src, mRoot + dst, overwrite);
            return res;
        }

        internal bool moveFile(String src, String dst, bool overwrite) {
            if (null == src) {
                throw new ArgumentNullException("src");
            }
            if (null == dst) {
                throw new ArgumentNullException("dst");
            }
            if (getFileInfo(src).Exists) {

                var res = !getFileInfo(dst).Exists;
                if (overwrite) {
                    File.Delete(mRoot + dst);
                }
                File.Move(mRoot + src, mRoot + dst);
                return res;
            }
            if (!getDirInfo(src).Exists) {
                throw new FileNotFoundException(src);
            }
            var dirRes = !getDirInfo(dst).Exists;
            if (overwrite) {
                Directory.Delete(mRoot + dst);
            }
            Directory.Move(mRoot + src, mRoot + dst);
            return dirRes;
        }

        internal void deleteFileOrDir(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
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
