using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyFS;
using System.Linq.Expressions;

namespace Biggy.TinyFS
{
    public class TinyList<T> :InMemoryList<T>, ITinyTable where T: new()  {

      private EmbeddedStorage store;
      private string listName;

      public bool InMemory { get; set; }
      
      public TinyList(EmbeddedStorage store, string listName)
      {
          if (store == null) throw new ArgumentNullException("store");
          this.store = store;
          this.InMemory = false;
          this.listName = listName;
          _items = TryLoadFileData(this.listName); 
      }
      public TinyList(string dbFileName)
      {

        this.InMemory = false;

        if (String.IsNullOrWhiteSpace(dbFileName)) throw new ArgumentNullException("dbFileName");
        
        var thingyType = this.GetType().GenericTypeArguments[0].Name;

        this.listName = Inflector.Inflector.Pluralize(thingyType).ToLower();
        store = new EmbeddedStorage(dbFileName);
        _items = TryLoadFileData(this.listName);       
      }

      
       public List<T> TryLoadFileData(string path) {

        List<T> result = new List<T>();
        if (store.Exists(path)) 
            {
                var json = UTF8Encoding.UTF8.GetString(store.Read(path));
                result = JsonConvert.DeserializeObject<List<T>>(json);
                if (result == null) result = _items;
            }
            else
            {
                store.CreateFile(path);
            }
        
        FireLoadedEvents();

        return result;
      }

      public void Reload() {
        _items = TryLoadFileData(this.listName);
      }

      public void Update(T item) {
        base.Update(item);
        this.FlushToDisk();
      }

      public void Add(T item) {
        base.Add(item);
        this.FlushToDisk();
      }

      public void Add(List<T> items)
      {
          items.ForEach(i => base.Add(i));
          this.FlushToDisk();
      }

      
      public void Clear() {
        base.Clear();
        this.FlushToDisk();
      }


      public bool Remove(T item) {
        var removed = base.Remove(item);
        this.FlushToDisk();
        return removed;
      }


      public bool FlushToDisk() {
      
        var json = JsonConvert.SerializeObject(this,Formatting.None);
        var buff = Encoding.Default.GetBytes(json);
        store.Write(listName,buff, 0, buff.Length);

        return true;
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
              store.Dispose();
          }

          // Free any unmanaged objects here. 
          //
          disposed = true;
      }

     
    }
}
