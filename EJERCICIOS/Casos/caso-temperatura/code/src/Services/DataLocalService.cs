using System.Text.Json;

namespace DataQualityIndex.Services;

/// <summary>Lee listas de entidades desde archivos JSON locales.</summary>
public sealed class DataLocalService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>Deserializa un archivo JSON como una lista del tipo solicitado.</summary>
    public async Task<List<T>> GetDataLocalAsync<T>(string fileName)
    {
        string datasetPath = Path.Combine(AppContext.BaseDirectory, "dataset");
        string path = Path.Combine(datasetPath, fileName);
        string json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<List<T>>(json, SerializerOptions) ?? [];
    }
}


