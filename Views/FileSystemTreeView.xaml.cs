using System.Windows;
using System.Windows.Controls;

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
