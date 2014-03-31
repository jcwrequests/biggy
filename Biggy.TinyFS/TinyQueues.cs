using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyQueues
    {
        dynamic db;

        public TinyQueues(TinyDB db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }
        public TinyQueue<T> CreateQueue<T>(string queueName)
        {
           var queue = db.AddTypedTable(queueName, typeof(T));

           return new TinyQueue<T>(queue);

        }
        public TinyQueue<dynamic> CreateQueue(string queueName)
        {
            var queue = db.AddTable(queueName);
            return new TinyQueue<dynamic>(queue);
        }
    }
}
