using ColdChainIndex.Models;
using DataQualityIndex.Services;
using DataQualityIndex;

namespace ColdChainIndex;

public static class Program
{
    public static async Task Main()
    {
        DataLocalService service = new();

        List<ColdChainRecord> historial = await service.GetDataLocalAsync<ColdChainRecord>("cold-chain-history.json");
        List<ReceivedTemperatureRecord> recibidos = await service.GetDataLocalAsync<ReceivedTemperatureRecord>("received-temperature-records.json");

        BinarySearchTree<int, ColdChainRecord> tree = new(record => record.RecordNumber);

        foreach (ColdChainRecord registro in historial)
        {
            tree.Insert(registro);
        }

        ImprimirResumen(tree);

        ImprimirBusqueda(tree, 4500);
        ImprimirBusqueda(tree, 9999);


        InsertReceived(tree, recibidos);

        ImprimirResumen(tree);
    }

    private static void ImprimirResumen(BinarySearchTree<int, ColdChainRecord> tree) {
        Console.WriteLine($"Registros {tree.Tree.CountNodes()}");
        Console.WriteLine($"Altura {tree.Tree.Height()}");
        Console.WriteLine($"Hojas {tree.Tree.CountLeaves()}");
        Console.WriteLine($"Nodo Internos {tree.Tree.CountInternal()}");
        Console.WriteLine($"Inorden {string.Join(", ", tree.Tree.Inorder().Select(nodo => nodo.Value.RecordNumber))}");
        Console.WriteLine($"Preorden {string.Join(", ", tree.Tree.Preorder().Select(nodo => nodo.Value.RecordNumber))}");
        Console.WriteLine($"Postorden {string.Join(", ", tree.Tree.Postorder().Select(nodo => nodo.Value.RecordNumber))}");
        Console.WriteLine($"Level order {string.Join(", ", tree.Tree.LevelOrder().Select(nodo => nodo.Value.RecordNumber))}");
        Console.WriteLine($"Valido {tree.IsValid()}");
    }

    private static void ImprimirBusqueda(BinarySearchTree<int, ColdChainRecord> tree, int registro)
    {
        List<BinaryNode<ColdChainRecord>> ruta = tree.SearchPath(registro);
        bool encontrado = ruta.Count > 0 && ruta.Last().Value.RecordNumber == registro;

        Console.WriteLine($"Busqueda {registro}: {(encontrado ? "Encontrado" : "No encontrado")}");
        Console.WriteLine($"Ruta: {string.Join(", ", ruta.Select(node => node.Value.RecordNumber))}");

    }

    private static string GetCondition(decimal temperature)
    {
        if (temperature < 0 || temperature > 8)
        {
            return "Critical";
        }
        
        if (temperature < 2 || temperature > 6)
        {
            return "Review";
        }

        return "Normal";
    }


    private static void InsertReceived(
        BinarySearchTree<int, ColdChainRecord> arbol,
        List<ReceivedTemperatureRecord> recibidos
    )
    {
        foreach (ReceivedTemperatureRecord receivedRecord in recibidos)
        {
            ColdChainRecord record = new()
            {
                RecordNumber = receivedRecord.RecordNumber,
                ProductCode = receivedRecord.ProductCode,
                StorageCenter = receivedRecord.StorageCenter,
                TemperatureCelsius = receivedRecord.TemperatureCelsius,
                Condition = GetCondition(receivedRecord.TemperatureCelsius)
            };

            bool inserted = arbol.Insert(record);
            Console.WriteLine(
                $"{record.RecordNumber}: condition={record.Condition}, "
                + (inserted ? "inserted" : "rechazo por duplicado")
            );
        }
    }
}
