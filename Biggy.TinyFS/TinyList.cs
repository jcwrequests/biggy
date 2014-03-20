using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyFS;

namespace Biggy.TinyFS
{
    public class TinyList<T> :InMemoryList<T> where T: new() {

      private EmbeddedStorage store;
      private string listName;

      public bool InMemory { get; set; }
      

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
              //format for the deserializer...
              var json = "[" + UTF8Encoding.UTF8.GetString(store.Read(path)) + "]";
              result = JsonConvert.DeserializeObject<List<T>>(json);
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
        var json = JsonConvert.SerializeObject(item);
        //append the to the file
        var buffer = UTF8Encoding.UTF8.GetBytes(json.ToCharArray());
        store.Write(listName, buffer, 0, buffer.Length);
        
        base.Add(item);
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
        var json = JsonConvert.SerializeObject(this);
        var cleaned = json.Replace("[", "").Replace("]", "").Replace(",", Environment.NewLine);
        var buff = Encoding.Default.GetBytes(json);
        store.Remove(listName);
        store.CreateFile(listName);
        store.Write(listName,buff, 0, buff.Length);

        //using (var fs = File.OpenWrite(this.DbPath)) {
        //  fs.WriteAsync(buff, 0, buff.Length);
        //}
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
