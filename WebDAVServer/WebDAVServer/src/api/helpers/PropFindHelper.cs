using System;
using System.IO;
using System.Xml.Linq;
using WebDAVServer.api.request;
using WebDAVServer.file;

namespace WebDAVServer.api.helpers {

    
    internal sealed class PropFindHelper {

        private static readonly XNamespace XML_NAMESPACE = "DAV:";
        private static readonly XAttribute NAMESPACE_ATTRIBUTE = new XAttribute(XNamespace.Xmlns + "d", XML_NAMESPACE);

        internal static String getFilesPropInDir(String dir, int depth) {
            if (null == dir) {
                throw new ArgumentNullException("dir");
            }
            if ('/' != dir[dir.Length - 1]) {
                dir += '/';
            }
            var dirInfo = FileManager.getInstanse().getDirInfo(dir);
            if (!dirInfo.Exists) {
                throw new DirectoryNotFoundException(dir);
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XML_NAMESPACE + "multistatus");
            multiStatus.Add(NAMESPACE_ATTRIBUTE);
            main.Add(multiStatus);
            multiStatus.Add(getFileProp(dir));
            if (0 == depth) {
                return main.Declaration.ToString() + main;
            }
            foreach (var directoryInfo in dirInfo.GetDirectories()) {
                multiStatus.Add(getFileProp(dir + directoryInfo.Name));
            }
            foreach (var fileInfo in dirInfo.GetFiles()) {
                multiStatus.Add(getFileProp(dir  + fileInfo.Name));
            }
            return main.Declaration.ToString() +  main;
        }
        internal static String getFilesProp(String file) {
            if (null == file) {
                throw new ArgumentNullException("file");
            }
            var fileInfo = FileManager.getInstanse().getFileInfo(file);
            if (!fileInfo.Exists) {
                throw new DirectoryNotFoundException(file);
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XML_NAMESPACE + "multistatus");
            multiStatus.Add(NAMESPACE_ATTRIBUTE);
            main.Add(multiStatus);
            multiStatus.Add(getFileProp(file));
            return main.Declaration.ToString() + main;
        }

        internal static String getLockDiscovery(LockInfo lockInfo) {
            if (null == lockInfo) {
                throw new ArgumentNullException("lockInfo");
            }
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var prop = new XElement(XML_NAMESPACE + "prop");
            prop.Add(NAMESPACE_ATTRIBUTE);
            main.Add(prop);
            prop.Add(getLockDisc(lockInfo));
            return main.Declaration.ToString() + main;
        }

        private static XElement getLockDisc(LockInfo lockInfo) {
            var main = new XElement(XML_NAMESPACE + "lockdiscovery");
            if (null == lockInfo) {
                return main;
            }
            var active = new XElement(XML_NAMESPACE + "activelock");
            main.Add(active);
            active.Add(new XElement(XML_NAMESPACE + "locktype"), new XElement(XML_NAMESPACE + "write"));
            String scope;
            switch (lockInfo.lockScope) {
                case LockScope.EXCLUSIVE:
                    scope = "exclusive";
                    break;
                case LockScope.SHARED:
                    scope = "shared";
                    break;
                default:
                    scope = "exclusive";
                    break;
            }
            active.Add(new XElement(XML_NAMESPACE + "lockscope"), new XElement(XML_NAMESPACE + scope));
            var depth = new XElement(XML_NAMESPACE + "depth") {Value = "" + 0};

            active.Add(depth);
            var locktoken = new XElement(XML_NAMESPACE + "locktoken");
            var href = new XElement(XML_NAMESPACE + "href") { Value = lockInfo.lockToken };
            locktoken.Add(href);
            active.Add(locktoken);
            var owner = new XElement(XML_NAMESPACE + "owner");
            href = new XElement(XML_NAMESPACE + "href") { Value = lockInfo.owner };
            owner.Add(href);
            active.Add(owner);
            return main;
        }

        private static XElement getFileProp(String path) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            var fileInfo = FileManager.getInstanse().getFileInfo(path);
            var directoryInfo = FileManager.getInstanse().getDirInfo(path);
            XElement main;
            if (fileInfo.Exists) {
                main = createXDocumentFromFile(path, false);
            } else if (directoryInfo.Exists) {
                main = createXDocumentFromFile(path, true);
            } else {
                throw new FileNotFoundException("getFileProp " + path);
            }
            return main;
        }

