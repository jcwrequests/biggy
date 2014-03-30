#Biggy + TinyFS = TinyDB
Rob Conery's Biggy Open Source Project has basically turned <span>List&lt;T&gt;</span> into a simple easy to use Document Database using LINQ as it's query Language.

If you love using NoSQL solutions like MongoDB but wish they had an embedded version that could be used with smart clients then this is the project for you. 

Biggy has the <span>BiggyList&lt;T&gt;</span> which allows you to store one <span>List&lt;T&gt;</span> per file but if you have more then one <span>List&lt;T&gt;</span> and want them to be stored in a single store then you may want to consider TinyDB.

TinyDB combines the work of Benny Olsson's Tiny FS(https://github.com/Aztherion/TinyFS.Net) and Biggy. Tiny FS provides the storage and Biggy provides the rest.

The API and implementation were inspired by Mark Rendel's Simple.Data which relies on Dynamics to create a clean API.

Here is an example using Dynamic and type entity

```csharp
	public class Entity
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
	
    string dbFilePath = @".\tiny.db";
    dynamic db = new TinyDB(dbFilePath);
	//creates a table of type dynamic
    db.AddTable("test");
	//creates a table of type Entity
	db.AddTypedTable("test2", typeof(Entity));
	
	Entity document1 = new Entity("test",Date.Parse("01/01/2014"),1);
	Entity document2 = new Entity("test2", Date.Parse("01/01/2014"),2);
	
	db.test.Add(document1);
	db.test2.Add(document2);
	
	var table = db.test2;
	
	var doc = table.Where(d => d.Name.Equals("test").FirstOrDefault;
	
	TinyList<SomeEntity> testTable = db.test;
	var queryResult1 = 
		testTable.Where(i => i.SomeId == 1);

	var queryResult2 = 
		((TinyList<SomeEntity>)db.test2).Where(t => t.SomeId == 2);
	
	
```

Currently TinyDB has following API

Constructor TinyDB(string dbFileName) - full path to the DB File

Methods
..*
*TinyDB.AddTable(string TableName) - Adds a Dynamic TinyList<span>&lt;T&gt;</span>
*TinyDB.AddTypedTable(string TableName, Type EnityType) - Adds a Typed TinyList<span>&lt;T&gt;</span>
*TinyDB.RemoveTable(string TableName) - Removes Table from Store
*TinyDB.TableCount() - Returns the total number of tables
*TinyDB.Save() - Forces any changes to be flushed to disk
*TinyDB.Dispose()
...


