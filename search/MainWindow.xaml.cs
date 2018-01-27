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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace search
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search s = new Search();
            s.Show();
        }

        private void index_btn_Click(object sender, RoutedEventArgs e)
        {
            Index i = new Index();
            i.Show();
        }
    }
}
