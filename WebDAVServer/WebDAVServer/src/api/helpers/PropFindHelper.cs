using System;
using System.IO;
using System.Xml.Linq;
using WebDAVServer.api.request;
using WebDAVServer.file;

namespace WebDAVServer.api.helpers {

    
    internal sealed class PropFindHelper {

        private static readonly XNamespace XmlNamespace = "DAV:";
        private static readonly XAttribute NamespaceAttribute = new XAttribute(XNamespace.Xmlns + "d", XmlNamespace);

        internal static String GetFilesPropInDir(String dir, int depth) {
            if (null == dir) {
                throw new ArgumentNullException("dir");
            }
            if ('/' != dir[dir.Length - 1]) {
                dir += '/';
            }
            var dirInfo = FileManager.GetInstanse().GetDirInfo(dir);
            if (!dirInfo.Exists) {
                throw new DirectoryNotFoundException(dir);
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XmlNamespace + "multistatus");
            multiStatus.Add(NamespaceAttribute);
            main.Add(multiStatus);
            multiStatus.Add(GetFileProp(dir));
            if (0 == depth) {
                return main.Declaration.ToString() + main;
            }
            foreach (var directoryInfo in dirInfo.GetDirectories()) {
                multiStatus.Add(GetFileProp(dir + directoryInfo.Name));
            }
            foreach (var fileInfo in dirInfo.GetFiles()) {
                multiStatus.Add(GetFileProp(dir  + fileInfo.Name));
            }
            return main.Declaration.ToString() +  main;
        }
        internal static String GetFilesProp(String file) {
            if (null == file) {
                throw new ArgumentNullException("file");
            }
            var fileInfo = FileManager.GetInstanse().GetFileInfo(file);
            if (!fileInfo.Exists) {
                throw new DirectoryNotFoundException(file);
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XmlNamespace + "multistatus");
            multiStatus.Add(NamespaceAttribute);
            main.Add(multiStatus);
            multiStatus.Add(GetFileProp(file));
            return main.Declaration.ToString() + main;
        }

        internal static String GetLockDiscovery(LockInfo lockInfo) {
            if (null == lockInfo) {
                throw new ArgumentNullException("lockInfo");
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var prop = new XElement(XmlNamespace + "prop");
            prop.Add(NamespaceAttribute);
            main.Add(prop);
            prop.Add(GetLockDisc(lockInfo));
            return main.Declaration.ToString() + main;
        }

        private static XElement GetLockDisc(LockInfo lockInfo) {
            var main = new XElement(XmlNamespace + "lockdiscovery");
            if (null == lockInfo) {
                return null;
            }
            var active = new XElement(XmlNamespace + "activelock");
            main.Add(active);
            active.Add(new XElement(XmlNamespace + "locktype", new XElement(XmlNamespace + "write")));
            String scope;
            switch (lockInfo.LockScope) {
                case LockScope.Exclusive:
                    scope = "exclusive";
                    break;
                case LockScope.Shared:
                    scope = "shared";
                    break;
                default:
                    scope = "exclusive";
                    break;
            }
            active.Add(new XElement(XmlNamespace + "lockscope", new XElement(XmlNamespace + scope)));
            var depth = new XElement(XmlNamespace + "depth") {Value = "" + 0};

            active.Add(depth);
            var locktoken = new XElement(XmlNamespace + "locktoken");
            var href = new XElement(XmlNamespace + "href") { Value = lockInfo.LockToken };
            locktoken.Add(href);
            active.Add(locktoken);
            var owner = new XElement(XmlNamespace + "owner");
            href = new XElement(XmlNamespace + "href") { Value = lockInfo.Owner };
            owner.Add(href);
            active.Add(owner);
            return main;
        }

        private static XElement GetFileProp(String path) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            var fileInfo = FileManager.GetInstanse().GetFileInfo(path);
            var directoryInfo = FileManager.GetInstanse().GetDirInfo(path);
            XElement main;
            if (fileInfo.Exists) {
                main = CreateXDocumentFromFile(path, false);
            } else if (directoryInfo.Exists) {
                main = CreateXDocumentFromFile(path, true);
            } else {
                throw new FileNotFoundException("getFileProp " + path);
            }
            return main;
        }

