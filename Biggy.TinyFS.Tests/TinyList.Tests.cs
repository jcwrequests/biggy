using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biggy.TinyFS.Tests.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace Biggy.TinyFS.Tests
{
    [TestClass]
    public class TinyList_Tests
    {
        const string db = @"./db.tiny";
        private Biggy.TinyFS.TinyList<SomeEntity> tinyList;

        [TestInitialize]
        public void Setup()
        {
            if (System.IO.File.Exists(db)) System.IO.File.Delete(db);
            tinyList = new Biggy.TinyFS.TinyList<SomeEntity>(db);

        }
        [TestCleanup]
        public void TearDown()
        {
            tinyList.Dispose();
            if (System.IO.File.Exists(db)) System.IO.File.Delete(db);
        }

        [TestMethod]
        public void LoadsEmpty()
        {
            Assert.IsTrue(tinyList.Count.Equals(0));
        }

        [TestMethod]
        public void LoadSingleDocument()
        {
            SomeEntity document = new SomeEntity(name: "Tiny",
                                                 dateCreated: DateTime.Parse("03/17/2014"),
                                                 someId: 22);


            tinyList.Add(document);

            Assert.IsTrue(tinyList.Count.Equals(1));

            var doc = tinyList.First();
            Assert.IsTrue(document.DateCreated.Equals(doc.DateCreated));
            Assert.IsTrue(document.Name.Equals(doc.Name));
            Assert.IsTrue(document.SomeId.Equals(doc.SomeId));

        }
        [TestMethod]
        public void WriteALot()
        {
            var data = new List<SomeEntity>();
            for (var i = 0; i < 10000; i++)
            {
                data.Add(new SomeEntity
                    {
                        Name = "A widget",
                        DateCreated = DateTime.Now.AddYears(1),
                        SomeId = i
                    });
            }

            data.ForEach(e => tinyList.Add(e));

            Assert.IsTrue(data.Count.Equals(tinyList.Count));

        }
        [TestMethod]
        public void Query()
        {
            var data = new List<SomeEntity>();
            for (var i = 0; i < 30; i++)
            {
                data.Add(new SomeEntity
                {
                    Name = "A widget",
                    DateCreated = DateTime.Now.AddYears(1),
                    SomeId = i
                });
            }

            data.ForEach(e => tinyList.Add(e));

            var query = from e in tinyList
                        where e.SomeId >= 5 & e.SomeId <= 10
                        select e;

            Assert.IsTrue(query.Count().Equals(6));
        }
        [TestMethod]
        public void Update()
        {
            SomeEntity document = new SomeEntity(name: "Tiny",
                                                 dateCreated: DateTime.Parse("03/17/2014"),
                                                 someId: 22);


            tinyList.Add(document);
            document.Name = "TEST";
            tinyList.FlushToDisk();

            var result = tinyList.Where(e => e.SomeId.Equals(22)).First();
            Assert.IsTrue(result.Name.Equals(document.Name));
        }
        [TestMethod]
        public void Delete()
        {
            SomeEntity document = new SomeEntity(name: "Tiny",
                                                 dateCreated: DateTime.Parse("03/17/2014"),
                                                 someId: 22);

            tinyList.Add(document);
            tinyList.Remove(document);

            Assert.IsTrue(tinyList.Count.Equals(0));
        }
    }

}
