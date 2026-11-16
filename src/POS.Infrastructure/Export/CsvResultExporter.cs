using System.Globalization;
using System.Text;
using POS.Application.DTOs;
using POS.Application.Ports;

namespace POS.Infrastructure.Export;

public class CsvResultExporter : IResultExporter
{
    public string Export(IReadOnlyCollection<StudentResultDto> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "POSData",
            "exports");

        Directory.CreateDirectory(directory);

        var fileName =
            $"final-results-{DateTime.Now:yyyyMMdd-HHmmss}.csv";

        var path = Path.Combine(directory, fileName);

        var lines = new List<string>
        {
            "Student ID,Student Name,Final Result"
        };

        foreach (var result in results)
        {
            var score = result.FinalScore?.ToString(
                "F2", CultureInfo.InvariantCulture) ?? "";

            lines.Add(
                $"{Escape(result.StudentId.ToString())}," +
                $"{Escape(result.StudentName)}," +
                $"{Escape(score)}");
        }

        File.WriteAllText(
            path,
            string.Join(Environment.NewLine, lines),
            new UTF8Encoding(true));

        return path;
    }

    private static string Escape(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
