using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyQueues
    {
        TinyDB db;

        public TinyQueues(TinyDB db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }
    }
}
