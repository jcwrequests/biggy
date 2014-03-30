using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biggy.TinyFS.Tests.ValueObjects;
using System.Linq;
using System.Collections.Generic;

namespace Biggy.TinyFS.Tests
{
    [TestClass]
    public class TinyDB_Tests
    {
        const string dbFilePath = @"./db.tiny";
        [TestInitialize]
        public void Setup()
        {
            if (System.IO.File.Exists(dbFilePath)) System.IO.File.Delete(dbFilePath);
            

        }
        [TestCleanup]
        public void TearDown()
        {
            if (System.IO.File.Exists(dbFilePath)) System.IO.File.Delete(dbFilePath);
        }
        [TestMethod]
        public void Create_DB()
        {
            dynamic db = new TinyDB(dbFilePath);
            Assert.IsFalse(db == null);
            db.Dispose();
        }
        [TestMethod]
        public void Create_Table()
        {
            dynamic db = new TinyDB(dbFilePath);
            TinyList<dynamic> test = db.AddTable("test");
            Assert.IsFalse(test == null);
            db.Dispose();
        }
        [TestMethod]
        public void Add_Items_To_DBTable()
        {
            dynamic db = new TinyDB(dbFilePath);
            
            SomeEntity document = 
                new SomeEntity(name: "Tiny",
                               dateCreated: DateTime.Parse("03/17/2014"),
                               someId: 22);

            AnotherEntity document2 =
                new AnotherEntity(name: "Tiny",
                                  dateCreated: DateTime.Parse("03/17/2014"),
                                  someId: 22);

            db.AddTable("test").Add(document);
            db.AddTable("test2").Add(document2);
            db.AddTypedTable("test3", typeof(SomeEntity)).Add(document);

            Assert.IsTrue(db.test.Count.Equals(1));
            Assert.IsTrue(db.test2.Count.Equals(1));
           
            db.Dispose();

        }
        [TestMethod]
        public void Update_Items_In_DB()
        {
            dynamic db = new TinyDB(dbFilePath);
            TinyList<dynamic> test = db.AddTable("test");

            SomeEntity document =
                new SomeEntity(name: "Tiny",
                               dateCreated: DateTime.Parse("03/17/2014"),
                               someId: 22);

            AnotherEntity document2 =
                new AnotherEntity(name: "Tiny",
                                  dateCreated: DateTime.Parse("03/17/2014"),
                                  someId: 22);

            test.Add(document);
            db.AddTable("test2").Add(document2);
            document.Name = "Tiny2";
            document2.Name = "TinyTiny";

            var result = ((TinyList<dynamic>)db.test).Where(t => t.SomeId == 22);
            
            db.Save();

            db.Dispose();
        }
        [TestMethod]
        public void Remove_Table_In_DB()
        {
            dynamic db = new TinyDB(dbFilePath);
            db.AddTable("test");
            db.AddTypedTable("test3", typeof(SomeEntity));
            Assert.IsTrue(db.TableCount.Equals(2));
            db.RemoveTable("test");
            Assert.IsTrue(db.TableCount.Equals(1));
            db.Dispose();
        }
        [TestMethod]
        public void Query_Tables_In_DB()
        {
            dynamic db = new TinyDB(dbFilePath);
       
            SomeEntity document =
                new SomeEntity(name: "Tiny",
                               dateCreated: DateTime.Parse("03/17/2014"),
                               someId: 22);

            AnotherEntity document2 =
                new AnotherEntity(name: "Tiny",
                                  dateCreated: DateTime.Parse("03/17/2014"),
                                  someId: 22);

            db.AddTable("test2").Add(document2);

            db.AddTypedTable("test", typeof(SomeEntity)).
                Add(document);


            TinyList<SomeEntity> testTable = db.test;
            var queryResult1 = testTable.Where(i => i.SomeId == 22);

            Assert.IsTrue(queryResult1.Count().Equals(1));

            var queryResult2 = ((TinyList<dynamic>)db.test2).Where(t => t.SomeId == 22);

            Assert.IsTrue(queryResult2.Count().Equals(1));
            db.Dispose();
        }
    }
}
