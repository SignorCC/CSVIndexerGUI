using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MeterReading
{
    public DateTime Timestamp { get; set; }
    public double MeterValue { get; set; }
    public int Missing { get; set; }
}

public static class CSVHelper
{
    public static void ProcessCsvFile(string filePath, bool removeDuplicates, bool removeNegatives)
    {
        try
        {
            // Read all lines from the CSV file
            var lines = File.ReadAllLines(filePath)
                            .Skip(1) // Skip header row
                            .Where(line => !string.IsNullOrWhiteSpace(line)) // Skip empty lines
                            .ToList();

            // Parse each line into a MeterReading object
            var meterReadings = new List<MeterReading>();
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length != 3)
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    continue;
                }

                if (!DateTime.TryParse(parts[0], out DateTime timestamp) ||
                    !double.TryParse(parts[1], out double meterValue) ||
                    !int.TryParse(parts[2], out int missing))
                {
                    Console.WriteLine($"Skipping line with parsing error: {line}");
                    continue;
                }

                meterReadings.Add(new MeterReading
                {
                    Timestamp = timestamp,
                    MeterValue = meterValue,
                    Missing = missing
                });
            }

            // Apply filters if needed
            if (removeNegatives)
                meterReadings = meterReadings.Where(r => r.MeterValue >= 0 && r.Missing >= 0).ToList();
            

            if (removeDuplicates)
                meterReadings = meterReadings.GroupBy(r => new { r.Timestamp, r.MeterValue, r.Missing })
                                             .Select(g => g.First())
                                             .ToList();
            

            // Write the processed data back to the file
            var outputLines = new List<string>
            {
                "timestamp,meter reading,missing" // Header
            };

            outputLines.AddRange(meterReadings.Select(r =>
                $"{r.Timestamp:yyyy-MM-dd HH:mm:sszzz},{r.MeterValue},{r.Missing}"
            ));

            File.WriteAllLines(filePath, outputLines);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing CSV file {filePath}: {ex.Message}");
        }
    }
}