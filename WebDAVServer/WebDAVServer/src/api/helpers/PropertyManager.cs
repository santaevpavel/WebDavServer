using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDAVServer.api.helpers {
    internal sealed class PropertyManager {
    
    private static PropertyManager instance;
    private readonly Dictionary<String, List<PropInfo>> props = new Dictionary<String, List<PropInfo>>();

        private PropertyManager() {    
        }

        internal static PropertyManager getInstanse() {
            return instance ?? (instance = new PropertyManager());
        }

        internal List<PropInfo> getLockInfo(String uri) {
            return props.ContainsKey(uri) ? props[uri] : null;
        }

        internal bool addProps(String uri, List<PropInfo> propInfos) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == propInfos) {
                throw new ArgumentNullException("propInfos");
            }
            if (props.ContainsKey(uri)) {
                props[uri].AddRange(propInfos);
                return true;
            }
            props.Add(uri, propInfos);
            return true;
        }

        internal bool delete(String uri, PropInfo propInfo) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == propInfo) {
                throw new ArgumentNullException("propInfo");
            }
            if (!props.ContainsKey(uri)) {
                return true;
            }
            var list = props[uri].Where(info => info.prop.Name.Equals(propInfo.prop.Name));
            var propInfos = list as PropInfo[] ?? list.ToArray();
            if (propInfos.Any()) {
                props[uri].Remove(propInfos.ElementAt(0));
            }
            return true;
        }
    }


    internal sealed class PropInfo {
        internal XElement prop;
    }
}
