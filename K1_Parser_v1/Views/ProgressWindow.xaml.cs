using System.Windows;
using System.Windows.Controls;

namespace K1_Parser_v1.Views
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void SetProgress(string fileName, double percentage)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressTextBlock.Text = $"{fileName} ({percentage:P0} complete)";
                ProgressBar.Value = percentage * 100;
            });
        }
    }
}
