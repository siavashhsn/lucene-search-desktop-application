﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Path = System.IO.Path;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Code7248.word_reader;
using System.Text;
using System.Threading;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace search
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Window
    {
        public static StringBuilder fileContent = new StringBuilder();
        public static string filename;
        public static string filepath;
        private readonly SynchronizationContext sync;
        private DateTime dt = DateTime.Now;



        //public static string[] vidioAudioFormats = new[]
        //{
        //    ".webm", ".mpg", ".mp2", ".mpeg", ".mpe",
        //    ".mpv", ".ogg", ".mp4", ".m4p", ".m4v",
        //    ".mp3", ".m4a", ".aac", ".oga"
        //};

        //public static string[] imageformats = new[]
        //{
        //    ".ani", ".bmp", ".cal", ".eps", ".fax",
        //    ".img", ".jbg", ".jpe", ".jpeg", "jpg",
        //    ".mac", ".pbm", ".pcd", ".pcx", ".pct",
        //    ".pgm", ".png", ".ppm", ".psd", ".ras",
        //    ".tga", ".tiff", ".wmf", ".gif"
        //};

        //public static string[] archiveformats = new[]
        //{
        //    ".bz2",".F",".gz",".lz",".lzma",".lzo",
        //    ".rz",".sz",".xz",".z",".7z",".s7z",
        //    ".ace",".afa",".apk",".pak",".rar",
        //    ".zip",".zipx",".APK", ".deb",".RPM",
        //    ".MSI" ,".JAR",".tar.gz", ".tgz",
        //    ".tar.Z", ".tar.bz2",".tbz2", ".tar.lzma",
        //    ".tlz" ,".tar.xz", ".txz"
        //};

        //public static string[] UnreadableFormats = new[]
        //{
        //    ".webm", ".mpg", ".mp2", ".mpeg", ".mpe",
        //    ".mpv", ".ogg", ".mp4", ".m4p", ".m4v",
        //    ".mp3", ".m4a", ".aac", ".oga",
        //    ".ani", ".bmp", ".cal", ".eps", ".fax",
        //    ".img", ".jbg", ".jpe", ".jpeg", "jpg",
        //    ".mac", ".pbm", ".pcd", ".pcx", ".pct",
        //    ".pgm", ".png", ".ppm", ".psd", ".ras",
        //    ".tga", ".tiff", ".wmf", ".gif",
        //    ".bz2",".F",".gz",".lz",".lzma",".lzo",
        //    ".rz",".sz",".xz",".z",".7z",".s7z",
        //    ".ace",".afa",".apk",".pak",".rar",
        //    ".zip",".zipx",".APK", ".deb",".RPM",
        //    ".MSI" ,".JAR",".tar.gz", ".tgz",
        //    ".tar.Z", ".tar.bz2",".tbz2", ".tar.lzma",
        //    ".tlz" ,".tar.xz", ".txz"
        //};

        public static string[] readableFormats = new[]
        {
            ".htm", ".odt", ".xls", ".xlsx", ".ods",
            ".ppt", ".pptx", ".doc", ".docx", ".dot",
            ".dotx", ".html", ".pdf", ".ppa", ".txt",
            ".xml", ".xhtml", ".php", ".pl", ".py", ".y",
            ".r", ".rb", ".c", ".cpp", ".cc", ".cxx", ".vb",
            ".h", ".hpp", ".hxx", ".ml", ".php", ".css", ".cs",
            ".php3", ".php4", ".php5", ".phps", ".phtml",
            ".json", ".asp", ".aspx", ".l"
        };
        public static List<string> readableFormatsList = new List<string>(readableFormats);

        lucene index = new lucene();

        public Index()
        {
            InitializeComponent();
            sync = SynchronizationContext.Current;
            browes_tbx.Text = "E:\\";
            result_tbx.Text = "opening ...\n";
            filename = string.Empty;
            filepath = string.Empty;
        }


        public void wordReader(string path)
        {
            try
            {
                TextExtractor extractor = new TextExtractor(path);
                fileContent.Append(extractor.ExtractText());
            }
            catch(Exception ex)
            {
                fileContent.Append("");
            }
        }

        public void pdfReader(string path)
        {
            try
            {
                PdfReader reader = new PdfReader(path);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    fileContent.Append(PdfTextExtractor.GetTextFromPage(reader, page));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(path);
                fileContent.Append("");
            }
        }

        public void txtReader(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    fileContent.Append(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                fileContent.Append("");
            }
        }

        public void show(string name)
        {
            result_tbx.Text += string.Format("\n***********{0}***********\n\n{1}\n", name, fileContent);
        }

        private async void index_btn_Click(object sender, RoutedEventArgs e)
        {

            if (browes_tbx.Text == null)
            {
                MessageBox.Show("Please select directory path", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!Directory.Exists(browes_tbx.Text))
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", browes_tbx.Text));
                return;
            }
            else
            {
                index.indexStart();
                index_btn.IsEnabled = false;
                bool result = false;
                string path = browes_tbx.Text;
                //await Task.Run(() =>
                //{
                try
                {
                    Queue<string> queue = new Queue<string>();
                    queue.Enqueue(path);
                    while (queue.Count > 0)
                    {
                        path = queue.Dequeue();
                        try
                        {
                            foreach (string subDir in Directory.GetDirectories(path))
                            {
                                if (subDir != @"E:\$RECYCLE.BIN" && subDir != @"E:\System Volume Information" && subDir != @"E:\.Trash-1000")
                                    queue.Enqueue(subDir);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        string[] files = null;
                        try
                        {
                            files = Directory.GetFiles(path);
                        }
                        catch (Exception ex)
                        {
                        }
                        if (files != null)
                        {
                            foreach (string file in files)
                            {
                                try
                                {

                                    fileContent.Clear();

                                    filename = string.Empty;
                                    filepath = string.Empty;

                                    if (readableFormatsList.Contains(Path.GetExtension(file)) && file.Length < (100) * (1024 * 1024))
                                    {
                                        string fx = Path.GetExtension(file);
                                        if (fx == ".doc") { wordReader(file); }
                                        else if (fx == ".docx") { wordReader(file); }
                                        else if (fx == ".pdf") { pdfReader(file); }
                                        else { txtReader(file); }
                                    }
                                    else
                                    {
                                        fileContent.Append("");
                                    }

                                    filename = Path.GetFileName(file);
                                    filepath = file;

                                    await Task.Run(() =>
                                    {
                                        result = index.lucene_index(filepath, filename, fileContent);

                                        updateResultTextBox(filename);
                                       

                                    });

                                }
                                catch (Exception eee)
                                {
                                    MessageBox.Show($"{filepath}---{eee.Message}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message.ToString());
                }
                index.indexClose();
                //});
            }

            result_tbx.Text += "\n...................done...................\n";
            result_tbx.ScrollToEnd();
        }
        public int i = 0;

        private void updateResultTextBox(string _filename)
        {
            var timeNow = DateTime.Now;
            
            //Here we only refresh our UI each 50 ms  
            if ((DateTime.Now - dt).Milliseconds <= 50) return;
            sync.Post(new SendOrPostCallback(o =>
            {
                if (result_tbx.LineCount%200==0)
                {
                    var txt = "";
                    for(int j=result_tbx.LineCount-14; j<result_tbx.LineCount; j++)
                    {
                        txt += result_tbx.GetLineText(j);
                    }
                    result_tbx.Text = txt;
                }
                result_tbx.Text += (i++).ToString() +  " Indexed \t" + (string)o + "\n";
                result_tbx.ScrollToEnd();

            }), _filename);
            dt = timeNow;

        }

        private void browes_tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                index_btn_Click(this, e);
            }
        }
    }
}


