using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDAVServer.api.helpers {
    internal sealed class PropPatchHelper {
        private static readonly XNamespace XML_NAMESPACE = "DAV:";
        private static readonly XAttribute NAMESPACE_ATTRIBUTE = new XAttribute(XNamespace.Xmlns + "d", XML_NAMESPACE);

        internal static String parsePropPatchContent(String uri, String content) {
            var document = XDocument.Parse(content);
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XML_NAMESPACE + "multistatus");
            var response = new XElement(XML_NAMESPACE + "response");
            main.Add(multiStatus);
            multiStatus.Add(response);
            multiStatus.Add(NAMESPACE_ATTRIBUTE);
            if (null == document.Root || !document.Root.Name.Equals(XML_NAMESPACE + "propertyupdate")) {
                return null;
            }
            var propertyupdate = document.Root;
            var setRes = parseSet(uri, getElement(propertyupdate, XML_NAMESPACE + "set"));
            var removeRes = parseRemove(uri, getElement(propertyupdate, XML_NAMESPACE + "remove"));
            response.Add(setRes);
            response.Add(removeRes);
            return main.Declaration.ToString() + main;
        }

        internal static List<XElement> parseSet(String uri, XContainer element) {
            if (element == null) {
                return null;
            }
            element = getElement(element, XML_NAMESPACE + "prop");
            if (null == element) {
                return new List<XElement>();
            }
            var props = (element.Elements()
                .Select(xElement => new {element = xElement, value = xElement.Elements() })
                .Select(@t => new PropInfo { prop = @t.element })).ToList();
            PropertyManager.getInstanse().addProps(uri, props);
            var propstats = new List<XElement>();
            foreach (var propInfo in props) {
                var propstat = new XElement(XML_NAMESPACE + "propstat");
                var prop = new XElement(XML_NAMESPACE + "prop");
                var status = new XElement(XML_NAMESPACE + "status");
                prop.Add(new XElement(propInfo.prop.Name));
                status.Value = "HTTP/1.1 200 OK";
                propstat.Add(prop);
                propstat.Add(status);
                propstats.Add(propstat);
            }
            return propstats;
        }

        internal static List<XElement> parseRemove(String uri, XContainer element) {
            if (element == null) {
                return null;
            }
            element = getElement(element, XML_NAMESPACE + "prop");
            if (null == element) {
                return new List<XElement>();
            }
            var props = (element.Elements()
                .Select(xElement => new { element = xElement, value = xElement.Elements() })
                .Select(@t => new PropInfo { prop = @t.element })).ToList();
            var propstats = new List<XElement>();
            foreach (var propInfo in props) {
                PropertyManager.getInstanse().delete(uri, propInfo);
                var propstat = new XElement(XML_NAMESPACE + "propstat");
                var prop = new XElement(XML_NAMESPACE + "prop");
                var status = new XElement(XML_NAMESPACE + "status");
                prop.Add(new XElement(propInfo.prop.Name));
                status.Value = "HTTP/1.1 200 OK";
                propstat.Add(prop);
                propstat.Add(status);
                propstats.Add(propstat);
            }
            return propstats;
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
