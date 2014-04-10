using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyQueues :  IDisposable
    {
        dynamic db;
        
        ConcurrentDictionary<string, dynamic> queues;

        public TinyQueues(string queuePath)
        {
            if (queuePath == null) throw new ArgumentNullException("queuePath");
            db = new TinyDB(queuePath);
          
            queues = new ConcurrentDictionary<string, dynamic>(StringComparer.InvariantCultureIgnoreCase);

            ((TinyDB)db).Cast<KeyValuePair<string, dynamic>>().
                ToList().
                ForEach(pair => queues.TryAdd(pair.Key,new TinyQueue<dynamic>(pair.Value,pair.Key)));

        }
        public TinyQueue<T> CreateQueue<T>(string queueName) where T : class, new()
        {
            if (!queues.ContainsKey(queueName))
            {
                var table = db.AddTypedTable(queueName, typeof(T));
                var queue = new TinyQueue<T>(table,queueName);
                queues.TryAdd(queueName, queue);
                return queue;
            }
            throw new Exception(string.Format("{0} queue Exists", queueName));
        }
        public TinyQueue<dynamic> CreateQueue(string queueName)
        {
            if (!queues.ContainsKey(queueName))
            {
                var table = db.AddTable(queueName);
                var queue = new TinyQueue<dynamic>(table,queueName);
                queues.TryAdd(queueName, queue);
                return queue;
            }
            throw new Exception(string.Format("{0} queue Exists", queueName));   
        }

        public IEnumerable<string> QueueNames()
        {
            return queues.Keys;
        }
        public TinyQueue<dynamic> GetQueueByName(string queueName)
        {
            if (queues.ContainsKey(queueName)) return queues[queueName];
            return null;
        }
        public void Dispose()
        {
            if (db != null) db.Dispose();
        }
    }
}
