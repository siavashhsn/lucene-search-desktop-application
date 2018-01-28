﻿using System;
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
    class lucene
    {

        private static Analyzer analyzer;
        private static Directory directory;
        private static IndexWriter writer;
        private static IndexReader reader;
        private static IndexSearcher searcher;
        public lucene()
        {
            analyzer = new StandardAnalyzer(Version.LUCENE_30);
            directory = FSDirectory.Open(new DirectoryInfo(Environment.CurrentDirectory + "\\LuceneIndex"));
            writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            reader = IndexReader.Open(directory, true);
            searcher = new IndexSearcher(reader);
        }

        public void writerclose()
        {
            reader.Dispose();
            writer.Dispose();
        }
        public void analyzerClose()
        {
            analyzer.Close();
        }

        public bool lucene_index(string path, string name, StringBuilder content)
        {
            try
            {
                var doc = new Document();
                doc.Add(new Field("path", path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("name", name.ToLower(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("content", content.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(doc);
                writer.Optimize();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(path + " can't be index...???!!!");
                return false;
            }
        }

        public List<Tuple<string, string>> lucene_search(string query)
        {
            writerclose();
            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(Version.LUCENE_30, new string[] { "name", "content" }, analyzer);
            TopDocs resultDocs = searcher.Search(queryParser.Parse(query), 10);
            var hits = resultDocs.ScoreDocs;

            List<Tuple<string, string>> s = new List<Tuple<string, string>>();
            foreach (var hit in hits)
            {
                var documentfromsearch = searcher.Doc(hit.Doc);
                s.Add(Tuple.Create(documentfromsearch.Get("path"), documentfromsearch.Get("name")));
            }
            searcher.Dispose();
            return s;
        }
    }
}