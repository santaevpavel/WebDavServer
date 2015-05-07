using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.helpers;

namespace WebDAVServer.tests {
    [TestClass]
    public class PropertyManagerTest {

        [TestMethod]
        public void addProp() {
            const string uri = "a.txt";
            const string prop = "prop1";
            var propInfo = new PropInfo {prop = new XElement(prop)};
            PropertyManager.getInstanse().addProps(uri, new List<PropInfo> {propInfo});
            var name = PropertyManager.getInstanse().getLockInfo(uri).ElementAt(0).prop.Name.LocalName;
            Assert.AreEqual(name, prop);
        }

        [TestMethod]
        public void deleteProp() {
            const string uri = "b.txt";
            const string prop = "prop2";
            var propInfo = new PropInfo { prop = new XElement(prop) };
            PropertyManager.getInstanse().addProps(uri, new List<PropInfo> { propInfo });
            var name = PropertyManager.getInstanse().getLockInfo(uri).ElementAt(0).prop.Name.LocalName;
            Assert.AreEqual(name, prop);
            PropertyManager.getInstanse().delete(uri, propInfo);
            var deletedInfo = PropertyManager.getInstanse().getLockInfo(uri).Count;
            Assert.AreEqual(0, deletedInfo);
        }
    }
}
