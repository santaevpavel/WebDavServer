using System;
using System.Linq;
using System.Xml.Linq;
using WebDAVServer.api.request;

namespace WebDAVServer.api.helpers {
    internal static class LockHelper {

        private static readonly XNamespace XML_NAMESPACE = "DAV:";

        internal static void parseLockContent(String content, LockRequest lockRequest) {
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
            parseLockType(lockRequest, document.Root);
            parseLockScope(lockRequest, document.Root);
            parseOwner(lockRequest, document.Root);
        }
        internal static void getLockProp(String content, LockRequest lockRequest) {
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
            parseLockType(lockRequest, document.Root);
            parseLockScope(lockRequest, document.Root);
            parseOwner(lockRequest, document.Root);
        }
        private static void parseLockType(LockRequest lockRequest, XContainer element) {
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            var lockType = getElement(element, XML_NAMESPACE + "locktype");
            if (null == lockType) {
                return;
            }
            //var type = lockType.Elements().ElementAt(0).ToString();
            lockRequest.setLockType(LockType.WRITE);
        }

        private static void parseLockScope(LockRequest lockRequest, XContainer element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            var lockScope = getElement(element, XML_NAMESPACE + "lockscope");
            if (null == lockScope) {
                return;
            }
            var type = lockScope.Elements().ElementAt(0).ToString();
            switch (type) {
                case "exclusive":
                    lockRequest.setLockScope(LockScope.EXCLUSIVE);
                    break;
                case "shared":
                    lockRequest.setLockScope(LockScope.SHARED);
                    break;
                default:
                    lockRequest.setLockScope(LockScope.EXCLUSIVE);
                    break;
            }
        }

        private static void parseOwner(LockRequest lockRequest, XContainer element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            if (null == lockRequest) {
                throw new ArgumentNullException("lockRequest");
            }
            var owner = getElement(element, XML_NAMESPACE + "owner");
            if (null == owner) {
                return;
            }
            var value = getElement(owner, XML_NAMESPACE + "href");
            lockRequest.setOwner(value.Value);
        }

        private static XElement getElement(XContainer element, XName name) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }
            var elems = element.Elements().Where(xElement => xElement.Name.Equals(name));
            var xElements = elems as XElement[] ?? elems.ToArray();
            return xElements.Any() ? xElements.ElementAt(0) : null;
        }
    }
}
