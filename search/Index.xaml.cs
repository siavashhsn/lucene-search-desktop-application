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

namespace search
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Window
    {
        public static StringBuilder fileContent = new StringBuilder();

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
            ".h", ".hpp", ".hxx", ".ml", ".php", ".css",
            ".php3", ".php4", ".php5", ".phps", ".phtml",
            ".json", ".asp", ".aspx", ".l"
        };

        public Index()
        {
            InitializeComponent();
            browes_tbx.Text = "C:\\Users\\siavash\\Desktop\\iDesktop\\Sias Files\\";
            result_tbx.Text = "opening E:\\Sias Files ...\n";
            //index_btn_Click(null, null);
            
        }

        public void openfile()
        {
            if (browes_tbx.Text == null)
            {
                MessageBox.Show("Please select directory path", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Directory.Exists(browes_tbx.Text))
            {
                foreach (string file in Directory.EnumerateFiles(browes_tbx.Text))
                {
                    fileContent.Clear();

                    if (readableFormats.Any(Path.GetExtension(file).Contains))
                    {
                        switch (Path.GetExtension(file))
                        {
                            case ".doc": { wordReader(file); break; }
                            case ".docx": { wordReader(file); break; }
                            case ".pdf": { pdfReader(file); break; }
                            case ".ppt": { pptReader(file); break; }
                            case ".xls": { xlsxReader(file); break; }
                            case ".xlsx": { xlsxReader(file); break; }
                            default: txtReader(file); break;
                        }
                    }
                    else
                    {
                        nameReader(file);
                    }
                    show(Path.GetFileName(file));
                }
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", browes_tbx.Text));
            }
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
            fileContent.Append("still in process ... ");
        }

        public void xlsxReader(string path)
        {
            fileContent.Append("still in process ... ");
        }

        public void nameReader(string path)
        {
            fileContent.Append(Path.GetFileName(path) + Path.GetExtension(path));
        }

        public void txtReader(string path)
        {
            //fileContent = File.ReadAllText(path);
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
            luceneIndex l = new luceneIndex();
            string s = l.lucene_index();
            result_tbx.Text = "*******search result*******\n" + s;
            //openfile();

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
