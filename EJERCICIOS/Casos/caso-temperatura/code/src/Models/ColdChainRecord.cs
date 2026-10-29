namespace ColdChainIndex.Models;

/// <summary>Representa una lectura histórica de temperatura farmacéutica.</summary>
public sealed class ColdChainRecord
{
    public int RecordNumber { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string StorageCenter { get; set; } = string.Empty;
    public decimal TemperatureCelsius { get; set; }
    public string TransportStage { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
}
