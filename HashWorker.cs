using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace CSVIndexerGUI
{
    public static class HashWorker
    {
        public static string CalculateMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder stringBuilder = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                        stringBuilder.Append(hashBytes[i].ToString("x2")); // Convert each byte to its hexadecimal representation
                    

                    return stringBuilder.ToString();
                }
            }
        }

        public static string CalculateSHA256Hash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    StringBuilder stringBuilder = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                        stringBuilder.Append(hashBytes[i].ToString("x2")); // Convert each byte to its hexadecimal representation
                    

                    return stringBuilder.ToString();
                }
            }
        }

        public static void SaveResultsToZip(List<HashFile> results, string initialDirectory)
        {
            if (!results.Any())
            {
                MessageBox.Show("No results to save.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Zip files (*.zip)|*.zip",
                DefaultExt = "zip",
                AddExtension = true,
                InitialDirectory = initialDirectory,
                OverwritePrompt = true  // This will prompt the user if they're about to overwrite an existing file
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            string zipPath = saveFileDialog.FileName;

            try
            {
                using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))  // Create mode to allow overwriting
                {
                    foreach (var file in results)
                    {
                        string filePath = file.filePath;
                        if (File.Exists(filePath))
                        {
                            // Use the original filename when adding to the zip
                            string fileName = Path.GetFileName(filePath);
                            zipArchive.CreateEntryFromFile(filePath, fileName);
                        }
                        else
                        {
                            // If the original file doesn't exist, create a text file with metadata
                            var entry = zipArchive.CreateEntry(file.metadata["Name"] + ".txt");
                            using (var writer = new StreamWriter(entry.Open()))
                            {
                                writer.WriteLine($"Name: {file.metadata["Name"]}");
                                writer.WriteLine($"Content: {file.metadata["Content"]}");
                                writer.WriteLine($"Description: {file.metadata["Description"]}");
                                writer.WriteLine($"Tags: {file.metadata["Tags"]}");
                                writer.WriteLine($"Start Date: {file.metadata["StartDate"]}");
                                writer.WriteLine($"End Date: {file.metadata["EndDate"]}");
                                writer.WriteLine($"Original path (file not found): {file.filePath}");
                            }
                        }
                    }
                }
                MessageBox.Show($"Results saved to {zipPath}", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving results: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
                
    }
}
