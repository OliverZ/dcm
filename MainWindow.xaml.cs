using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new ProcessStartInfo(@"dcmtk\dcmodify.exe", "-nb -m \"(0010,0010)=baz^bar\" dcmtk\\IM-0001-0010.dcm");

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();


            // instead of p.WaitForExit(), do
            StringBuilder q = new StringBuilder();
            while (!p.HasExited)
            {
                q.Append(p.StandardOutput.ReadToEnd());
            }
            string r = q.ToString();

        //   dcmodify.exe -nb -m "(0010,0010)=foo^bar" IM-0001-0010.dcm
        }

        private void ButtonRenameTo(object sender, RoutedEventArgs e)
        { 
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo = new ProcessStartInfo(@"dcmtk\dcmodify.exe", string.Format("-nb -m \"(0010,0010)={0}\" {1}", patientsName.Text, path.Text));
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            // instead of p.WaitForExit(), do
            StringBuilder q = new StringBuilder();
            while (!p.HasExited)
            {
                q.Append(p.StandardOutput.ReadToEnd());
            }
            string r = q.ToString();
        }

        private void ButtonCustomTag(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            var fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe"); 
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -m \"({0})={1}\" {2}", customTag.Text, customValue.Text, path.Text));
            //...
            p.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            var fullPathToDcmodify = System.IO.Path.Combine("dcmtk", "dcmodify.exe");
            p.StartInfo = new ProcessStartInfo(fullPathToDcmodify, string.Format("-nb -m \"(0010,0020)={0}\" {1}", patientsID.Text, path.Text));
            p.Start();
        }

        private void ButtonTagList(object sender, RoutedEventArgs e)
        {
            textfoo.Text = dcmtagcombobox.Text;
        }

 
    }
}
