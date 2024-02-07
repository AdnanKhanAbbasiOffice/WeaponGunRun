using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Falcon.FalconCore.Scripts.Entities
{
    public class FConcurrentDict<T, V> : ConcurrentDictionary<T, V>
    {
        private readonly LockMap<T> lockMap= new LockMap<T>();

        public FConcurrentDict()
        {
        }

        public FConcurrentDict([NotNull] IEnumerable<KeyValuePair<T, V>> collection) : base(collection)
        {
        }

        public FConcurrentDict([NotNull] IEnumerable<KeyValuePair<T, V>> collection, [NotNull] IEqualityComparer<T> comparer) : base(collection, comparer)
        {
        }

        public FConcurrentDict([NotNull] IEqualityComparer<T> comparer) : base(comparer)
        {
        }

        public FConcurrentDict(int concurrencyLevel, [NotNull] IEnumerable<KeyValuePair<T, V>> collection, [NotNull] IEqualityComparer<T> comparer) : base(concurrencyLevel, collection, comparer)
        {
        }

        public FConcurrentDict(int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity)
        {
        }

        public FConcurrentDict(int concurrencyLevel, int capacity, [NotNull] IEqualityComparer<T> comparer) : base(concurrencyLevel, capacity, comparer)
        {
        }

        public V Compute(T key, Func<bool, V, V> function)
        {
            using (lockMap.Lock(key))
            {
                V oldValue;
                var newValue = TryGetValue(key, out oldValue)
                    ? function.Invoke(true, oldValue)
                    : function.Invoke(false, default(V));

                if (newValue != null)
                    this[key] = newValue;
                else
                    Remove(key);
                return newValue;
            }
        }

        public KeyValuePair<bool,V> ComputeIfPresent(T key, Func<V, V> ifPresent)
        {
            using (lockMap.Lock(key))
            {
                V oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    V value = ifPresent.Invoke(oldValue);
                    if (value != null)
                        this[key] = value;
                    else
                        Remove(key);

                    return new KeyValuePair<bool, V>(true, value);
                }
                
                return new KeyValuePair<bool, V>(false, default(V));
            }
        }

        public bool ComputeIfPresent(T key, Action<V> ifPresent)
        {
            using (lockMap.Lock(key))
            {
                V oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    ifPresent.Invoke(oldValue);
                    return true;
                }
                return false;
            }
        }

        public KeyValuePair<bool, V> ComputeIfAbsent(T key, Func<V> ifAbsent)
        {
            using (lockMap.Lock(key))
            {
                if (!ContainsKey(key))
                {
                    V value = ifAbsent.Invoke();
                    if (value != null) this[key] = value;
                    return new KeyValuePair<bool, V>(true, value);
                }
                return new KeyValuePair<bool, V>(false, default(V));
            }
        }

        public bool ComputeIfAbsent(T key, Action ifAbsent)
        {
            using (lockMap.Lock(key))
            {
                if (!ContainsKey(key))
                {
                    ifAbsent.Invoke();
                    return true;
                }

                return false;
            }
        }

        public V GetOrSet(T key, V orSet)
        {
            V result;
            if (TryGetValue(key, out result)) return result;

            using (lockMap.Lock(key))
            {
                if (ContainsKey(key)) return this[key];

                this[key] = orSet;
                return orSet;
            }
        }

        public V GetOrDefault(T key, V orDefault)
        {
            V result;
            if (TryGetValue(key, out result)) return result;

            return orDefault;
        }

        public bool Replace(T key, V ifAbsent)
        {
            if (!ContainsKey(key))
            {
                return false;
            }
            using (lockMap.Lock(key))
            {
                if (ContainsKey(key))
                {
                    this[key] = ifAbsent;
                    return true;
                }

                return false;
            }
        }

        #region NewFuncs

        public object this[object key]
        {
            get { return this[(T)key]; }
            set
            {
                using (lockMap.Lock((T)key))
                {
                    this[(T)key] = (V)value;
                }
            }
        }


        public new void Clear()
        {
            using (lockMap.LockAll())
            {
                base.Clear();
            }
        }

        public bool Contains(KeyValuePair<T, V> item)
        {
            using (lockMap.Lock(item.Key))
            {
                if (ContainsKey(item.Key) && Equals(this[item.Key], item.Value)) return true;

                return false;
            }
        }

        public void Add(T key, V value)
        {
            using (lockMap.Lock(key))
            {
                this[key] = value;
            }
        }


        public bool Remove(T key)
        {
            using (lockMap.Lock(key))
            {
                V ignored;
                return TryRemove(key, out ignored);
            }
        }

        public new V this[T key]
        {
            get { return base[key]; }
            set
            {
                using (lockMap.Lock(key))
                {
                    base[key] = value;
                }
            }
        }

        #endregion
    }
}