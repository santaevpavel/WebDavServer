using System;
using System.IO;

namespace WebDAVServer.file {
    internal sealed class FileManager {

        private static FileManager _instance;

        private readonly String _mRoot;

        internal static void Init(String root) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            _instance = new FileManager(root);
        }

        internal static FileManager GetInstanse() {
            if (null == _instance) {
                throw new Exception("You should init before get instance");
            } 
            return _instance;
        }

        private FileManager(String root) {
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            _mRoot = root;
        }

        internal FileStream GetFile(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            return File.Open(path, FileMode.Open);
        }

        internal FileStream GetFileForRead(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            return File.OpenRead(path);
        }

        internal FileInfo GetFileInfo(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            return new FileInfo(path);
        }

        internal DirectoryInfo GetDirInfo(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            return new DirectoryInfo(path);
        }

        internal FileStream CreateFile(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            return File.Open(path, FileMode.Create);
        }

        internal bool CopyFile(String src, String dst, bool overwrite) {
            if (null == src) {
                throw new ArgumentNullException("src");
            }
            if (null == dst) {
                throw new ArgumentNullException("dst");
            }
            var res = !GetFileInfo(dst).Exists;
            File.Copy(_mRoot + src, _mRoot + dst, overwrite);
            return res;
        }

        internal bool MoveFile(String src, String dst, bool overwrite) {
            if (null == src) {
                throw new ArgumentNullException("src");
            }
            if (null == dst) {
                throw new ArgumentNullException("dst");
            }
            if (GetFileInfo(src).Exists) {

                var res = !GetFileInfo(dst).Exists;
                if (overwrite) {
                    File.Delete(_mRoot + dst);
                }
                File.Move(_mRoot + src, _mRoot + dst);
                return res;
            }
            if (!GetDirInfo(src).Exists) {
                throw new FileNotFoundException(src);
            }
            var dirRes = !GetDirInfo(dst).Exists;
            if (overwrite) {
                Directory.Delete(_mRoot + dst);
            }
            Directory.Move(_mRoot + src, _mRoot + dst);
            return dirRes;
        }

        internal void DeleteFileOrDir(String url) {
            if (null == url) {
                throw new ArgumentNullException("url");
            }
            var path = _mRoot + url;
            var file = new FileInfo(path);
            if (file.Exists) {
                File.Delete(path);
            } else {
                Directory.Delete(path);
            }
            
        }
        
    }
}
