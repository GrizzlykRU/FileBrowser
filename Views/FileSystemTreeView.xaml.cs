using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace FileBrowser.Views
{
    /// <summary>
    /// Логика взаимодействия для FileSystemTreeView.xaml
    /// </summary>
    public partial class FileSystemTreeView : UserControl
    {
        public FileSystemTreeView()
        {
            InitializeComponent();
        }

        private void btn_ExpanderClick(object sender, RoutedEventArgs e)
        {
            Expander.Content = Expander.Content.ToString() == "4" ? "6" : "4";
        }
    }
}
