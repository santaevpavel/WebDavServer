using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDAVServer.api.helpers {
    internal sealed class PropPatchHelper {
        private static readonly XNamespace XmlNamespace = "DAV:";
        private static readonly XAttribute NamespaceAttribute = new XAttribute(XNamespace.Xmlns + "d", XmlNamespace);

        internal static String ParsePropPatchContent(String uri, String content) {
            var document = XDocument.Parse(content);
            var main = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var multiStatus = new XElement(XmlNamespace + "multistatus");
            var response = new XElement(XmlNamespace + "response");
            main.Add(multiStatus);
            multiStatus.Add(response);
            multiStatus.Add(NamespaceAttribute);
            if (null == document.Root || !document.Root.Name.Equals(XmlNamespace + "propertyupdate")) {
                return null;
            }
            var propertyupdate = document.Root;
            var setRes = ParseSet(uri, GetElement(propertyupdate, XmlNamespace + "set"));
            var removeRes = ParseRemove(uri, GetElement(propertyupdate, XmlNamespace + "remove"));
            response.Add(setRes);
            response.Add(removeRes);
            return main.Declaration.ToString() + main;
        }

        internal static List<XElement> ParseSet(String uri, XContainer element) {
            if (element == null) {
                return null;
            }
            element = GetElement(element, XmlNamespace + "prop");
            if (null == element) {
                return new List<XElement>();
            }
            var props = (element.Elements()
                .Select(xElement => new {element = xElement, value = xElement.Elements() })
                .Select(@t => new PropInfo { Prop = @t.element })).ToList();
            PropertyManager.GetInstanse().AddProps(uri, props);
            var propstats = new List<XElement>();
            foreach (var propInfo in props) {
                var propstat = new XElement(XmlNamespace + "propstat");
                var prop = new XElement(XmlNamespace + "prop");
                var status = new XElement(XmlNamespace + "status");
                prop.Add(new XElement(propInfo.Prop.Name));
                status.Value = "HTTP/1.1 200 OK";
                propstat.Add(prop);
                propstat.Add(status);
                propstats.Add(propstat);
            }
            return propstats;
        }

        internal static List<XElement> ParseRemove(String uri, XContainer element) {
            if (element == null) {
                return null;
            }
            element = GetElement(element, XmlNamespace + "prop");
            if (null == element) {
                return new List<XElement>();
            }
            var props = (element.Elements()
                .Select(xElement => new { element = xElement, value = xElement.Elements() })
                .Select(@t => new PropInfo { Prop = @t.element })).ToList();
            var propstats = new List<XElement>();
            foreach (var propInfo in props) {
                PropertyManager.GetInstanse().Delete(uri, propInfo);
                var propstat = new XElement(XmlNamespace + "propstat");
                var prop = new XElement(XmlNamespace + "prop");
                var status = new XElement(XmlNamespace + "status");
                prop.Add(new XElement(propInfo.Prop.Name));
                status.Value = "HTTP/1.1 200 OK";
                propstat.Add(prop);
                propstat.Add(status);
                propstats.Add(propstat);
            }
            return propstats;
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
