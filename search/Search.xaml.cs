using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace search
{
    public partial class Search : Window
    {
        public Search()
        {
            InitializeComponent();
        }


        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            List<Tuple<string, Tuple<string, string>>> l;
            List<
            lucene lucenesearch = new lucene();
            lucenesearch.searchStart();
            l = lucenesearch.lucene_search(search_tbx.Text.ToLower());
            foreach (var item in l)
            {
                result_tbx.Text += item.Item1 + "\n*************\n" + item.Item2.Item1 + "\n*************\n" + item.Item2.Item2 + "\n";
            }
            lucenesearch.searchClose();

        }

        private void search_tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                search_btn_Click(this, e);
            }
        }
    }
    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Mail { get; set; }
    }
}
