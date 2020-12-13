using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows.Controls;

namespace ImageCompresser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int ivalue = 0;
        public MainWindow()
        {
            
            InitializeComponent();
            ListviewData.AllowDrop = true;
            ProcessingPanel.Visibility = Visibility.Collapsed;

        }
        public async Task<List<FileInfo>> GetImageFilesFromFolder(String folder)
        {
           return await  Task.Run(() =>
            {
                List<FileInfo> Files = new List<FileInfo>();
                DirectoryInfo info = new DirectoryInfo(folder);
                foreach (var file in info.EnumerateFiles())
                {
                    switch (file.Extension.ToLower())
                    {
                        case ".jpg": Files.Add(file); break;
                        case ".jpeg": Files.Add(file); break;
                        case ".png": Files.Add(file); break;
                    }
                    ClearRam();

                }
               
                return Files;

            });
        }
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                ProcessingPanel.Visibility = Visibility.Visible;
                ProcessStatusLabel.Content = "Добавление изображений...";

                var filesAwaiter= GetImageFilesFromFolder(folderBrowserDialog.SelectedPath).GetAwaiter();
                filesAwaiter.OnCompleted(() =>
                {
                    foreach (var k in filesAwaiter.GetResult())
                    {
                        ListviewData.Items.Add(new DataViewModel()
                        {
                            FilePath = k.FullName,
                            FileSize=k.Length,
                            Title=k.Name,
                            

                        }) ;
                    }

                    ProcessingPanel.Visibility = Visibility.Collapsed;
                });
            }
        }
        public void Compress(String FilePath,int CompressValue)
        {
            using (Bitmap bmp = new Bitmap(FilePath))
            {
                bmp.Save(
                    FilePath.Substring(0, FilePath.LastIndexOf("\\")) + "\\compressed_" + FilePath.Split('\\')[FilePath.Split('\\').Length - 1],
                    ImageCodecInfo.GetImageEncoders()[1],
                    new EncoderParameters()
                    {
                        Param = new EncoderParameter[] {
                            new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L - CompressValue*10)
                        }
                        
                    });
               
            }
            
                

            ClearRam();
        }

        private async void StartCompressButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessingPanel.Visibility = Visibility.Visible;
            int countzipped = 0;
            int totalcountzipped=ListviewData.Items.Count;
          
            ProcessStatusLabel.Content = "Сжатие изображений\nИзображений сжато:"+countzipped.ToString()+" из "+ totalcountzipped.ToString();
            _ = Task.Run(async () =>
            {

                object[] list = null;
                this.Dispatcher.Invoke(() =>
                {

                    list = new object[ListviewData.Items.Count];
                    for (int i = 0; i < ListviewData.Items.Count; i++)
                        list[i] = ListviewData.Items[i];
                    
                });

                foreach (var data in list)
                {
                    Compress(((DataViewModel)data).FilePath, ivalue);
                    ProcessStatusLabel.Dispatcher.Invoke(() =>
                    {
                        countzipped = countzipped + 1;
                        ProcessStatusLabel.Content = "Сжатие изображений\n Изображений сжато:" + countzipped.ToString() + " из " + totalcountzipped.ToString();
                    });
                }
                ProcessingPanel.Dispatcher.Invoke(() =>
                {
                    ProcessingPanel.Visibility = Visibility.Collapsed;
                    ListviewData.Items.Clear();
                });

            });
        }

         
        private void ClearRam()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.Collect();
        }

        private void ListviewData_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void ListviewData_Drop(object sender, DragEventArgs e)
        {
            ProcessingPanel.Visibility = Visibility.Visible;
            ProcessStatusLabel.Content = "Добавление изображений...";
            string[] filesPath = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in filesPath)
            {
                if (File.GetAttributes(file) == FileAttributes.Directory)
                {
                    var filesAwaiter = GetImageFilesFromFolder(file).GetAwaiter();
                    filesAwaiter.OnCompleted(() =>
                    {
                        foreach (var k in filesAwaiter.GetResult())
                        {
                            ListviewData.Items.Add(new DataViewModel()
                            {
                                FilePath = k.FullName,
                                FileSize = k.Length,
                                Title = k.Name,
                            });
                        }
                    });
                }
                else
                {

                    FileInfo Finfo = new FileInfo(file);
                    string LFilename = Finfo.Name.ToLower();
                    if (LFilename.EndsWith(".jpg") ||
                        LFilename.EndsWith(".jpeg") ||
                        LFilename.EndsWith(".png"))
                    {
                        ListviewData.Items.Add(new DataViewModel()
                        {
                            FilePath = Finfo.FullName,
                            FileSize = Finfo.Length,
                            Title = Finfo.Name,


                        });
                    }

                }
            }
            ClearRam();
            ProcessingPanel.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if(ListviewData.Items.Count>0)
                if(ListviewData.SelectedItems.Count>0)
                    ListviewData.Items.RemoveAt(ListviewData.SelectedIndex);
                
            
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            ListviewData.Items.Clear();
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Файлы изображений (*.jpeg, *.jpg, *.png) | *.jpeg; *.jpg; *.png";
            if (openFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                FileInfo Finfo = new FileInfo(openFileDialog.FileName);
                string LFilename = Finfo.Name.ToLower();
                if (LFilename.EndsWith(".jpg") ||
                    LFilename.EndsWith(".jpeg") ||
                    LFilename.EndsWith(".png"))
                {
                    ListviewData.Items.Add(new DataViewModel()
                    {
                        FilePath = Finfo.FullName,
                        FileSize = Finfo.Length,
                        Title = Finfo.Name,
                    });
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
           var s = sender as RadioButton;
           ivalue=int.Parse(s.Content.ToString().Trim());
            foreach(DataViewModel item in  ListviewData.Items)
                item.CompressedFileSize =item.FileSize/ivalue;
            
        }
    }
}
