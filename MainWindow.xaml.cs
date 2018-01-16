using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FeliratKezelo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> mediaList;
        List<string> subList;
        List<string> subPathList;
        List<string> mediaPathList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            DialogResult res = dialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                mediaList = new List<string>();
                subList = new List<string>();
                subPathList = new List<string>();
                mediaPathList = new List<string>();
                GetVideoAndSubFiles(dialog.SelectedPath);
                TextBox1.Text = dialog.SelectedPath;
            }
        }



        private void GetVideoAndSubFiles(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);


                foreach (var file in files)
                {
                    if (file.EndsWith("mkv") && !System.IO.Path.GetFileName(file).Contains("sample"))
                    {

                        string tempFileName = System.IO.Path.GetFileName(file);
                        tempFileName = tempFileName.Replace(".mkv", "");
                        mediaPathList.Add(file.ToLower());
                        mediaList.Add(tempFileName.ToLower());
                    }
                    if (file.EndsWith("srt"))
                    {
                        subPathList.Add(file.ToLower());
                        subList.Add(System.IO.Path.GetFileName(file.ToLower()));
                    }
                }
            }

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if(mediaList == null || subList == null)
            {
                System.Windows.MessageBox.Show("No directory selected!");
            }

            else if (mediaList.Count == 1 && subList.Count == 1)
            {

                string tempPath = subPathList[0];
                tempPath = tempPath.Replace(subList[0], mediaList[0] + ".srt");
                File.Move(subPathList[0], tempPath);
                System.Windows.MessageBox.Show("Done");
            }
            else if(mediaList.Count == 0)
            {
                System.Windows.MessageBox.Show("No mediaelement found!");
            }
            else if(subList.Count == 0)
            {
                System.Windows.MessageBox.Show("No subtitle file found!");
            }
            else if(mediaList.Count > 1 || subList.Count > 1)
            {
                Regex rxp = new Regex(@"^.*?s(?<season>\d{2})e(?<episode>\d{2}).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (string media in mediaList)
                {

                    Match matchMedia = rxp.Match(media);
                    if (rxp.IsMatch(media))
                    {
                        foreach (string sub in subList)
                        {
                            Match matchSub = rxp.Match(sub);

                            if (rxp.IsMatch(sub) && (matchMedia.Groups["season"].Value.Equals(matchSub.Groups["season"].Value)) && (matchMedia.Groups["episode"].Value.Equals(matchSub.Groups["episode"].Value)))
                            {
                                foreach (string sub1 in subPathList)
                                {
                                    if (sub1.Contains(sub))
                                    {
                                        string tempPath = sub1;
                                        tempPath = tempPath.Replace(sub, media + ".srt");
                                        File.Move(sub1, tempPath);
                                    }
                                }
                            }
                        }
                    }
                }
                System.Windows.MessageBox.Show("Done");
            } 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }   
}