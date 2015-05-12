using System;
using System.Collections.Generic;
using WebDAVServer.api.request;

namespace WebDAVServer.file {
    internal sealed class LockManager {
        private static LockManager _instance;
        private volatile Dictionary<String, LockInfo> _locks = new Dictionary<String, LockInfo>();

        private LockManager() {    
        }

        internal static LockManager GetInstanse() {
            return _instance ?? (_instance = new LockManager());
        }

        internal LockInfo GetLockInfo(String uri) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            return _locks.ContainsKey(uri) ? _locks[uri] : null;
        }

        internal void LockFile(String uri, LockInfo lockInfo) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == lockInfo) {
                throw new ArgumentNullException("lockInfo");
            }
            _locks.Remove(uri);
            _locks.Add(uri, lockInfo);
        }

        internal bool Unlock(String uri, String token) {
            if (null == uri) {
                throw new ArgumentNullException("uri");
            }
            if (null == token) {
                throw new ArgumentNullException("token");
            }
            if (!_locks.ContainsKey(uri) || !_locks[uri].LockToken.Equals(token)) {
                return false;
            }
            _locks.Remove(uri);
            return true;
        }
    }


    internal sealed class LockInfo {
        internal String Path;
        internal LockType LockType;
        internal LockScope LockScope;
        internal int Depth = -1;
        internal String Owner = "";
        internal String LockToken = "";

        internal LockInfo() {}

        internal LockInfo(String path, LockType lockType, LockScope lockScope,
            int depth, String owner, String lockToken) {
            Path = path;
            LockType = lockType;
            LockScope = lockScope;
            Depth = depth;
            Owner = owner;
            LockToken = lockToken;
        }
    }
}