        private static XElement CreateXDocumentFromFile(String path, bool isDir) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            var fileInfo = FileManager.GetInstanse().GetFileInfo(path);
            var dirInfo = FileManager.GetInstanse().GetDirInfo(path);
            var response = isDir ? new FileResponseBuilder(dirInfo, path) : new FileResponseBuilder(fileInfo, path);
            return response.GetXml();
        }

        private class FileResponseBuilder {
            private readonly FileInfo _mFileInfo;
            private readonly String _mPath;
            private readonly DirectoryInfo _mDirInfo;
            private readonly bool _isDir;

            internal FileResponseBuilder(FileInfo fileInfo, String path) {
                if (null == fileInfo) {
                    throw new ArgumentNullException("fileInfo");
                }
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                _mFileInfo = fileInfo;
                _mDirInfo = null;
                _isDir = false;
                _mPath = path;
            }

            internal FileResponseBuilder(DirectoryInfo dirInfo, String path) {
                if (null == dirInfo) {
                    throw new ArgumentNullException("dirInfo");
                }
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                _mDirInfo = dirInfo;
                _mFileInfo = null;
                _isDir = true;
                _mPath = path;
            }

            internal XElement GetXml() {
                var propStat = _isDir ? new FilePropStatBuilder(_mDirInfo, _mPath) : new FilePropStatBuilder(_mFileInfo, _mPath);
                var main = new XElement(XmlNamespace + "response");
                var href = new XElement(XmlNamespace + "href", _mPath);
                main.Add(href);
                main.Add(propStat.GetXml());
                return main;
            }
        }

        internal class FilePropStatBuilder {
            private readonly FileInfo _mFileInfo;
            private readonly DirectoryInfo _mDirInfo;
            private readonly bool _isDir;
            private readonly String _relativeName;

            internal FilePropStatBuilder(FileInfo fileInfo, String relName) {
                if (null == fileInfo) {
                    throw new ArgumentNullException("fileInfo");
                }
                if (null == relName) {
                    throw new ArgumentNullException("relName");
                }
                _mFileInfo = fileInfo;
                _mDirInfo = null;
                _isDir = false;
                _relativeName = relName;
            }

            internal FilePropStatBuilder(DirectoryInfo dirInfo, String relName) {
                if (null == dirInfo) {
                    throw new ArgumentNullException("dirInfo");
                }
                if (null == relName) {
                    throw new ArgumentNullException("relName");
                }
                _mDirInfo = dirInfo;
                _mFileInfo = null;
                _isDir = true;
                _relativeName = relName;
            }

            internal XElement GetXml() {
                var main = new XElement(XmlNamespace + "propstat");
                var status = new XElement(XmlNamespace + "status", "HTTP/1.1 200 OK");
                var prop = GetXmlProp();
                main.Add(status);
                main.Add(prop);
                return main;
            }

            private XElement GetXmlProp() {
                var main = new XElement(XmlNamespace + "prop");
                var creationDate = GetCreationDate();
                var lastModifyDate = GetLastModifiedDate();
                var lockDiscover = GetLockDisc(LockManager.GetInstanse().GetLockInfo(_relativeName));
                if (null != lockDiscover) {
                    main.Add(lockDiscover);
                }
                main.Add(creationDate);
                main.Add(lastModifyDate);
                if (_isDir) {
                    var resType = new XElement(XmlNamespace + "resourcetype", new XElement(XmlNamespace + "collection"));
                    main.Add(resType);
                } else {
                    var resType = new XElement(XmlNamespace + "resourcetype");
                    var length = new XElement(XmlNamespace + "getcontentlength", _mFileInfo.Length);
                    main.Add(resType);
                    main.Add(length);
                }
                return main;
            }

            private XElement GetCreationDate() {
                if (_isDir) {
                    return new XElement(XmlNamespace + "creationdate", _mDirInfo.CreationTime.ToUniversalTime()
                         .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                }
                return new XElement(XmlNamespace + "creationdate", _mFileInfo.CreationTime.ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }

            private XElement GetLastModifiedDate() {
                if (_isDir) {
                    return new XElement(XmlNamespace + "getlastmodified", Directory.GetLastWriteTime(_mDirInfo.FullName).ToUniversalTime()
                        .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                }
                return new XElement(XmlNamespace + "getlastmodified", File.GetLastWriteTime(_mFileInfo.FullName).ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
        }

    }

}
