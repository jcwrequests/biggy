using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biggy.TinyFS.Tests.ValueObjects;
using System.Linq;

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
        public void Get_Table()
        {
            dynamic db = new TinyDB(dbFilePath);
            TinyList<dynamic> test = db.test;
            Assert.IsFalse(test == null);
            db.Dispose();
        }
        [TestMethod]
        public void Add_Items_To_DB()
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

            db.test.Add(document);
            db.test2.Add(document2);

            Assert.IsTrue(db.test.Count().Equals(1));
            Assert.IsTrue(db.test2.Count().Equals(1));
           
            db.Dispose();
        }
        [TestMethod]
        public void Update_Items_In_DB()
        {
            dynamic db = new TinyDB(dbFilePath);
            TinyList<dynamic> test = db.test;
            SomeEntity document =
                new SomeEntity(name: "Tiny",
                               dateCreated: DateTime.Parse("03/17/2014"),
                               someId: 22);

            AnotherEntity document2 =
                new AnotherEntity(name: "Tiny",
                                  dateCreated: DateTime.Parse("03/17/2014"),
                                  someId: 22);

            test.Add(document);
            db.test2.Add(document2);
            document.Name = "Tiny2";
            document2.Name = "TinyTiny";

            db.Save();


           

            db.Dispose();
        }
    }
}
