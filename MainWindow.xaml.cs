using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSVIndexerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        
        // Use .exe directory as default
        public string TextBox_WorkingDirectory_Selection = AppDomain.CurrentDomain.BaseDirectory;
        public string TextBox_IndexingDirectory_Selection = AppDomain.CurrentDomain.BaseDirectory;
        public string TextBox_RootDirectory_Selection = AppDomain.CurrentDomain.BaseDirectory;

        // Prepare FileWorker
        public FileWorker fileWorker = new FileWorker();

        // ComboBox Values
        public string ComboBox_MethodSelection_Selection;
        public string ComboBox_PickName_Selection;
        public string ComboBox_PickContent_Selection;
        public string ComboBox_PickDescription_Selection;
        public string ComboBox_PickTags_Selection;
        public string ComboBox_WorkingMode_Selection;
        public string TextBox_Name_Selection;
        public string TextBox_Content_Selection;
        public string TextBox_Description_Selection;
        public string TextBox_Tags_Selection;

        // CheckBox Values
        public bool CheckBox_SearchMetaData_Selection = true;
        public bool Checkbox_RenameFiles_Selection = true;
        public bool CheckBox_RemoveNegatives_Selection = false;
        public bool CheckBox_RemoveDuplicates_Selection = false;
        public bool CheckBox_ForceRebuild_Selection = false;
        public bool CheckBox_CheckHash_Selection = true;  
        public bool abort = false;
        

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            // Set Culture (Displays everything in English, Dates in German)
            var culture = new System.Globalization.CultureInfo("en-DE");
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            // Set paths in TextBoxes
            TextBox_WorkingDirectory.Text = TextBox_WorkingDirectory_Selection;
            TextBox_IndexingDirectory.Text = TextBox_IndexingDirectory_Selection;
            TextBox_RootDirectory.Text = TextBox_RootDirectory_Selection;

            // Populate ComboBoxes and set global variables
            ComboBox_MethodSelection.Items.Add("MD5");
            ComboBox_MethodSelection.Items.Add("SHA256");
            ComboBox_MethodSelection_Selection = "MD5";


            ComboBox_PickName.Items.Add("Include");
            ComboBox_PickName.Items.Add("Exclude");
            ComboBox_PickName_Selection = "Include";

            ComboBox_PickContent.Items.Add("Include");
            ComboBox_PickContent.Items.Add("Exclude");
            ComboBox_PickContent_Selection = "Include";

            ComboBox_PickDescription.Items.Add("Include");
            ComboBox_PickDescription.Items.Add("Exclude");
            ComboBox_PickDescription_Selection = "Include";

            ComboBox_PickTags.Items.Add("Match Any");
            ComboBox_PickTags.Items.Add("Match All");
            ComboBox_PickTags.Items.Add("Match None");
            ComboBox_PickTags_Selection = "Match Any";

            ComboBox_WorkingMode.Items.Add("Directory");
            ComboBox_WorkingMode.Items.Add("Single File");
            ComboBox_WorkingMode_Selection = "Directory";


            // Set CheckBoxes
            CheckBox_SearchMetaData.IsChecked = CheckBox_SearchMetaData_Selection;
            CheckBox_RenameFiles.IsChecked = Checkbox_RenameFiles_Selection;
            CheckBox_RemoveNegatives.IsChecked = CheckBox_RemoveNegatives_Selection;
            CheckBox_RemoveDuplicates.IsChecked = CheckBox_RemoveDuplicates_Selection;
            CheckBox_ForceRebuild.IsChecked = CheckBox_ForceRebuild_Selection;
            CheckBox_CheckHash.IsChecked = CheckBox_CheckHash_Selection;

            // Read Allocation Table from Disk
            fileWorker.ReadTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));
        }

        private string OpenFileDialog(object sender, RoutedEventArgs e, string initialPath = "c:\\")
        {
            string filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();


            openFileDialog.InitialDirectory = initialPath;
            openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
                filePath = openFileDialog.FileName;
            

            return filePath;
            
        }

        private string OpenFolderDialog(object sender, RoutedEventArgs e, string initialPath = "c:\\")
        {
            string filePath = string.Empty;

            OpenFolderDialog openFolderDialog = new OpenFolderDialog();

            openFolderDialog.InitialDirectory = initialPath;
            openFolderDialog.Multiselect = false;
            openFolderDialog.ValidateNames = true;
            
            if(openFolderDialog.ShowDialog() == true)
                filePath = openFolderDialog.FolderName;
            

            return filePath;
        }

        private void ComboBox_PickName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_PickName_Selection = ComboBox_PickName.SelectedItem.ToString();
        }

        private void ComboBox_PickContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_PickContent_Selection = ComboBox_PickName.SelectedItem.ToString();
        }

        private void ComboBox_PickTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_PickTags_Selection = ComboBox_PickName.SelectedItem.ToString();
        }

        private void ComboBox_PickDescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_PickDescription_Selection = ComboBox_PickName.SelectedItem.ToString();
        }

        private void ComboBox_MethodSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_MethodSelection_Selection = ComboBox_MethodSelection.SelectedItem.ToString();
        }

        private void ComboBox_WorkingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_WorkingMode_Selection = ComboBox_WorkingMode.SelectedItem.ToString();

            if (ComboBox_WorkingMode_Selection == "Directory")
            {
                TextBlock_DirectoryToIndex.Text = "Directory to Index: ";
                Button_Index.Content = "Index Directory";
            }

            else
            {
                TextBlock_DirectoryToIndex.Text = "File to Index: ";
                Button_Index.Content = "Index File";
            }
        }

        private void CheckBox_RemoveNegatives_Checked(object sender, RoutedEventArgs e)
        {
            if(CheckBox_RemoveNegatives.IsChecked == true)
                CheckBox_RemoveNegatives_Selection = true;

            else
                CheckBox_RemoveNegatives_Selection = false;    
        }

        private void CheckBox_RenameFiles_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.IsChecked == true)
                Checkbox_RenameFiles_Selection = true;

            else
                Checkbox_RenameFiles_Selection = false;
        }

        private void CheckBox_RemoveDuplicates_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.IsChecked == true)
                CheckBox_RemoveDuplicates_Selection = true;

            else
                CheckBox_RemoveDuplicates_Selection = false;
        }

        private void CheckBox_CheckHash_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_CheckHash.IsChecked == true)
                CheckBox_CheckHash_Selection = true;

            else
                CheckBox_CheckHash_Selection = false;
        }

        private void CheckBox_ForceRebuild_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_ForceRebuild.IsChecked == true)
                CheckBox_ForceRebuild_Selection = true;

            else
                CheckBox_ForceRebuild_Selection = false;
        }

        private void CheckBox_SearchMetaData_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_SearchMetaData.IsChecked == true)
                CheckBox_SearchMetaData_Selection = true;

            else
                CheckBox_SearchMetaData_Selection = false;
        }

        private void TextBox_WorkingDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Error handling before parsing string
            if (!string.IsNullOrEmpty(TextBox_WorkingDirectory.Text))
                TextBox_WorkingDirectory_Selection = TextBox_WorkingDirectory.Text;
        }

        private void TextBox_IndexingDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newPath = String.Empty;

            if (!string.IsNullOrEmpty(TextBox_IndexingDirectory.Text))
            {   
                newPath = TextBox_IndexingDirectory.Text;
                TextBox_IndexingDirectory_Selection = newPath;
                TextBox_RootDirectory_Selection = newPath;
                TextBox_RootDirectory.Text = newPath;
                UpdateFields(newPath);
            }
        }

        private void TextBox_RootDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Error handling before parsing string
            if (!string.IsNullOrEmpty(TextBox_RootDirectory.Text))
                TextBox_RootDirectory_Selection = TextBox_RootDirectory.Text;
        }

        private void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(TextBox_Name.Text))
                TextBox_Name_Selection = TextBox_Name.Text;
        }

        private void TextBox_Content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(TextBox_Content.Text))
                TextBox_Content_Selection = TextBox_Content.Text;
        }

        private void TextBox_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(TextBox_Description.Text))
                TextBox_Description_Selection = TextBox_Description.Text;
        }

        private void TextBox_Tags_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(TextBox_Tags.Text))
                TextBox_Tags_Selection = TextBox_Tags.Text;
        }

        private void Button_SearchWorkingDirectory_Click(object sender, RoutedEventArgs e)
        {
            string newPath = OpenFolderDialog(sender, e, AppDomain.CurrentDomain.BaseDirectory);

            if (!String.IsNullOrEmpty(newPath))
            {
                TextBox_WorkingDirectory_Selection = newPath;
                TextBox_WorkingDirectory.Text = newPath;

                // Read Allocation Table from Disk
                if(Path.Exists(TextBox_WorkingDirectory_Selection))
                    fileWorker.ReadTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));
            }

        }

        private void Button_SearchIndexDirectory_Click(object sender, RoutedEventArgs e)
        {
            string newPath;

            if (ComboBox_WorkingMode_Selection == "Directory")
                newPath = OpenFolderDialog(sender, e, AppDomain.CurrentDomain.BaseDirectory);

            else
                newPath = OpenFileDialog(sender, e, AppDomain.CurrentDomain.BaseDirectory);

            if (!String.IsNullOrEmpty(newPath))
            {
                TextBox_IndexingDirectory_Selection = newPath;
                TextBox_IndexingDirectory.Text = newPath;
                TextBox_RootDirectory_Selection = newPath;
                TextBox_RootDirectory.Text = newPath;
                UpdateFields(newPath);
            }

        }

        private void UpdateFields(string newPath)
        {
            if (ComboBox_WorkingMode_Selection == "Single File" && File.Exists(newPath))
            {

                if (File.Exists(TextBox_IndexingDirectory_Selection))
                {
                    if (!File.Exists(TextBox_IndexingDirectory_Selection))
                    {
                        MessageBox.Show("File Path is invalid. Please select a valid file.", "Error");
                        return;
                    }

                    Dictionary<string, string> metadata = new Dictionary<string, string>();

                    string filename = Path.GetFileNameWithoutExtension(TextBox_IndexingDirectory_Selection);
                    string tags = filename.Replace(" ", "") + ";";
                    string startDate = Utility.GetConsistentDateString(File.GetCreationTime(TextBox_IndexingDirectory_Selection).ToString());
                    string endDate = Utility.GetConsistentDateString(File.GetLastWriteTime(TextBox_IndexingDirectory_Selection).ToString());

                    // generate metadata
                    TextBox_Name_Selection = filename;
                    TextBox_Content_Selection = "";
                    TextBox_Description_Selection = "";
                    TextBox_Tags_Selection = tags;
                    DatePicker_EndDate.SelectedDate = DateTime.Parse(endDate);
                    DatePicker_StartDate.SelectedDate = DateTime.Parse(startDate);

                    // Write Metadata to GUI
                    TextBox_Name.Text = TextBox_Name_Selection;
                    TextBox_Content.Text = TextBox_Content_Selection;
                    TextBox_Description.Text = TextBox_Description_Selection;
                    TextBox_Tags.Text = TextBox_Tags_Selection;

                }
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!Path.Exists(TextBox_WorkingDirectory_Selection))
            {
                MessageBox.Show("Working Directory Path is invalid. Please select a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() =>
                {
                    TextBlock_Progress.Text = "Aborted!";
                    ProgressBar.Value = 0;
                });
                return;
            }
            // Search based on Filters using LINQ
            List<HashFile> results = new List<HashFile>();
            string startDate = Utility.GetConsistentDateString(DatePicker_StartDate.SelectedDate.ToString());
            string endDate = Utility.GetConsistentDateString(DatePicker_EndDate.SelectedDate.ToString());
            // Read Allocation Table from Disk
            fileWorker.ReadTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));
            results = fileWorker.GetTable()
            .SelectMany(kvp => kvp.Value)
            .Where(file =>
                (string.IsNullOrEmpty(TextBox_Name_Selection) ||
                 (ComboBox_PickName_Selection == "Include" && file.metadata["Name"].Contains(TextBox_Name_Selection)) ||
                 (ComboBox_PickName_Selection == "Exclude" && !file.metadata["Name"].Contains(TextBox_Name_Selection))) &&
                (string.IsNullOrEmpty(TextBox_Content_Selection) ||
                 (ComboBox_PickContent_Selection == "Include" && file.metadata["Content"].Contains(TextBox_Content_Selection)) ||
                 (ComboBox_PickContent_Selection == "Exclude" && !file.metadata["Content"].Contains(TextBox_Content_Selection))) &&
                (string.IsNullOrEmpty(TextBox_Description_Selection) ||
                 (ComboBox_PickDescription_Selection == "Include" && file.metadata["Description"].Contains(TextBox_Description_Selection)) ||
                 (ComboBox_PickDescription_Selection == "Exclude" && !file.metadata["Description"].Contains(TextBox_Description_Selection))) &&
                (string.IsNullOrEmpty(TextBox_Tags_Selection) ||
                 (ComboBox_PickTags_Selection == "Match Any" && TextBox_Tags_Selection.Split(';', StringSplitOptions.RemoveEmptyEntries).Any(tag => file.metadata["Tags"].Contains(tag))) ||
                 (ComboBox_PickTags_Selection == "Match All" && TextBox_Tags_Selection.Split(';', StringSplitOptions.RemoveEmptyEntries).All(tag => file.metadata["Tags"].Contains(tag))) ||
                 (ComboBox_PickTags_Selection == "Match None" && !TextBox_Tags_Selection.Split(';', StringSplitOptions.RemoveEmptyEntries).Any(tag => file.metadata["Tags"].Contains(tag)))) &&
                (string.IsNullOrEmpty(startDate) || string.Compare(file.metadata["StartDate"], startDate) >= 0) &&
                (string.IsNullOrEmpty(endDate) || string.Compare(file.metadata["EndDate"], endDate) <= 0)
            )
            .ToList();

            // Create and show scrollable results window
            ShowScrollableResults(results);
        }

        private void ShowScrollableResults(List<HashFile> results)
        {
            var resultWindow = new Window
            {
                Title = "Search Results",
                Width = 600,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var scrollViewer = new ScrollViewer();
            var stackPanel = new StackPanel();

            foreach (var file in results)
            {
                var textBox = new TextBox
                {
                    Text = file.ToString(),
                    Margin = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap,
                    IsReadOnly = true,
                    VerticalAlignment = VerticalAlignment.Top,
                    BorderThickness = new Thickness(0)
                };
                stackPanel.Children.Add(textBox);
            }

            scrollViewer.Content = stackPanel;
            Grid.SetRow(scrollViewer, 0);
            mainGrid.Children.Add(scrollViewer);

            var saveButton = new Button
            {
                Content = "Save to ZIP",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            saveButton.Click += (s, e) => HashWorker.SaveResultsToZip(results, TextBox_WorkingDirectory_Selection);
            Grid.SetRow(saveButton, 1);
            mainGrid.Children.Add(saveButton);

            resultWindow.Content = mainGrid;
            resultWindow.Show();
        }

        private async void Button_Index_Click(object sender, RoutedEventArgs e)
        {
            if (!Path.Exists(TextBox_WorkingDirectory_Selection))
            {
                MessageBox.Show("Working Directory Path is invalid. Please select a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() =>
                {
                    TextBlock_Progress.Text = "Aborted!";
                    ProgressBar.Value = 0;
                });
                return;
            }

            if (Button_Index.Content.ToString() == "Abort")
            {
                abort = true;

                if(ComboBox_WorkingMode_Selection == "Directory")
                    Dispatcher.Invoke(() =>
                    {
                        Button_Index.Content = "Index Directory";
                        TextBlock_Progress.Text = "Aborted!";
                        ProgressBar.Value = 0;
                    });

                else
                    Dispatcher.Invoke(() =>
                    {
                        Button_Index.Content = "Index File";
                        TextBlock_Progress.Text = "Aborted!";
                        ProgressBar.Value = 0;
                    });

                return;
            }

            
            // Read Allocation Table from Disk (if it exists, else a new one is instantiated)
            if (Path.Exists(TextBox_WorkingDirectory_Selection))
                fileWorker.ReadTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));

            if (ComboBox_WorkingMode_Selection == "Directory")
            {
                Dispatcher.Invoke(() =>
                {
                    Button_Index.Content = "Abort";
                    ProgressBar.Value = 0;
                    TextBlock_Progress.Text = "Indexing Files..."; ;
                });
                await ProcessDirectoryAsync();
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    Button_Index.Content = "Abort";
                    ProgressBar.Value = 0;
                    TextBlock_Progress.Text = "Indexing File..."; ;
                });
                await ProcessSingleFileAsync();
            }
        }

        private async Task ProcessDirectoryAsync()
        {
            // Boilerplate Code for handling Dumb Users
            if (!Directory.Exists(TextBox_IndexingDirectory_Selection))
            {
                MessageBox.Show("Directory Path is invalid. Please select a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() =>
                {
                    TextBlock_Progress.Text = "Aborted!";
                    ProgressBar.Value = 0;
                    Button_Index.Content = "Index Directory";
                });
                return;
            }

            string[] files = Directory.GetFiles(TextBox_IndexingDirectory_Selection, "*.csv", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                MessageBox.Show("Directory Path is invalid. Please select a valid directory.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Dispatcher.Invoke(() =>
                {
                    TextBlock_Progress.Text = "Aborted!";
                    ProgressBar.Value = 0;
                    Button_Index.Content = "Index Directory";
                });
                return;
            }


            double progressIncrement = 100.0 / files.Length;

            await Task.Run(() =>
            {
                foreach (string file in files)
                {
                    if (abort)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TextBlock_Progress.Text = "Aborted!";
                            ProgressBar.Value = 0;
                            Button_Index.Content = "Index Directory";
                        });
                        abort = false;
                        return;
                    }

                    string json = String.Empty;
                    Dictionary<string, string> metadata = new Dictionary<string, string>();
                    Dictionary<string, string> indexedMetadata = new Dictionary<string, string>();

                    // Searching for existing metadata in the same directory of the file
                    if (CheckBox_SearchMetaData_Selection)
                    {
                        if (File.Exists(Path.ChangeExtension(file, ".json")))
                            json = File.ReadAllText(Path.ChangeExtension(file, ".json"));
                        else if (File.Exists(Path.Combine(Path.GetDirectoryName(file), "metadata.json")))
                            json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(file), "metadata.json"));

                        if (!String.IsNullOrEmpty(json))
                        {
                            try
                            {
                                json = json.Replace("[]", "\"[]\"");
                                indexedMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
                            }
                            catch (Exception ex)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show($"Error parsing JSON at {file}: " + ex.Message, "Error");
                                });
                            }
                        }
                    }

                    string filename = Path.GetFileNameWithoutExtension(file);
                    string hash = String.Empty;
                    string tags = String.Empty;
                    string startDate = String.Empty;
                    string endDate = String.Empty;


                    if (Checkbox_RenameFiles_Selection)
                    {
                        // filename shouldn't start with _
                        string placeholder = TextBox_RootDirectory_Selection;
                        if(!placeholder.EndsWith(Path.DirectorySeparatorChar))
                            placeholder += Path.DirectorySeparatorChar;

                        filename = Path.GetFullPath(file)
                        .Replace(placeholder, "")
                        .Replace(Path.DirectorySeparatorChar, '_')
                        .Replace(Path.VolumeSeparatorChar, '_')
                        .TrimStart('_');
                    }
                    
                    
                    // Calculating the Hashes
                    if (ComboBox_MethodSelection_Selection == "MD5")
                        hash = HashWorker.CalculateMD5Hash(file);
                    else if (ComboBox_MethodSelection_Selection == "SHA256")
                        hash = HashWorker.CalculateSHA256Hash(file);

                    if (fileWorker.ContainsFile(hash) && CheckBox_ForceRebuild_Selection)
                        fileWorker.RemoveFile(hash);

                    // Check if file already exists in Allocation Table (if desired)
                    if (fileWorker.ContainsFile(hash) && CheckBox_CheckHash_Selection)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBoxResult result = MessageBox.Show($"File: \"{filename}\" already exists in Allocation Table. Skipping.\n\nPress Abort to cancel Indexing Operation.", 
                                                                      "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.Cancel)
                                abort = true;

                            ProgressBar.Value += progressIncrement;
                        });
                        continue;
                    }

                    // Generate Metadata if none was found
                    if (indexedMetadata.Count == 0 || String.IsNullOrEmpty(json))
                    {
                        endDate = Utility.GetConsistentDateString(File.GetLastWriteTime(file).ToString());
                        startDate = Utility.GetConsistentDateString(File.GetCreationTime(file).ToString());

                        metadata.Add("Name", filename);
                        metadata.Add("Content", "");
                        metadata.Add("Description", "");
                        metadata.Add("Tags", "");
                        metadata.Add("StartDate", startDate);
                        metadata.Add("EndDate", endDate);
                        metadata.Add("Hash", hash);
                    }

                    // Boilerplate for parsing .json metadata
                    else
                    {
                        if (indexedMetadata.TryGetValue("start timestamp", out startDate))
                            startDate = Utility.GetConsistentDateString(startDate);
                        else
                            startDate = Utility.GetConsistentDateString(File.GetCreationTime(file).ToString());

                        if (indexedMetadata.TryGetValue("end timestamp", out endDate))
                            endDate = Utility.GetConsistentDateString(endDate);
                        else
                            endDate = Utility.GetConsistentDateString(File.GetLastWriteTime(file).ToString());

                        tags = $"{filename};{hash};";

                        foreach (var (key, value) in indexedMetadata)
                            if (!string.IsNullOrEmpty(value) && value.Count(char.IsLetter) >= 2)
                                tags += $"{value};";

                        indexedMetadata.TryGetValue("type", out string type);
                        indexedMetadata.TryGetValue("description", out string description);

                        metadata.Add("Name", filename);
                        metadata.Add("Content", type ?? "");
                        metadata.Add("Description", description ?? "");
                        metadata.Add("Tags", tags);
                        metadata.Add("StartDate", startDate);
                        metadata.Add("EndDate", endDate);
                        metadata.Add("Hash", hash);
                    }

                    string newFile = Path.Combine(TextBox_WorkingDirectory_Selection, filename);

                    // Check if file already exists in the working directory, generate filenames until it doesn't
                    if (File.Exists(newFile))
                    {
                        int i = 1;
                        while (File.Exists(newFile))
                        {
                            newFile = Path.Combine(TextBox_WorkingDirectory_Selection, $"{Path.GetFileNameWithoutExtension(filename)}_{i}.csv");
                            i++;
                        }
                    }

                    try
                    {
                        File.Copy(file, newFile, false);

                        // Perform simple checks on CSVs
                        if (CheckBox_RemoveNegatives_Selection || CheckBox_RemoveDuplicates_Selection)
                            CSVHelper.ProcessCsvFile(newFile, CheckBox_RemoveDuplicates_Selection, CheckBox_RemoveNegatives_Selection);
                    }

                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Error copying/processing file: " + ex.Message, "Error");
                        });
                    }

                    HashFile hashFile = new HashFile(newFile, hash, tags, metadata, filename, startDate, endDate);
                    fileWorker.AddFile(hashFile);

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value += progressIncrement;
                        TextBlock_Progress.Text = $"Processing: {Path.GetFileName(file)}";
                    });
                }

                // finally, save the allocation table to disk
                fileWorker.WriteTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 100;
                    TextBlock_Progress.Text = "Done!";
                    Button_Index.Content = "Index Directory";
                });
            });
        }

        private async Task ProcessSingleFileAsync()
        {
            if (!File.Exists(TextBox_IndexingDirectory_Selection))
            {
                MessageBox.Show("File Path is invalid. Please select a valid file.", "Error");
                Dispatcher.Invoke(() =>
                {
                    TextBlock_Progress.Text = "Aborted!";
                    ProgressBar.Value = 0;
                    Button_Index.Content = "Index File";
                });
                return;
            }

            ProgressBar.Value = 0;

            await Task.Run(() =>
            {
                string hash = String.Empty;
                string filepath = TextBox_IndexingDirectory_Selection;
                string filename = Path.GetFileName(filepath);

                if (Path.Exists(TextBox_WorkingDirectory_Selection))
                    fileWorker.ReadTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 25;
                    TextBlock_Progress.Text = "Hashing File...";
                });

                if (abort)
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBlock_Progress.Text = "Aborted!";
                        ProgressBar.Value = 0;
                        Button_Index.Content = "Index File";
                    });
                    abort = false;
                    return;
                }

                // Calculate Hash
                if (ComboBox_MethodSelection_Selection == "MD5")
                    hash = HashWorker.CalculateMD5Hash(filepath);
                else if (ComboBox_MethodSelection_Selection == "SHA256")
                    hash = HashWorker.CalculateSHA256Hash(filepath);

                if (fileWorker.ContainsFile(hash) && CheckBox_ForceRebuild_Selection)
                    fileWorker.RemoveFile(hash);

                if (fileWorker.ContainsFile(hash) && CheckBox_CheckHash_Selection)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"File {filename} already exists in Allocation Table. Aborting.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Button_Index.Content = "Index File";
                        ProgressBar.Value = 0;
                        TextBlock_Progress.Text = "Aborted!";
                    });
                    return;
                }

                // Generate Metadata (for single filemode, the user is prompted to enter metadata)
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                string endDate = Dispatcher.Invoke(() => Utility.GetConsistentDateString(DatePicker_EndDate.SelectedDate.ToString()));
                string startDate = Dispatcher.Invoke(() => Utility.GetConsistentDateString(DatePicker_StartDate.SelectedDate.ToString()));
                string tags = TextBox_Tags_Selection + $"{hash};";

                metadata.Add("Name", filename);
                metadata.Add("Content", TextBox_Content_Selection);
                metadata.Add("Description", TextBox_Description_Selection);
                metadata.Add("Tags", tags);
                metadata.Add("StartDate", startDate);
                metadata.Add("EndDate", endDate);
                metadata.Add("Hash", hash);

                if (abort)
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBlock_Progress.Text = "Aborted!";
                        ProgressBar.Value = 0;
                        Button_Index.Content = "Index File";
                    });
                    abort = false;
                    return;
                }

                string newFile = Path.Combine(TextBox_WorkingDirectory_Selection, filename);

                // Same as above, generate new filenames if file already exists
                if (File.Exists(newFile))
                {
                    int i = 1;
                    while (File.Exists(newFile))
                    {
                        newFile = Path.Combine(TextBox_WorkingDirectory_Selection, $"{Path.GetFileNameWithoutExtension(filename)}_{i}.csv");
                        i++;
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 50;
                });
                
                try
                {
                    File.Copy(filepath, newFile, false);

                    // Perform simple checks on CSV
                    if (CheckBox_RemoveNegatives_Selection || CheckBox_RemoveDuplicates_Selection)
                        CSVHelper.ProcessCsvFile(newFile, CheckBox_RemoveDuplicates_Selection, CheckBox_RemoveNegatives_Selection);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Error copying/processing file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }

                HashFile hashFile = new HashFile(newFile, hash, tags, metadata, filename, startDate, endDate);
                fileWorker.AddFile(hashFile);

                fileWorker.WriteTable(Path.Combine(TextBox_WorkingDirectory_Selection, "allocationTable.json"));

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 100;
                    TextBlock_Progress.Text = "Done!";
                    Button_Index.Content = "Index File";
                });
            });
        }

    }
}
