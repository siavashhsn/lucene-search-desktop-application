using System;
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
        private SynchronizationContext MainThread;
        private Thread openfilethread;


        public static string[] vidioAudioFormats = new[]
        {
            ".webm", ".mpg", ".mp2", ".mpeg", ".mpe",
            ".mpv", ".ogg", ".mp4", ".m4p", ".m4v",
            ".mp3", ".m4a", ".aac", ".oga"
        };

        public static string[] imageformats = new[]
        {
            ".ani", ".bmp", ".cal", ".eps", ".fax",
            ".img", ".jbg", ".jpe", ".jpeg", "jpg",
            ".mac", ".pbm", ".pcd", ".pcx", ".pct",
            ".pgm", ".png", ".ppm", ".psd", ".ras",
            ".tga", ".tiff", ".wmf", ".gif"
        };

        public static string[] archiveformats = new[]
        {
            ".bz2",".F",".gz",".lz",".lzma",".lzo",
            ".rz",".sz",".xz",".z",".7z",".s7z",
            ".ace",".afa",".apk",".pak",".rar",
            ".zip",".zipx",".APK", ".deb",".RPM",
            ".MSI" ,".JAR",".tar.gz", ".tgz",
            ".tar.Z", ".tar.bz2",".tbz2", ".tar.lzma",
            ".tlz" ,".tar.xz", ".txz"
        };

        public static string[] UnreadableFormats = new[]
        {
            ".webm", ".mpg", ".mp2", ".mpeg", ".mpe",
            ".mpv", ".ogg", ".mp4", ".m4p", ".m4v",
            ".mp3", ".m4a", ".aac", ".oga",
            ".ani", ".bmp", ".cal", ".eps", ".fax",
            ".img", ".jbg", ".jpe", ".jpeg", "jpg",
            ".mac", ".pbm", ".pcd", ".pcx", ".pct",
            ".pgm", ".png", ".ppm", ".psd", ".ras",
            ".tga", ".tiff", ".wmf", ".gif",
            ".bz2",".F",".gz",".lz",".lzma",".lzo",
            ".rz",".sz",".xz",".z",".7z",".s7z",
            ".ace",".afa",".apk",".pak",".rar",
            ".zip",".zipx",".APK", ".deb",".RPM",
            ".MSI" ,".JAR",".tar.gz", ".tgz",
            ".tar.Z", ".tar.bz2",".tbz2", ".tar.lzma",
            ".tlz" ,".tar.xz", ".txz"
        };

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

        lucene index = new lucene();

        public Index()
        {
            InitializeComponent();
            browes_tbx.Text = "C:\\Users\\siavash\\Desktop\\iDesktop\\";
            result_tbx.Text = "opening iDesktop ...\n";
            filename = string.Empty;
            filepath = string.Empty;
            MainThread = SynchronizationContext.Current;
            if (MainThread == null) MainThread = new SynchronizationContext();
        }

        public void openFilesToBeIndex()
        {
            index.indexStart();

            MainThread.Send((object state) =>
            {

                index_btn.IsEnabled = false;
                bool result = false;

                string[] entries = Directory.GetFileSystemEntries(browes_tbx.Text, "*", SearchOption.AllDirectories);
                foreach (string file in entries)
                {
                    if (File.Exists(file))
                    {
                        fileContent.Clear();
                        filename = string.Empty;
                        filepath = string.Empty;

                        if (readableFormats.Any(Path.GetExtension(file).Contains))
                        {
                            string fx = Path.GetExtension(file);
                            if (fx == ".doc") { wordReader(file); }
                            else if (fx == ".docx") { wordReader(file); }
                            else if (fx == ".pdf") { pdfReader(file); }
                            else if (fx == ".ppt") { pptReader(file); }
                            else if (fx == ".xls") { xlsxReader(file); }
                            else if (fx == ".xlsx") { xlsxReader(file); }
                            else { txtReader(file); }
                        }
                    }
                    else
                    {
                        fileContent.Append("");
                    }

                    filename = Path.GetFileName(file);
                    filepath = file;


                    result = index.lucene_index(filepath, filename, fileContent);

                    if (result)
                        result_tbx.Text += "Indexed \t" + filename + "\n";
                    else
                        result_tbx.Text += "not Indexed \t" + filename + "\n";
                    result_tbx.ScrollToEnd();
                }
            }, null);
            index.indexClose();
        }


        public void show(bool r)
        {
            MainThread.Send((object state) =>
            {
                if (r)
                    result_tbx.Text += "Indexed \t" + filename + "\n";
                else
                    result_tbx.Text += "not Indexed \t" + filename + "\n";
                result_tbx.ScrollToEnd();

            }, r);

        }

        public void wordReader(string path)
        {
            TextExtractor extractor = new TextExtractor(path);
            fileContent.Append(extractor.ExtractText());
        }

        public void pdfReader(string path)
        {
            PdfReader reader = new PdfReader(path);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                fileContent.Append(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            reader.Close();
        }

        public void pptReader(string path)
        {
            result_tbx.Text += "still in process ... ";
        }

        public void xlsxReader(string path)
        {
            result_tbx.Text += "still in process ... ";
        }

        public void txtReader(string path)
        {
            //used from this side 
            //https://www.dotnetperls.com/file-readalltext
            using (StreamReader reader = new StreamReader(path))
            {
                fileContent.Append(reader.ReadToEnd());
            }
        }

        public void show(string name)
        {
            result_tbx.Text += string.Format("\n***********{0}***********\n\n{1}\n", name, fileContent);
        }

        private void index_btn_Click(object sender, RoutedEventArgs e)
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
                openfilethread = new Thread(new ThreadStart(openFilesToBeIndex));
                openfilethread.Start();
                result_tbx.Text += "\n...................done...................\n";
            }
            return;
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