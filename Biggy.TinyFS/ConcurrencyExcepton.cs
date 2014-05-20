using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biggy.TinyFS
{
    public class ConcurrencyExcepton : Exception
    {
        public ConcurrencyExcepton(string message) : base(message) { }
    }
}
