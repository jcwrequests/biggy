﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.Postgres {
  public class PGList<T> : ICollection<T> where T : new() {

    
    List<T> _items = null;
    public bool InMemory { get; set; }
    public string TableName { get; set; }
    public string ConnectionString { get; set; }
    public PGTable<T> Model { get; set; }

    public event EventHandler ItemRemoved;
    public event EventHandler ItemAdded;
    public event EventHandler Changed;
    public event EventHandler Loaded;


    public PGList(string connectionStringName, string tableName = "guess", string primaryKeyName = "id") {
      this.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
      if (tableName!="guess") {
        this.TableName = tableName;
      } else {
        var thingyType = this.GetType().GenericTypeArguments[0].Name;
        this.TableName = Inflector.Inflector.Pluralize(thingyType).ToLower();
      }
      this.Model = new PGTable<T>(connectionStringName, this.TableName, primaryKeyName);
      this.Reload();

      if (this.Loaded != null) {
        var args = new BiggyEventArgs<T>();
        args.Items = _items;
        this.Loaded.Invoke(this, args);
      }
    }

    public IEnumerable<T> Query(string sql, params object[] args) {
      return this.Model.Query<T>(sql, args);
    }

    public void Purge() {
      this.Clear();
    }

    public void Reload() {
      _items = this.Model.All<T>().ToList();
    }

    public int Update(T item) {
      var updated = 0;
      var index = _items.IndexOf(item);
      if (index > -1) {
        updated = this.Model.Update(item);
        _items.RemoveAt(index);
        _items.Insert(index, item);
      }
      return updated;
    }

    public void Add(T item) {
      this.Model.Insert(item);
      _items.Add(item);

      if (this.ItemAdded != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.ItemAdded.Invoke(this, args);
      }
      if (this.Changed != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.Changed.Invoke(this, args);
      }
    }

    public int AddRange(List<T> items) {
        int affected = this.Model.BulkInsert(items);
        this.Reload();
        return affected;
    }
      
    public void Clear() {
      _items.Clear();
      this.Model.DeleteWhere("");
      if (this.Changed != null) {
        var args = new BiggyEventArgs<T>();
        this.Changed.Invoke(this, args);
      }
    }

    public bool Contains(T item) {
      return _items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
      _items.CopyTo(array, arrayIndex);
    }

    public int Count {
      get { return _items.Count; }
    }

    public bool IsReadOnly {
      get { return false; }
    }

    public bool Remove(T item) {
      this.Model.Delete(this.Model.GetPrimaryKey(item));
      if (this.ItemRemoved != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.ItemRemoved.Invoke(this, args);
      }
      if (this.Changed != null) {
        var args = new BiggyEventArgs<T>();
        args.Item = item;
        this.Changed.Invoke(this, args);
      }
      return _items.Remove(item);
    }

    public int RemoveSet(IEnumerable<T> list) {
      var removed = 0;
      if (list.Count() > 0) {
        //remove from the DB
        var keyList = new List<string>();
        foreach (var item in list) {
          keyList.Add(this.Model.GetPrimaryKey(item).ToString());
        }
        var keySet = String.Join(",", keyList.ToArray());
        var inStatement = this.Model.PrimaryKeyField + " IN (" + keySet + ")";
        removed = this.Model.DeleteWhere(inStatement, "");

        this.Reload();
      }
      return removed;
    }

    public IEnumerator<T> GetEnumerator() {
      return _items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return _items.GetEnumerator();
    }

  }
}
