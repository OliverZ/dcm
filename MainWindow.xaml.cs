using System;
using System.Collections.Generic;
using System.Diagnostics;
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


namespace WpfApplication1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseDirButton(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder with dicom files";
            
            var selectedPath = "";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
                // Open document 
                string filename = selectedPath.ToString();
                // Write Filename to TextBox
                path.Text = System.IO.Path.Combine(filename, "*");
            }          
              
        }

        private void ButtonRenameTo(object sender, RoutedEventArgs e)
        { 
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo = new ProcessStartInfo(@"dcmtk\dcmodify.exe", string.Format("-nb -m \"(0010,0010)={0}\" {1}", patientsName.Text, path.Text));
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void ButtonCustomTag(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            var fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe"); 
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -m \"({0})={1}\" {2}", customTag.Text, customValue.Text, path.Text));
            dbgblock.Text = string.Format("-nb -m \"({0})={1}\" {2}", customTag.Text, customValue.Text, path.Text);
            p.Start();
        }

        private void Button_ID(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            var fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe");
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -m \"(0010,0020)={0}\" {1}", patientsID.Text, path.Text));
            p.Start();
        }

        private void ButtonTagList(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            var fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe");

            var onlyNumberFromTagList = dcmtagcombobox.Text.Split(new Char[] {' '});
            dbgblock.Text = string.Format("-nb -m \"({0})={1}\" {2}", onlyNumberFromTagList[0], customValueForTagList.Text, path.Text);
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -m \"({0})={1}\" {2}", onlyNumberFromTagList[0], customValueForTagList.Text, path.Text));
            p.Start();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            int counter = 0;
            string line;
            data.Add("tag");
            // Read the file and display it line by line.
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
