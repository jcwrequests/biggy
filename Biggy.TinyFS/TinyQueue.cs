using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyQueue<T> 
    {
        TinyList<T> store;
        object queueLock;
        string queueName;
        public TinyQueue(TinyList<T> store, string queueName)
        {
            if (store == null) throw new ArgumentNullException("store");
            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException("queueName");

            queueLock = new object();
            this.store = store;
            this.queueName = queueName;
        }
        public string QueueName { get { return queueName; } }

        public void EnQueue(T item){
            lock (queueLock)
            {
                store.Add(item);
            }
        }
        public T DeQueue() {
            lock (queueLock)
            {
                var item = store.FirstOrDefault();
                if (item != null)
                {
                    store.Remove(item);
                    return item;
                } 
                return default(T);
            }
        }
        public T Peek() {
            lock (queueLock)
            {
                var item = store.FirstOrDefault();
                if (item != null)
                {
                    return item;
                }
                return default(T);
            }
        }
        public int MessageCount() { return store.Count; }

        public IEnumerator<T> GetEnumerator() {
            lock (queueLock)
            {
                return store.GetEnumerator();
            }
        }

    }
}