        private static XElement createXDocumentFromFile(String path, bool isDir) {
            if (null == path) {
                throw new ArgumentNullException("path");
            }
            var fileInfo = FileManager.getInstanse().getFileInfo(path);
            var dirInfo = FileManager.getInstanse().getDirInfo(path);
            var response = isDir ? new FileResponseBuilder(dirInfo, path) : new FileResponseBuilder(fileInfo, path);
            return response.getXml();
        }

        private class FileResponseBuilder {
            private readonly FileInfo mFileInfo;
            private readonly String mPath;
            private readonly DirectoryInfo mDirInfo;
            private readonly bool isDir;

            internal FileResponseBuilder(FileInfo fileInfo, String path) {
                if (null == fileInfo) {
                    throw new ArgumentNullException("fileInfo");
                }
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                mFileInfo = fileInfo;
                mDirInfo = null;
                isDir = false;
                mPath = path;
            }

            internal FileResponseBuilder(DirectoryInfo dirInfo, String path) {
                if (null == dirInfo) {
                    throw new ArgumentNullException("dirInfo");
                }
                if (null == path) {
                    throw new ArgumentNullException("path");
                }
                mDirInfo = dirInfo;
                mFileInfo = null;
                isDir = true;
                mPath = path;
            }

            internal XElement getXml() {
                var propStat = isDir ? new FilePropStatBuilder(mDirInfo, mPath) : new FilePropStatBuilder(mFileInfo, mPath);
                var main = new XElement(XML_NAMESPACE + "response");
                var href = new XElement(XML_NAMESPACE + "href", mPath);
                main.Add(href);
                main.Add(propStat.getXml());
                return main;
            }
        }

        internal class FilePropStatBuilder {
            private readonly FileInfo mFileInfo;
            private readonly DirectoryInfo mDirInfo;
            private readonly bool isDir;
            private readonly String relativeName;

            internal FilePropStatBuilder(FileInfo fileInfo, String relName) {
                if (null == fileInfo) {
                    throw new ArgumentNullException("fileInfo");
                }
                if (null == relName) {
                    throw new ArgumentNullException("relName");
                }
                mFileInfo = fileInfo;
                mDirInfo = null;
                isDir = false;
                relativeName = relName;
            }

            internal FilePropStatBuilder(DirectoryInfo dirInfo, String relName) {
                if (null == dirInfo) {
                    throw new ArgumentNullException("fileInfo");
                }
                if (null == relName) {
                    throw new ArgumentNullException("relName");
                }
                mDirInfo = dirInfo;
                mFileInfo = null;
                isDir = true;
                relativeName = relName;
            }

            internal XElement getXml() {
                var main = new XElement(XML_NAMESPACE + "propstat");
                var status = new XElement(XML_NAMESPACE + "status", "HTTP/1.1 200 OK");
                var prop = getXmlProp();
                main.Add(status);
                main.Add(prop);
                return main;
            }

            private XElement getXmlProp() {
                var main = new XElement(XML_NAMESPACE + "prop");
                var creationDate = getCreationDate();
                var lastModifyDate = getLastModifiedDate();
                var lockDiscover = getLockDisc(LockManager.getInstanse().getLockInfo(relativeName));
                if (null != lockDiscover) {
                    main.Add(lockDiscover);
                }
                main.Add(creationDate);
                main.Add(lastModifyDate);
                if (isDir) {
                    var resType = new XElement(XML_NAMESPACE + "resourcetype", new XElement(XML_NAMESPACE + "collection"));
                    main.Add(resType);
                } else {
                    var resType = new XElement(XML_NAMESPACE + "resourcetype");
                    var length = new XElement(XML_NAMESPACE + "getcontentlength", mFileInfo.Length);
                    main.Add(resType);
                    main.Add(length);
                }
                return main;
            }

            private XElement getCreationDate() {
                if (isDir) {
                    return new XElement(XML_NAMESPACE + "creationdate", mDirInfo.CreationTime.ToUniversalTime()
                         .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                }
                return new XElement(XML_NAMESPACE + "creationdate", mFileInfo.CreationTime.ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }

            private XElement getLastModifiedDate() {
                if (isDir) {
                    return new XElement(XML_NAMESPACE + "getlastmodified", Directory.GetLastWriteTime(mDirInfo.FullName).ToUniversalTime()
                        .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                }
                return new XElement(XML_NAMESPACE + "getlastmodified", File.GetLastWriteTime(mFileInfo.FullName).ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
        }

    }

}
