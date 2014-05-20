using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public interface IDocument
    {
        string DocumentID { get; set; }
        int DocumentVersion { get; set; }
    }
}
