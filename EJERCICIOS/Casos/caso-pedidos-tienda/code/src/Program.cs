using PedidosTienda.Models;
using DataQualityIndex.Services;
using DataQualityIndex;

namespace PedidosTienda;

public static class Program
{
    public static async Task Main()
    {
        DataLocalService service = new();

        List<PedidoRecord> historial = await service.GetDataLocalAsync<PedidoRecord>("order-history.json");
        List<PedidoRecibido> recibidos = await service.GetDataLocalAsync<PedidoRecibido>("received-orders.json");

        BinarySearchTree<int, PedidoRecord> tree = new(pedido => pedido.OrderId);

        foreach (PedidoRecord pedido in historial)
        {
            tree.Insert(pedido);
        }

        Console.WriteLine("Initial metrics:");
        ImprimirResumen(tree);

        // Índice aparte de los pedidos nuevos, con ProductCode como clave, para buscarlos por producto.
        BinarySearchTree<string, PedidoRecibido> porProducto = new(pedido => pedido.ProductCode);

        foreach (PedidoRecibido pedido in recibidos)
        {
            porProducto.Insert(pedido);
        }

        ImprimirBusquedaPorProducto(porProducto, "HOGAR-M");
        ImprimirBusquedaPorProducto(porProducto, "ELEC-Z");

        Console.WriteLine("Received orders:");
        InsertarRecibidos(tree, recibidos);

        Console.WriteLine("Metrics after inserting received orders:");
        ImprimirResumen(tree);

        MarcarAtrasados(tree);
    }

    private static void ImprimirResumen(BinarySearchTree<int, PedidoRecord> tree)
    {
        Console.WriteLine($"Records in tree: {tree.Tree.CountNodes()}");
        Console.WriteLine($"height: {tree.Tree.Height()}");
        Console.WriteLine($"leaves: {tree.Tree.CountLeaves()}");
        Console.WriteLine($"internal nodes: {tree.Tree.CountInternal()}");
        Console.WriteLine($"Inorder: {string.Join(", ", tree.Tree.Inorder().Select(nodo => nodo.Value.OrderId))}");
        Console.WriteLine($"Preorder: {string.Join(", ", tree.Tree.Preorder().Select(nodo => nodo.Value.OrderId))}");
        Console.WriteLine($"Postorder: {string.Join(", ", tree.Tree.Postorder().Select(nodo => nodo.Value.OrderId))}");
        Console.WriteLine($"Level order: {string.Join(", ", tree.Tree.LevelOrder().Select(nodo => nodo.Value.OrderId))}");
        Console.WriteLine($"valid: {tree.IsValid()}");
    }

    private static void ImprimirBusquedaPorProducto(BinarySearchTree<string, PedidoRecibido> indice, string productCode)
    {
        List<BinaryNode<PedidoRecibido>> ruta = indice.SearchPath(productCode);
        bool encontrado = ruta.Count > 0 && ruta.Last().Value.ProductCode == productCode;

        Console.WriteLine($"Search {productCode}: {(encontrado ? "found" : "not found")}");
        Console.WriteLine($"Path {productCode}: {string.Join(", ", ruta.Select(nodo => nodo.Value.ProductCode))}");
    }

    /// <summary>Delayed cuando los días de entrega reales superan a los estimados.</summary>
    private static string GetStage(int estimatedDeliveryDays, int deliveringDays, string informedStage)
    {
        return deliveringDays > estimatedDeliveryDays ? "Delayed" : informedStage;
    }

    /// <summary>
    /// Los pedidos nuevos NO vienen con días de entrega en el dataset, así que se completan aquí:
    /// - DeliveringDays se fija en 0: todavía no se ha entregado nada.
    /// - EstimatedDeliveryDays se calcula con un contador que suma 1 día por cada pedido de la cola:
    ///   el primero en insertar espera 1 día, el segundo 2, el tercero 3, el cuarto 4 (incremento acumulado).
    /// Con DeliveringDays = 0 y EstimatedDeliveryDays >= 1 la regla nunca marca Delayed: el pedido
    /// conserva la etapa informada (Processing o Shipping).
    /// </summary>
    private static void InsertarRecibidos(
        BinarySearchTree<int, PedidoRecord> arbol,
        List<PedidoRecibido> recibidos
    )
    {
        int contadorEspera = 0;

        foreach (PedidoRecibido recibido in recibidos)
        {
            int estimatedDeliveryDays = ++contadorEspera;

            PedidoRecord pedido = new()
            {
                OrderId = recibido.OrderId,
                ProductCode = recibido.ProductCode,
                StoreCode = recibido.StoreCode,
                EstimatedDeliveryDays = estimatedDeliveryDays,
                DeliveringDays = 0,
                Stage = GetStage(estimatedDeliveryDays, 0, recibido.Stage)
            };

            bool inserted = arbol.Insert(pedido);
            Console.WriteLine(
                $"{pedido.OrderId}: stage={pedido.Stage}, "
                + (inserted ? "inserted" : "rejected because of duplicate")
            );
        }
    }

    /// <summary>Aplica la regla a todo el árbol accediendo a cada nodo, sin reconstruirlo.</summary>
    private static void MarcarAtrasados(BinarySearchTree<int, PedidoRecord> tree)
    {
        Console.WriteLine("Delayed orders (DeliveringDays > EstimatedDeliveryDays):");

        foreach (BinaryNode<PedidoRecord> nodo in tree.Tree.Inorder())
        {
            if (nodo.Value.DeliveringDays > nodo.Value.EstimatedDeliveryDays)
            {
                nodo.Value.Stage = "Delayed";
                Console.WriteLine($"{nodo.Value.OrderId}: stage={nodo.Value.Stage}");
            }
        }

        Console.WriteLine($"valid: {tree.IsValid()}");
    }
}
