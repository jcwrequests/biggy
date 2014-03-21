using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS.Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading from File Store...");
            var monkies = new TinyList<Monkey>(@"./store.tiny");
            monkies.Clear();
            var sw = new Stopwatch();


            Action insert10000Documents = () =>
                {
                    Console.WriteLine("Inserting 10,000 documents");
                    sw.Reset();
                    sw.Start();
                    var addRange = new List<Monkey>();
                    for (int i = 0; i < 10000; i++)
                    {
                        addRange.Add(new Monkey { ID = i, Name = "MONKEY " + i, Birthday = DateTime.Today, Description = "The Monkey on my back" });
                    }

                    monkies.Add(addRange);

                    sw.Stop();
                    Console.WriteLine("Just inserted {0} as documents in {1} ms", monkies.Count(), sw.ElapsedMilliseconds);
                };

            Action loadDocuments = () =>
            {

                //use a DB that has an int PK
                sw.Reset();
                sw.Start();
                Console.WriteLine("Loading {0}...", monkies.Count);
                monkies.Reload();
                sw.Stop();
                Console.WriteLine("Loaded {0} documents in {1}ms", monkies.Count, sw.ElapsedMilliseconds);
            };

            Action queryDocuments = () =>
                {
                    sw.Reset();
                    sw.Start();
                    Console.WriteLine("Querying Middle 100 Documents");
                    var found = monkies.Where(x => x.ID > 100 && x.ID < 500);
                    sw.Stop();
                    Console.WriteLine("Queried {0} documents in {1}ms", found.Count(), sw.ElapsedMilliseconds);
                };

            insert10000Documents();
            loadDocuments();
            queryDocuments();

            
            Console.WriteLine("Complete");
            Console.ReadLine();

        }
    }
}
