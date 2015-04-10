using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using WebDAVServer.file;

namespace WebDAVServer.api.helpers {

    
    internal sealed class PropFindHelper {

        public static readonly XNamespace xmlNamespace = "DAV:";
        public static readonly XAttribute namespaceAttribute = new XAttribute(XNamespace.Xmlns + "d", xmlNamespace);

        public static String getFilesPropInDir(String dir) {
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(xmlNamespace + "multistatus");
            multiStatus.Add(namespaceAttribute);
            main.Add(multiStatus);
            var dirInfo = FileManager.getInstanse().getDirInfo(dir);
            foreach (var directoryInfo in dirInfo.GetDirectories()) {
                multiStatus.Add(getFileProp(dir + directoryInfo.Name));
            }
            foreach (var fileInfo in dirInfo.GetFiles()) {
                multiStatus.Add(getFileProp(dir  + fileInfo.Name));
            }
            return main.Declaration.ToString() +  main.ToString();
        }


        private static XElement getFileProp(String path) {
            var fileInfo = FileManager.getInstanse().getFileInfo(path);
            var directoryInfo = FileManager.getInstanse().getDirInfo(path);
            XElement main;
            if (fileInfo.Exists) {
                main = createXDocumentFromFile(path, false);
                //main.Add(namespaceAttribute);
            } else if (directoryInfo.Exists) {
                main = createXDocumentFromFile(path, true);
                //main.Add(namespaceAttribute);
            } else {
                throw new FileNotFoundException("getFileProp " + path);
            }
            return main;
        }

        private static XElement createXDocumentFromFile(String path, bool isDir) {
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

            public FileResponseBuilder(FileInfo fileInfo, String path) {
                mFileInfo = fileInfo;
                mDirInfo = null;
                isDir = false;
                mPath = path;
            }

            public FileResponseBuilder(DirectoryInfo dirInfo, String path) {
                mDirInfo = dirInfo;
                mFileInfo = null;
                isDir = true;
                mPath = path;
            }

            public XElement getXml() {
                var propStat = isDir ? new FilePropStatBuilder(mDirInfo) : new FilePropStatBuilder(mFileInfo);
                var main = new XElement(xmlNamespace + "response");
                var href = new XElement(xmlNamespace + "href", mPath);
                main.Add(href);
                //main.Add(namespaceAttribute);
                main.Add(propStat.getXml());
                return main;
            }
        }

        internal class FilePropStatBuilder {
            private readonly FileInfo mFileInfo;
            private readonly DirectoryInfo mDirInfo;
            private readonly bool isDir;

            public FilePropStatBuilder(FileInfo fileInfo) {
                mFileInfo = fileInfo;
                mDirInfo = null;
                isDir = false;
            }

            public FilePropStatBuilder(DirectoryInfo dirInfo) {
                mDirInfo = dirInfo;
                mFileInfo = null;
                isDir = true;
            }

            public XElement getXml() {

                var main = new XElement(xmlNamespace + "propstat");
                var status = new XElement(xmlNamespace + "status", "HTTP/1.1 200 OK");
                //status.SetValue("HTTP/1.1 200 OK");
                var prop = getXmlProp();
                main.Add(status);
                main.Add(prop);
                //main.Add(namespaceAttribute);
                return main;
            }

            private XElement getXmlProp() {
                var main = new XElement(xmlNamespace + "prop");
                if (isDir) {
                    var resType = new XElement(xmlNamespace + "resourcetype", new XElement(xmlNamespace + "collection"));
                    var creationDate = new XElement(xmlNamespace + "creationdate", "2015-01-02T18:47:43Z");
                    main.Add(resType);
                    main.Add(creationDate);
                    //main.Add(namespaceAttribute);
                } else {
                    var resType = new XElement(xmlNamespace + "resourcetype");
                    var creationDate = new XElement(xmlNamespace + "creationdate", "2015-01-02T18:47:43Z");
                    var length = new XElement(xmlNamespace + "getcontentlength", mFileInfo.Length);
                    main.Add(resType);
                    main.Add(creationDate);
                    main.Add(length);
                    //main.Add(namespaceAttribute);
                }
                return main;
            }
        }

    }

}
