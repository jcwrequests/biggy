﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

using TinyFS;

namespace Biggy.TinyFS
{
    public class TinyDB : DynamicObject , IDisposable
    {
        ConcurrentDictionary<string, dynamic> tables;
        private EmbeddedStorage store;

        public TinyDB(string dbFileName)
        {
            if (string.IsNullOrEmpty(dbFileName)) throw new ArgumentNullException("dbFileName");

            store = new EmbeddedStorage(dbFileName);
            tables = new ConcurrentDictionary<string, dynamic>();
            store.Files().ForEach(f => tables.TryAdd(f.Name, new TinyList<dynamic>(store,f.Name))); 
           
          
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.Equals("AddTable", StringComparison.InvariantCultureIgnoreCase))
            {
                 Func<string,TinyList<dynamic>> addTable = (string tableName) => 
                 {
                    string key = tableName;
                    if (!tables.ContainsKey(key))
                    {
                        tables.TryAdd(key,new TinyList<dynamic>(store,key));
                    }
                    return tables[key];
                 };

                 result = addTable;
                 return true;

            }
            if (binder.Name.Equals("AddTypedTable", StringComparison.InvariantCultureIgnoreCase))
            {
                Func<string,Type, dynamic> addTable = (string tableName, System.Type type) =>
                {
                    string key = tableName;
                    if (!tables.ContainsKey(key))
                    {
                        Type tableType = typeof(TinyList<>);
                        Type[] typeArgs = { type };
                        Type constructed = tableType.MakeGenericType(typeArgs);
                        object[] args = {store,key};
                        var newTable = Activator.CreateInstance(constructed,args);
                        tables.TryAdd(key, newTable);
                    }
                    return tables[key];
                };

                result = addTable;
                return true;

            }
            if (!tables.ContainsKey(binder.Name)) return base.TryGetMember(binder, out result);
           
            result = tables[binder.Name];
            return true;
           
        }
       
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (tables.ContainsKey(binder.Name)) return false;
            if (!value.GetType().BaseType.IsGenericType) return false;
            if (!value.GetType().BaseType.Equals(typeof(TinyList<>))) return false;
            tables.TryAdd(binder.Name, (TinyList<dynamic>)value);
            return true;

        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count().Equals(1) && indexes[0].GetType().Equals(typeof(string)))
            {
                string key = (string)indexes[0];
                if (tables.ContainsKey(key))
                {
                    result = tables[key];
                    return true;
                }
            }
            return base.TryGetIndex(binder, indexes, out result);
        }
       
        public void Save()
        {
            tables.Values.ToList().ForEach(t => t.FlushToDisk());
        }
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                tables.Values.ToList().ForEach(t => t.FlushToDisk());
                store.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }
}
