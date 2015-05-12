using System;
using System.Linq;
using System.Xml.Linq;
using WebDAVServer.api.request;

namespace WebDAVServer.api.helpers {
    internal static class LockHelper {

        private static readonly XNamespace XmlNamespace = "DAV:";

        internal static void ParseLockContent(String content, LockRequest lockRequest) {
            if (null == content) {
                throw new ArgumentNullException("content");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            if (!content.Any()) {
                return;
            }
            var document = XDocument.Parse(content);
            if (document.Root == null) {
                return;
            }
            ParseLockType(lockRequest, document.Root);
            ParseLockScope(lockRequest, document.Root);
            ParseOwner(lockRequest, document.Root);
        }
        internal static void GetLockProp(String content, LockRequest lockRequest) {
            if (null == content) {
                throw new ArgumentNullException("content");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            var document = XDocument.Parse(content);
            if (document.Root == null) {
                return;
            }
            ParseLockType(lockRequest, document.Root);
            ParseLockScope(lockRequest, document.Root);
            ParseOwner(lockRequest, document.Root);
        }
        private static void ParseLockType(LockRequest lockRequest, XContainer element) {
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            var lockType = GetElement(element, XmlNamespace + "locktype");
            if (null == lockType) {
                return;
            }
            lockRequest.SetLockType(LockType.Write);
        }

        private static void ParseLockScope(LockRequest lockRequest, XContainer element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            var lockScope = GetElement(element, XmlNamespace + "lockscope");
            if (null == lockScope) {
                return;
            }
            var type = lockScope.Elements().ElementAt(0).ToString();
            switch (type) {
                case "exclusive":
                    lockRequest.SetLockScope(LockScope.Exclusive);
                    break;
                case "shared":
                    lockRequest.SetLockScope(LockScope.Shared);
                    break;
                default:
                    lockRequest.SetLockScope(LockScope.Exclusive);
                    break;
            }
        }

        private static void ParseOwner(LockRequest lockRequest, XContainer element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            var owner = GetElement(element, XmlNamespace + "owner");
            if (null == owner) {
                return;
            }
            var value = GetElement(owner, XmlNamespace + "href");
            lockRequest.SetOwner(value.Value);
        }

        private static XElement GetElement(XContainer element, XName name) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            var elems = element.Elements().Where(xElement => xElement.Name.Equals(name));
            var xElements = elems as XElement[] ?? elems.ToArray();
            return xElements.Any() ? xElements.ElementAt(0) : null;
        }
    }
}
