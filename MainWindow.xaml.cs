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
using System.Windows.Forms;
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
        String fullPathToDcmdump = System.IO.Path.Combine("dcmtk", "dcmdump.exe");

        private void ChooseDirButton(object sender, RoutedEventArgs e)
        {

            if (CommonOpenFileDialog.IsPlatformSupported)
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
                    SurroundPathWithQuotes();
                }
            }
            else
            {
                // Windows Server 2003
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Select folder with dicom files";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    path.Text = System.IO.Path.Combine(dialog.SelectedPath, "*");
                    SurroundPathWithQuotes();
                }
            }    
        }

        private void DumpButton(object sender, RoutedEventArgs e)
        {
            Process dumpProcess = new Process();
            ProcessStartInfo dumpProcessStartInfo = new ProcessStartInfo(fullPathToDcmdump, path.Text);
            dumpProcessStartInfo.UseShellExecute = false;
            dumpProcessStartInfo.RedirectStandardOutput = true;
            dumpProcessStartInfo.RedirectStandardError = true;
            dumpProcessStartInfo.CreateNoWindow = true;
            dumpProcess.StartInfo = dumpProcessStartInfo;
            dumpProcess.Start();

            StreamReader standardOutputReader = dumpProcess.StandardOutput;
            StreamReader errorOutputReader = dumpProcess.StandardError;
            var dump = standardOutputReader.ReadToEnd();
            dumpProcess.Close();

            DumpIntoFile(dump);
            OpenDumpInNotepad();
        }

        private void DumpIntoFile(String dump)
        {
            System.IO.File.WriteAllText("dump.txt", dump);
        }

        private static void OpenDumpInNotepad()
        {
            System.Diagnostics.Process notepadProcess = new System.Diagnostics.Process();
            notepadProcess.StartInfo = new ProcessStartInfo("notepad.exe", "dump.txt");
            notepadProcess.Start();
        }

        private void ButtonRenameTo(object sender, RoutedEventArgs e)
        {
            var tag = "0010,0010";
            var value = patientsName.Text;
            var filesPath = path.Text;

            Modify(tag, value, filesPath);
        }

        private void ButtonID(object sender, RoutedEventArgs e)
        {
            var tag = "0010,0020";
            var value = patientsID.Text;
            var filesPath = path.Text;

            Modify(tag, value, filesPath);
        }

        private void ButtonOtherTag(object sender, RoutedEventArgs e)
        {
            var tag = otherTag.Text;
            var value = otherValue.Text;
            var filesPath = path.Text;

            Modify(tag, value, filesPath);
        }

        private void Modify(string tag, string value, string filesPath)
        {
            System.Diagnostics.Process modificationProcess = new System.Diagnostics.Process();
            modificationProcess.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -ma \"({0})={1}\" {2}", tag, value, filesPath));
            modificationProcess.StartInfo.UseShellExecute = false;
            modificationProcess.StartInfo.RedirectStandardError = true;
            modificationProcess.StartInfo.RedirectStandardOutput = true;
            modificationProcess.StartInfo.CreateNoWindow = true;

            dbgblock.Text = string.Format("-nb -ma \"({0})={1}\" {2}", tag, value, filesPath);

            modificationProcess.Start();

            while (!modificationProcess.HasExited) { } // wait till process ends

            var errorlevel = modificationProcess.ExitCode.ToString();
            if (modificationProcess.HasExited && (errorlevel != "0"))
            {
                StreamReader standardOutputReader = modificationProcess.StandardOutput;
                StreamReader errorStreamReader = modificationProcess.StandardError;
                ShowErrorMessageBox(errorlevel, standardOutputReader, errorStreamReader);
            }
        }

        private static void ShowErrorMessageBox(string errorlevel, StreamReader standardOutputReader, StreamReader errorStreamReader)
        {
            System.Windows.MessageBox.Show(
                "An Error Occured\nErrorlevel: " + errorlevel + "\n" + standardOutputReader.ReadToEnd() + "\n" + errorStreamReader.ReadToEnd(),
                "Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error
            );
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
            var comboBox = sender as System.Windows.Controls.ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        private void ComboBoxDicomTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as System.Windows.Controls.ComboBox;

            string value = comboBox.SelectedItem as string;
        
            string[] stringSeparators = new string[] { "   " };
            string[] result;

            result = value.Split(stringSeparators, StringSplitOptions.None);
            var onlyNumberFromTagList = result[0];
            otherTag.Text = onlyNumberFromTagList;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.Title = "Select dicom file";

            var selectedPath = "";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
                string filename = selectedPath.ToString();
                path.Text = filename;
                SurroundPathWithQuotes();
            }
        }

        private void OnDragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.All;
            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            
            if (Directory.Exists(files[0]))
            {
                path.Text = System.IO.Path.Combine(files[0],"*");
            }
            else
            {
                path.Text = files[0];
            }
            SurroundPathWithQuotes();
            e.Handled = true;
        }

        private void SurroundPathWithQuotes()
        {
            path.Text = string.Format("\"" + path.Text + "\"");
        }
    }
}
