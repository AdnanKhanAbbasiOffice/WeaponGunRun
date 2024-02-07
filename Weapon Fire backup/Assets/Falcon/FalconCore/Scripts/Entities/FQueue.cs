using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Falcon.FalconCore.Scripts.Entities
{
    public class FQueue<T> : ConcurrentQueue<T>
    {
#pragma warning disable CS0109
        public new List<T> Clear()
#pragma warning restore CS0109
        {
            var result = new List<T>();
            T val;
            while (TryDequeue(out val)) result.Add(val);

            return result;
        }

        public void EnqueueAll(IEnumerable<T> collection)
        {
            foreach (var t in collection) Enqueue(t);
        }
        
        public void Remove(T item)
        {
            if (this.Contains(item))
            {
                var result = Clear();
                result.Remove(item);
                EnqueueAll(result);
            }
        }
    }
}