using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS.Perf
{
    class Monkey
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        [FullText]
        public string Description { get; set; }
    }
}
