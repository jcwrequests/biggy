using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biggy.TinyFS.Tests.ValueObjects
{
    public class SomeEntity
    {
        public SomeEntity() { }
        public SomeEntity(string name, DateTime dateCreated, int someId)
        {
            this.Name = name;
            this.DateCreated = dateCreated;
            this.SomeId = someId;
        }
        public string Name { get;  set; }
        public DateTime DateCreated { get;  set; }
        public int SomeId { get;  set; }
    }
}
