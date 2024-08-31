using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CSVIndexerGUI
{
    public static class Utility
    {
        
        public static void Log(string message, string? comment = "", string severity = "Info")
        {
            string logFileName = DateTime.Now.ToString("dd.MM.yyyy") + ".txt";

            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "logs");

            // create directory if it doesn't exist
            Directory.CreateDirectory(logDirectory);

            // create log file if it doesn't exist
            string logFilePath = Path.Combine(logDirectory, logFileName);

            if (!File.Exists(logFilePath))
            {
                using (FileStream fs = File.Create(logFilePath))
                    fs.Close();
            }

            // construct log entry
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string entry = $"{timestamp} [{severity}]";

            if (!string.IsNullOrEmpty(comment))
                entry += $" [{comment}]";

            entry += $" {message}\n";

            // write log to console (logs in docker)
            Console.WriteLine(entry);

            // write log entry to file
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.Write(entry);
                sw.Close();
            }
        }

        public static string ResolvePath(string parsedPath)
        {
            // To resolve paths platform-independently, deconstruct path into parts, then resolve each part individually
            char separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/';
            string[] parts = parsedPath.Split(separator);
            var resolvedParts = new List<string>();


            foreach (string part in parts)
            {
                if (part == "" || part == ".")
                    continue;

                if (part == "..")
                {
                    if (resolvedParts.Count == 0)
                        throw new ArgumentException("Invalid parsed path");
                    resolvedParts.RemoveAt(resolvedParts.Count - 1);
                }

                else
                    resolvedParts.Add(part);
            }
            return string.Join(separator, resolvedParts);
        }

        public static string GetConsistentDateString(string timestampOrDateTime)
        {
            if (TryParseTimestamp(timestampOrDateTime, out DateTime dateTime))
                return dateTime.ToString("yyyy-MM-dd");

            if (DateTime.TryParse(timestampOrDateTime, out dateTime))
                return dateTime.ToString("yyyy-MM-dd");

            return timestampOrDateTime;
        }

        private static bool TryParseTimestamp(string timestamp, out DateTime dateTime)
        {
            string[] parts = timestamp.Split('T');
            if (parts.Length == 2 && DateTime.TryParse(parts[0], out dateTime))
                return true;

            dateTime = default;
            return false;
        }

    }

}
