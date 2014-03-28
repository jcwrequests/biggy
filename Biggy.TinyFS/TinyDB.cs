using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using Newtonsoft.Json;
using TinyFS;

namespace Biggy.TinyFS
{
    public class TinyDB : DynamicObject , IDisposable
    {
        ConcurrentDictionary<string, dynamic> tables;
        ConcurrentDictionary<string, Type> tableTypes;
        private EmbeddedStorage store;
        const string tinyTableTypes = "tinyTableTypes";

        public TinyDB(string dbFileName)
        {
            if (string.IsNullOrEmpty(dbFileName)) throw new ArgumentNullException("dbFileName");

            store = new EmbeddedStorage(dbFileName);
            tables = new ConcurrentDictionary<string, dynamic>();
            tableTypes = new ConcurrentDictionary<string, Type>();
            LoadTypes();

            store.
                Files().
                Where(f => f.Name != "tinyTableTypes").
                ToList().
                ForEach(f => 
                        {
                            Type listType;
                            tableTypes.TryGetValue(f.Name, out listType);
                            var table = CreateTable(f.Name, listType);
                            tables.TryAdd(f.Name, table);
                        }); 

           
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
                        tableTypes.TryAdd(key, typeof(object));
                        StoreTypes();
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
                        var newTable = CreateTable(tableName, type);
                        tables.TryAdd(key, newTable);
                        tableTypes.TryAdd(key, type);
                        StoreTypes();
                    }
                    return tables[key];
                };

                result = addTable;
                return true;

            }
            if (binder.Name.Equals("RemoveTable", StringComparison.InvariantCultureIgnoreCase))
            {
                Action<string> removeTable = (string tableName) =>
                    {
                        Type value;
                        tableTypes.TryRemove(tableName, out value);
                        store.Remove(tableName);
                        dynamic value2;
                        tables.TryRemove(tableName, out value2);

                    };
                result = removeTable;
                return true;
            }
            if (binder.Name.Equals("TableCount", StringComparison.InvariantCultureIgnoreCase))
            {
                result = tables.Count();
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
            tables.TryAdd(binder.Name, value);
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
        private  dynamic CreateTable(string tableName, System.Type type)
        {
            Type tableType = typeof(TinyList<>);
            Type[] typeArgs = { type };
            Type constructed = tableType.MakeGenericType(typeArgs);
            object[] args = { store, tableName };
            var newTable = Activator.CreateInstance(constructed, args);
            return newTable;
        }
        private void StoreTypes()
        {
            var json = JsonConvert.SerializeObject(tableTypes, Formatting.None);
            var buff = Encoding.Default.GetBytes(json);
            store.Write(tinyTableTypes, buff, 0, buff.Length);
        }
        private void LoadTypes()
        {
            if (!store.Exists(tinyTableTypes)) store.CreateFile(tinyTableTypes);

            var json = UTF8Encoding.UTF8.GetString(store.Read(tinyTableTypes));
            var result = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Type>>(json);
            if (result != null) this.tableTypes = result;
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
