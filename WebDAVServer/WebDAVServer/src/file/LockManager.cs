using System;
using System.Collections.Generic;
using WebDAVServer.api.request;

namespace WebDAVServer.file {
    internal sealed class LockManager {
        private static LockManager instance;
        private readonly Dictionary<String, LockInfo> locks = new Dictionary<String, LockInfo>();

        private LockManager() {    
        }

        internal static LockManager getInstanse() {
            return instance ?? (instance = new LockManager());
        }

        internal LockInfo getLockInfo(String uri) {
            return locks.ContainsKey(uri) ? locks[uri] : null;
        }

        internal void lockFile(String uri, LockInfo lockInfo) {
            locks.Remove(uri);
            locks.Add(uri, lockInfo);
        }

        internal bool unlock(String uri, String token) {
            if (!locks.ContainsKey(uri) || !locks[uri].lockToken.Equals(token)) {
                return false;
            }
            locks.Remove(uri);
            return true;
        }
    }


    internal sealed class LockInfo {
        internal String path;
        internal LockType lockType;
        internal LockScope lockScope;
        internal int depth = -1;
        internal String owner;
        internal String lockToken;

        internal LockInfo() {
            
        }
        internal LockInfo(String path, LockType lockType, LockScope lockScope,
            int depth, String owner, String lockToken) {
            this.path = path;
            this.lockType = lockType;
            this.lockScope = lockScope;
            this.depth = depth;
            this.owner = owner;
            this.lockToken = lockToken;
        }
    }
}
