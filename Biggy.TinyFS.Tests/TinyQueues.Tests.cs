using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Biggy.TinyFS.Tests
{
    [TestClass]
    public class TinyQueueTests
    {
        const string dbFilePath = @"./queues.db";
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
        public void CreateQueues()
        {
            TinyQueues queues = new TinyQueues(dbFilePath);
            queues.Dispose();
        }
      
        [TestMethod]
        public void CreateANewQueue()
        {
            TinyQueues queues = new TinyQueues(dbFilePath);

            var queue = queues.CreateQueue("test");
            Assert.IsFalse(queue == null);
            queues.Dispose();
        }

        [TestMethod]
        public void QueueAnItem()
        {
            TinyQueues queues = new TinyQueues(dbFilePath);

            var queue = queues.CreateQueue("test");
            queue.EnQueue(new { name = "test", time = DateTime.Now });
            Assert.IsTrue(queue.MessageCount() == 1);

            queues.Dispose();
        }

        [TestMethod]
        public void DeQueueAnItem()
        {
            TinyQueues queues = new TinyQueues(dbFilePath);

            var queue = queues.CreateQueue("test");
            queue.EnQueue(new { name = "test", time = DateTime.Now });
            var item = queue.DeQueue();
            Assert.IsTrue(queue.MessageCount() == 0);
            Assert.IsFalse(item == null);
            Assert.IsTrue(item.name == "test");
            queues.Dispose();
        }

        [TestMethod]
        public void AddItemCloseQueuesThenOpenAgainAndDequeueAnItem()
        {
            TinyQueues queues = new TinyQueues(dbFilePath);

            var queue = queues.CreateQueue("test");
            var now = DateTime.Now;
            queue.EnQueue(new { name = "test", time = now });
            queues.Dispose();
            queue = null;

            queues = new TinyQueues(dbFilePath);
            queue = queues.GetQueueByName("test");
            var item = queue.DeQueue();
            queues.Dispose();

            Assert.IsTrue(item.name == "test" && item.time == now);




        }
    }
}
