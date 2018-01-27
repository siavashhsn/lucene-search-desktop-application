using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Lucene.Net.Analysis;
using System.Windows;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace search
{
    class luceneIndex
    {

        private static Analyzer analyzer;
        private static Directory directory;
        private static IndexWriter writer;
        private static IndexReader reader;
        private static IndexSearcher searcher;
        public luceneIndex() {
            analyzer = new StandardAnalyzer(Version.LUCENE_30);
            directory = FSDirectory.Open(new DirectoryInfo(Environment.CurrentDirectory + "\\LuceneIndex"));
            writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
           
        }

        public void opensercher()
        {
            reader = IndexReader.Open(directory, true);
            searcher = new IndexSearcher(reader);
        }

        public void writerclose()
        {
            analyzer.Close();
            writer.Dispose();
        }

        //var fordFiesta = new Document();
        //fordFiesta.Add(new Field("Id", "1", Field.Store.YES, Field.Index.NOT_ANALYZED));
        //fordFiesta.Add(new Field("Make", "ford", Field.Store.YES, Field.Index.NOT_ANALYZED));
        //fordFiesta.Add(new Field("Model", "Fiesta", Field.Store.YES, Field.Index.NOT_ANALYZED));

        //var vauxhallAstra = new Document();
        //vauxhallAstra.Add(new Field("Id", "2", Field.Store.YES, Field.Index.NOT_ANALYZED));
        //vauxhallAstra.Add(new Field("Make", "Vauxhall", Field.Store.YES, Field.Index.NOT_ANALYZED));
        //vauxhallAstra.Add(new Field("Model", "Astra", Field.Store.YES, Field.Index.NOT_ANALYZED));
        public void lucene_index(Document doc)
        {
            writer.AddDocument(doc);
            writer.Optimize();
        }

    

        public void lucene_search()
        {
            //var queryParser = new QueryParser(Version.LUCENE_30, "Make", analyzer);
            //var query = queryParser.Parse("ford")
            TopDocs resultDocs = searcher.Search(query, 1);
            var hits = resultDocs.ScoreDocs;

            string s = string.Empty;
            foreach (var hit in hits)
            {
                var documentfromsearch = searcher.Doc(hit.Doc);
                s += documentfromsearch.Get("Make") + " " + documentfromsearch.Get("Model");
            }
        }

        
    }

}
