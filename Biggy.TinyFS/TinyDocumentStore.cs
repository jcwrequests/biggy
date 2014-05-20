using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biggy.TinyFS
{
    public class TinyDocumentStore<TDocument> where TDocument : class, IDisposable, IDocument, new()
    {
        private dynamic store;
        TinyList<TDocument> documents;
        object _lock;

        public TinyDocumentStore(string filePath)
        {
            _lock = new Object();
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            store = new TinyDB(filePath);

            if (store.TableCount().Equals(0))
            {
                documents = store.AddTypedTable("Documents", typeof(TDocument));
            }
            else
            {
                documents = store.Documents;
            }
            
        
        }
        public IEnumerable<TDocument> GetLastestDocuments() 
        {
            lock (_lock)
            {
                IEnumerable<TDocument> result =
                        documents.
                            OrderBy((TDocument d) => d.DocumentID).
                            OrderByDescending((TDocument d) => d.DocumentVersion).
                            GroupBy(keySelector: (TDocument k) => k.DocumentID,
                                    resultSelector: (string k, IEnumerable<TDocument> e) => e.First()).
                                    Select((TDocument d, int i) => d);
                return result;
            }
        }
        public TDocument GetDocument(string  documentID)
        {
            TDocument document;

            lock (_lock)
            {
                document =
                    documents.Where(d => d.DocumentID.Equals(documentID)).
                    OrderBy(d => d.DocumentID).
                    OrderByDescending(v => v.DocumentVersion).
                    FirstOrDefault();

            }
            return document;
        }
      
        public void InsertDocument(TDocument document)
        {
            TDocument lastDocument = GetDocument(document.DocumentID);
            if (lastDocument != null) throw new Exception(string.Format("Document {0} already exists",document.DocumentID));

            documents.Add(document);
        }
        public void InsertDocuments(List<TDocument> items)
        {
            if (!documents.Count().Equals(0)) throw new Exception("This Can only be called once to initialize the store.");
            documents.Add(items);
        }
        public void UpdateDocument(TDocument document)
        {
            var lastDocument = GetDocument(document.DocumentID);
            if (lastDocument == null) throw new Exception(string.Format("Document {0} does not exist",document.DocumentID));

            lock (_lock)
            {
                int lastVersion = lastDocument.DocumentVersion;
                int currentVersion = document.DocumentVersion;

                if ((lastVersion + 1) != currentVersion) throw new ConcurrencyExcepton(string.Format("Document ID: {0}", document.DocumentID));
                documents.Add(document);
            }
        }
    
    }    



    
}
