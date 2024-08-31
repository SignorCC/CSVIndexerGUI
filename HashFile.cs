using System;
using System.Collections.Generic;


namespace CSVIndexerGUI
{
    // Internal class for handling indexed files
    [Serializable]
    public class HashFile
    {
        public string filePath { get; set; }

        public string fileName { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string hash { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public string tags { get; set; }

        public HashFile(string filePath, string hash, string tags, Dictionary<string, string> metadata, string fileName, string startTime, string endTime)
        {
            this.filePath = filePath;
            this.hash = hash;
            this.tags = tags;
            this.metadata = metadata;
            this.fileName = fileName;
            this.startTime = startTime;
            this.endTime = endTime;
        }


        // Override ToString method and Equals method to enable comparisons
        public override string ToString()
        {
            return $"File: {fileName}, Path: {filePath}, Hash: {hash}, Tags: {tags}";
        }

        public override bool Equals(object obj)
        {
            if (obj is HashFile)
            {
                var file = obj as HashFile;
                return file.filePath == this.filePath && file.hash == this.hash;
            }
            return false;
        }
    }
}
