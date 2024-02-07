using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Falcon.FalconCore.Scripts.Entities
{
    public class LockMap<T>
    {
        private readonly ConcurrentDictionary<T, LockKey<T>> dictionary = new ConcurrentDictionary<T, LockKey<T>>();
        private bool lockAll;
        
        public bool TryLock(T key, out LockKey<T> lockKey)
        {
            if (lockAll)
            {
                lockKey = null;
                return false;
            }

            lockKey = dictionary.GetOrAdd(key, new LockKey<T>(this, key));
            if (lockKey.TryLock())
            {
                return true;
            }

            return false;
        }

        public LockKey<T> Lock(T key)
        {
            LockKey<T> result;
            while (!TryLock(key, out result))
            {
                Thread.Yield();
            }


            return result;
        }

        public LockAllKey<T> LockAll()
        {
            lockAll = true;
            while (!dictionary.IsEmpty)
            {
                Thread.Yield();
            }

            return new LockAllKey<T>(this);
        }

        internal void Unlock(T key)
        {
            LockKey<T> ignored;
            dictionary.TryRemove(key, out ignored);
        }

        internal void UnlockAll()
        {
            lockAll = false;
        }
    }

    public class LockKey<T> : IDisposable
    {
        private readonly LockMap<T> lockMap;
        private readonly T key;
        private readonly int threadId = Thread.CurrentThread.ManagedThreadId;
        private int codeDepth;

        internal LockKey(LockMap<T> lockMap, T key)
        {
            this.lockMap = lockMap;
            this.key = key;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            codeDepth--;
            if (codeDepth == 0)
            {
                lockMap.Unlock(key);
            }
        }

        internal bool TryLock()
        {
            if (threadId == Thread.CurrentThread.ManagedThreadId)
            {
                codeDepth++;
                return true;
            }

            return false;
        }
    }

    public class LockAllKey<T> : IDisposable
    {
        private readonly LockMap<T> lockMap;

        public LockAllKey(LockMap<T> lockMap)
        {
            this.lockMap = lockMap;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            lockMap.UnlockAll();
        }
    }
}