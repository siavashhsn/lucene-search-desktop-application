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
            List<Tuple<string, string>> list;
            List<listitem> l = new List<listitem>();
            lucene lucenesearch = new lucene();
            lucenesearch.searchStart();
            list = lucenesearch.lucene_search(search_tbx.Text.ToLower());
            if (list != null)
            {
                foreach (var item in list)
                {
                    listitem li = new listitem();
                    li.name = item.Item2;
                    li.path = item.Item1;
                    l.Add(li);
                }

                result_ltv.ItemsSource = l;
                lucenesearch.searchClose();
            }

        }

        private void search_tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                search_btn_Click(this, e);
            }
        }

        private void openFile_btn_Click(object sender, RoutedEventArgs e)
        {
            if ((dynamic)result_ltv.Items.Count != 0)
            {
                if ((dynamic)result_ltv.SelectedItems.Count > 0)
                {
                    var selectedItem = (dynamic)result_ltv.SelectedItems[0];
                    if (selectedItem != null)
                        System.Diagnostics.Process.Start(selectedItem.path);
                }
            }
        }

        private void openFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            if ((dynamic)result_ltv.Items.Count != 0)
            {
                if ((dynamic)result_ltv.SelectedItems.Count > 0)
                {
                    var selectedItem = (dynamic)result_ltv.SelectedItems[0];
                    if (selectedItem != null)
                        System.Diagnostics.Process.Start("explorer.exe", "/select," + selectedItem.path);
                }
            }
        }
    }
    public class listitem
    {
        public string name { get; set; }
        public string path { get; set; }
    }
}
