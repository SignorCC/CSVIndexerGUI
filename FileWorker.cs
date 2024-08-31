using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace CSVIndexerGUI
{
    public class FileWorker
    {
        private Dictionary<string, List<HashFile>> AllocationTable;

        public FileWorker()
        {
            AllocationTable = new Dictionary<string, List<HashFile>>();
        }

        // Search for all files with the same hash in allocation Table
        public List<HashFile> SearchFiles(string hash)
        {
            if (AllocationTable.TryGetValue(hash, out List<HashFile> files))
                return files;

            else
                return new List<HashFile>();
        }

        // Add a file to the allocation table
        public void AddFile(HashFile file)
        {
            if (AllocationTable.TryGetValue(file.hash, out List<HashFile> files))             
                files.Add(file);

            else
                AllocationTable[file.hash] = new List<HashFile> { file };
        }

        public bool ContainsFile(string hash)
        {
            return AllocationTable.ContainsKey(hash);
        }

        // Remove a file from the allocation table
        public bool RemoveFile(HashFile file)
        {
            if (AllocationTable.TryGetValue(file.hash, out List<HashFile> files))
                if (files.Remove(file))
                {
                    if (files.Count == 0)                    
                        AllocationTable.Remove(file.hash);
                    
                    return true;
                }
            
            return false;
        }

        public bool RemoveFile(string hash)
        {
            if(AllocationTable.TryGetValue(hash, out List<HashFile> files))
            {
                AllocationTable.Remove(hash);
                return true;
            }

            return false;
        }

        // Read table in from disk
        public bool ReadTable(string path)
        {
            // Read in settings from json file
            path = Utility.ResolvePath(path);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                AllocationTable = JsonConvert.DeserializeObject<Dictionary<string, List<HashFile>>>(json);
                return true;
            }
            else
                return false;
        }

        // Write table to disk
        public bool WriteTable(string path)
        {
            path = Utility.ResolvePath(path);

            if (AllocationTable.Count <= 0)
                return false;

            else
            {
                string json = JsonConvert.SerializeObject(AllocationTable);
                File.WriteAllText(path, json);
                return true;
            }
        }

        public Dictionary<string, List<HashFile>> GetTable()
        {
            return AllocationTable;
        }
    }
}