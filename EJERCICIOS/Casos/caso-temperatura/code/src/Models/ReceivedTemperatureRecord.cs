namespace ColdChainIndex.Models;

/// <summary>Representa una nueva lectura pendiente de clasificación.</summary>
public sealed class ReceivedTemperatureRecord
{
    public int RecordNumber { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string StorageCenter { get; set; } = string.Empty;
    public decimal TemperatureCelsius { get; set; }
    public string TransportStage { get; set; } = string.Empty;
}
