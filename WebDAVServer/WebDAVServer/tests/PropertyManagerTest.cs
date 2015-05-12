using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDAVServer.api.helpers;

namespace WebDAVServer.tests {
    [TestClass]
    public class PropertyManagerTest {

        [TestMethod]
        public void AddProp() {
            const string uri = "a.txt";
            const string prop = "prop1";
            var propInfo = new PropInfo {Prop = new XElement(prop)};
            PropertyManager.GetInstanse().AddProps(uri, new List<PropInfo> {propInfo});
            var name = PropertyManager.GetInstanse().GetLockInfo(uri).ElementAt(0).Prop.Name.LocalName;
            Assert.AreEqual(name, prop);
        }

        [TestMethod]
        public void DeleteProp() {
            const string uri = "b.txt";
            const string prop = "prop2";
            var propInfo = new PropInfo { Prop = new XElement(prop) };
            PropertyManager.GetInstanse().AddProps(uri, new List<PropInfo> { propInfo });
            var name = PropertyManager.GetInstanse().GetLockInfo(uri).ElementAt(0).Prop.Name.LocalName;
            Assert.AreEqual(name, prop);
            PropertyManager.GetInstanse().Delete(uri, propInfo);
            var deletedInfo = PropertyManager.GetInstanse().GetLockInfo(uri).Count;
            Assert.AreEqual(0, deletedInfo);
        }
    }
}
