using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyQueues : IEnumerable<dynamic> , IDisposable
    {
        dynamic db;
        
        ConcurrentDictionary<string, dynamic> queues;

        public TinyQueues(TinyDB db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            db.Cast<KeyValuePair<string, dynamic>>().
                ToList().
                ForEach(pair => queues.TryAdd(pair.Key, pair.Value));

        }
        public TinyQueue<T> CreateQueue<T>(string queueName)
        {
           
           var table = db.AddTypedTable(queueName, typeof(T));
           var queue = new TinyQueue<T>(table,queueName);
           queues.TryAdd(queueName, queue);
           return queue;
        }
        public TinyQueue<dynamic> CreateQueue(string queueName)
        {
            var table = db.AddTable(queueName);
            var queue = new TinyQueue<dynamic>(table,queueName);
            queues.TryAdd(queueName, queue);
            return queue;
        }

        public IEnumerator<dynamic> GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.queues.Values.GetEnumerator();
        }

        public void Dispose()
        {
            if (db != null) db.Dispose();
        }
    }
}
