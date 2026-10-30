namespace PedidosTienda.Models;

/// <summary>Representa un pedido histórico de la tienda.</summary>
public sealed class PedidoRecord
{
    public int OrderId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string StoreCode { get; set; } = string.Empty;
    public int EstimatedDeliveryDays { get; set; }
    public int DeliveringDays { get; set; }
    public string Stage { get; set; } = string.Empty;
}
