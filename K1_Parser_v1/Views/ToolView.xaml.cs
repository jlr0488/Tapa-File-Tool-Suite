using K1_Parser_v1.Components;
using K1_Parser_v1.Tool_Logic;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace K1_Parser_v1.Views
{
    /// <summary>
    /// Interaction logic for ToolView.xaml
    /// </summary>
    public partial class ToolView : System.Windows.Controls.UserControl
    {
        private string _outputDirectory;

        public ToolView()
        {
            InitializeComponent();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            var response = openFileDialog.ShowDialog();
            if (response == System.Windows.Forms.DialogResult.OK)
            {
                _outputDirectory = openFileDialog.SelectedPath;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_outputDirectory == null)
            {
                MessageBox.Show("You must select an output directory location first!", "Output Directory Not Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                //Get Selected Files
                string[] files = openFileDialog.FileNames;

                //Iterate and add all selected files to upload
                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);

                    string filePath = System.IO.Path.GetFullPath(files[i]);

                    // create a new parser object and parse through the file
                    var parser = new K1_Processor();

                    parser.OutputDirectory = _outputDirectory;

                    if (System.IO.Path.GetExtension(filename) == ".pdf")
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        UploadingFilesList.Items.Add(new fileDetail()
                        {
                            FileName = filename,

                            //To Convert bytes to Mb => bytes / 1.049e+6
                            FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                            UploadProgress = 100
                        });
                    }

                    // parse the file
                    parser.LoadPDF(filePath);
                }

                string outputMessage = "The following K1 files were successfully processed:";

                foreach(var file in files)
                {
                    outputMessage = outputMessage + "\n\n" + file;
                }

                MessageBox.Show(outputMessage, "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (_outputDirectory == null)
            {
                MessageBox.Show("You must select an output directory location first!", "Output Directory Not Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Checking what kind of file is User dropping 
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                //Iterate and add all selected files to upload
                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);

                    string filePath = System.IO.Path.GetFullPath(files[i]);

                    // create a new parser object and parse through the file
                    var parser = new K1_Processor();

                    parser.OutputDirectory = _outputDirectory;

                    if (System.IO.Path.GetExtension(filename) == ".pdf")
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        UploadingFilesList.Items.Add(new fileDetail()
                        {
                            FileName = filename,

                            //To Convert bytes to Mb => bytes / 1.049e+6
                            FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                            UploadProgress = 100
                        });
                    }

                    // parse the file
                    parser.LoadPDF(filePath);
                }

                string outputMessage = "The following K1 files were successfully processed:\n\n";

                foreach (var file in files)
                {
                    outputMessage = outputMessage + "\n\n" + file;
                }

                MessageBox.Show(outputMessage, "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
