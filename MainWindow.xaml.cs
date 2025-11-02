using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ZeroCup.DataHelper.Models;
using NPinyin;

namespace ZeroCup.DataHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScenicData data = new ScenicData();
        string folderPath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadDataFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            // 显示对话框并检查用户是否选择了文件
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 获取选择的文件路径
                    string filePath = openFileDialog.FileName;

                    // 读取文件内容
                    string jsonContent = File.ReadAllText(filePath);

                    var data = JsonConvert.DeserializeObject<ScenicData>(jsonContent) ?? new ScenicData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取文件时出错：{ex.Message}");
                }
            }
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // 获取所选文件夹的路径
                    folderPath = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"重命名过程中出错：{ex.Message}");
                }
            }
        }

        private async void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            // 在后台执行重命名操作
            await System.Threading.Tasks.Task.Run(() => RenameImages(folderPath));
        }

        private void RenameImages(string rootPath)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", "webp" };

            // 获取所有二级子文件夹
            var secondLevelFolders = Directory.GetDirectories(rootPath)
                .SelectMany(parent => Directory.GetDirectories(parent))
                .Where(folder => Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                    .Any(file => imageExtensions.Contains(Path.GetExtension(file).ToLower())));

            foreach (var folder in secondLevelFolders)
            {
                // 获取该文件夹中的所有图片文件
                var imageFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .OrderBy(file => file);

                int counter = 1;
                foreach (var file in imageFiles)
                {
                    string extension = Path.GetExtension(file);
                    string newFileName = Path.Combine(folder, $"{counter}{extension}");

                    // 如果新文件名已存在，跳过
                    if (!File.Exists(newFileName))
                    {
                        File.Move(file, newFileName);
                    }
                    counter++;
                }
            }

            MessageBox.Show("图片重命名完成！");
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("请先选择文件夹");
                return;
            }

            try
            {
                data = new ScenicData { Categories = new List<Category>() };
                var firstLevelFolders = Directory.GetDirectories(folderPath);

                foreach (var firstLevelFolder in firstLevelFolders)
                {
                    var category = new Category
                    {
                        Name = Path.GetFileName(firstLevelFolder),
                        Id = NPinyin.Pinyin.GetPinyin(Path.GetFileName(firstLevelFolder)).Replace(" ", ""),
                        Attractions = new List<ScenicSpot>()
                    };

                    var secondLevelFolders = Directory.GetDirectories(firstLevelFolder);
                    foreach (var secondLevelFolder in secondLevelFolders)
                    {
                        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
                        category.Attractions.Add(new ScenicSpot
                        {
                            Name = Path.GetFileName(secondLevelFolder),
                            Id = NPinyin.Pinyin.GetPinyin(Path.GetFileName(secondLevelFolder)).Replace(" ", ""),
                            Details = new SpotDetails
                            {
                                Summary = File.ReadAllText(Directory.GetFiles(secondLevelFolder, "*.txt").FirstOrDefault() ?? ""),
                                Images = Directory.GetFiles(secondLevelFolder)
                                    .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                                    .Select(f => Path.GetFileName(f))
                                    .ToList()
                            }
                        });
                    }

                    data.Categories.Add(category);
                }

                MessageBox.Show("数据加载完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据时出错：{ex.Message}");
            }
        }

        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("请先选择文件夹");
                return;
            }

            if (data?.Categories == null || data.Categories.Count == 0)
            {
                MessageBox.Show("没有数据可保存");
                return;
            }

            try
            {
                string jsonPath = Path.Combine(folderPath, "scenic_data.json");
                string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(jsonPath, jsonContent);
                MessageBox.Show("数据保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存数据时出错：{ex.Message}");
            }
        }

    }
}