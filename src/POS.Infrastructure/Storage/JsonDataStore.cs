using System.Text.Json;

namespace POS.Infrastructure.Storage;

public class JsonDataStore
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonDataStore()
    {
        _directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "POSData");

        Directory.CreateDirectory(_directory);
    }

    public List<T> Load<T>(string fileName)
    {
        var path = Path.Combine(_directory, fileName);

        if (!File.Exists(path))
            return new List<T>();

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<T>>(json, _options)
               ?? new List<T>();
    }

    public void Save<T>(string fileName, List<T> data)
    {
        var path = Path.Combine(_directory, fileName);
        var json = JsonSerializer.Serialize(data, _options);

        File.WriteAllText(path, json);
    }
}
