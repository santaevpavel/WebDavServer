using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDAVServer.api.helpers {
    internal sealed class PropertyManager {
    
    private static PropertyManager _instance;
    private readonly Dictionary<String, List<PropInfo>> _props = new Dictionary<String, List<PropInfo>>();

        private PropertyManager() {    
        }

        internal static PropertyManager GetInstanse() {
            return _instance ?? (_instance = new PropertyManager());
        }

        internal List<PropInfo> GetLockInfo(String uri) {
            return _props.ContainsKey(uri) ? _props[uri] : null;
        }

        internal bool AddProps(String uri, List<PropInfo> propInfos) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == propInfos) {
                throw new ArgumentNullException("propInfos");
            }
            if (_props.ContainsKey(uri)) {
                _props[uri].AddRange(propInfos);
                return true;
            }
            _props.Add(uri, propInfos);
            return true;
        }

        internal bool Delete(String uri, PropInfo propInfo) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == propInfo) {
                throw new ArgumentNullException("propInfo");
            }
            if (!_props.ContainsKey(uri)) {
                return true;
            }
            var list = _props[uri].Where(info => info.Prop.Name.Equals(propInfo.Prop.Name));
            var propInfos = list as PropInfo[] ?? list.ToArray();
            if (propInfos.Any()) {
                _props[uri].Remove(propInfos.ElementAt(0));
            }
            return true;
        }
    }


    internal sealed class PropInfo {
        internal XElement Prop;
    }
}
