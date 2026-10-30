namespace PedidosTienda.Models;

/// <summary>Representa un nuevo pedido pendiente de clasificación.</summary>
public sealed class PedidoRecibido
{
    public int OrderId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string StoreCode { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
}
