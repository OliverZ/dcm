using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.WindowsAPICodePack.Dialogs;


namespace dcmeditor
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        String fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe"); 

        private void ChooseDirButton(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder with dicom files";
            
            var selectedPath = "";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
                string filename = selectedPath.ToString();
                path.Text = System.IO.Path.Combine(filename, "*");
            }          
              
        }

        private void ButtonRenameTo(object sender, RoutedEventArgs e)
        { 
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -ma \"(0010,0010)={0}\" {1}", patientsName.Text, path.Text));
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void ButtonCustomTag(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -ma \"({0})={1}\" {2}", customTag.Text, customValue.Text, path.Text));
            dbgblock.Text = string.Format("-nb -ma \"({0})={1}\" {2}", customTag.Text, customValue.Text, path.Text);

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            
            p.Start();

            while (!p.HasExited) { } // wait till process ends

            var errorlevel = p.ExitCode.ToString();
            if ( p.HasExited && (errorlevel != "0") )
            {
                StreamReader standardOutputReader = p.StandardOutput;
                StreamReader errorStreamReader = p.StandardError;
                showErrorMessageBox(errorlevel, standardOutputReader, errorStreamReader);
            }  
        }

        private static void showErrorMessageBox(String errorlevel, StreamReader standardOutputReader, StreamReader errorStreamReader)
        {
            MessageBox.Show(
                "An Error Occured\nErrorlevel: " + errorlevel + "\n" + standardOutputReader.ReadToEnd() + "\n" + errorStreamReader.ReadToEnd(),
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ButtonID(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -ma \"(0010,0020)={0}\" {1}", patientsID.Text, path.Text));
            p.Start();
        }

        private void ComboBoxDicomTagsLoaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            int counter = 0;
            string line;
            data.Add("tag");
            var fullPathToTagList = System.IO.Path.Combine("dcmtk", "dicomtaglist.txt");
            System.IO.StreamReader file = new System.IO.StreamReader(fullPathToTagList);
            while ( ( line = file.ReadLine() ) != null )
            {
                data.Add(line);
                counter++;
            }
            file.Close();
             
            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        private void ComboBoxDicomTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            string value = comboBox.SelectedItem as string;
        
            string[] stringSeparators = new string[] { "   " };
            string[] result;

            result = value.Split(stringSeparators, StringSplitOptions.None);
            var onlyNumberFromTagList = result[0];
            customTag.Text = onlyNumberFromTagList;
        }
    }
}
