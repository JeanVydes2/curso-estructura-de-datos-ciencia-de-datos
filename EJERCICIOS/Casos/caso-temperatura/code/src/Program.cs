using ColdChainIndex.Models;
using DataQualityIndex.Services;
using DataQualityIndex;

namespace ColdChainIndex;

public static class Program
{
    public static async Task Main()
    {
        DataLocalService dataService = new();
        List<ColdChainRecord> history =
            await dataService.GetDataLocalAsync<ColdChainRecord>("cold-chain-history.json");
        List<ReceivedTemperatureRecord> received =
            await dataService.GetDataLocalAsync<ReceivedTemperatureRecord>("received-temperature-records.json");

        BinarySearchTree<int, ColdChainRecord> tree = new(record => record.RecordNumber);

        foreach (ColdChainRecord record in history)
        {
            // record.Condition = GetCondition(record.TemperatureCelsius);
            tree.Insert(record);
        }

        Console.WriteLine("Initial metrics:");

        PrintSummary(tree);

        PrintSearch(tree, 4500);
        PrintSearch(tree, 9999);

        InsertReceived(tree, received);

        Console.WriteLine("Metrics after inserting received records:");

        PrintSummary(tree);
    }

    private static void InsertReceived(
        BinarySearchTree<int, ColdChainRecord> tree,
        List<ReceivedTemperatureRecord> received)
    {
        Console.WriteLine("Received temperature records:");

        foreach (ReceivedTemperatureRecord receivedRecord in received)
        {
            ColdChainRecord record = new()
            {
                RecordNumber = receivedRecord.RecordNumber,
                ProductCode = receivedRecord.ProductCode,
                StorageCenter = receivedRecord.StorageCenter,
                TemperatureCelsius = receivedRecord.TemperatureCelsius,
                TransportStage = receivedRecord.TransportStage,
                Condition = GetCondition(receivedRecord.TemperatureCelsius)
            };

            bool inserted = tree.Insert(record);
            Console.WriteLine(
                $"{record.RecordNumber}: condition={record.Condition}, "
                + (inserted ? "inserted" : "rejected because of duplicate"));
        }
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

    private static void PrintSummary(BinarySearchTree<int, ColdChainRecord> tree)
    {
        Console.WriteLine($"Records in tree: {tree.Tree.CountNodes()}");
        Console.WriteLine($"height: {tree.Tree.Height()}");
        Console.WriteLine($"leaves: {tree.Tree.CountLeaves()}");
        Console.WriteLine($"internal nodes: {tree.Tree.CountInternal()}");
        Console.WriteLine($"Inorder: {string.Join(", ", tree.Tree.Inorder().Select(node => node.Value.RecordNumber))}");
        Console.WriteLine($"Preorder: {string.Join(", ", tree.Tree.Preorder().Select(node => node.Value.RecordNumber))}");
        Console.WriteLine($"Postorder: {string.Join(", ", tree.Tree.Postorder().Select(node => node.Value.RecordNumber))}");
        Console.WriteLine($"Level order: {string.Join(", ", tree.Tree.LevelOrder().Select(node => node.Value.RecordNumber))}");
        Console.WriteLine($"valid: {tree.IsValid()}");
    }

    private static void PrintSearch(BinarySearchTree<int, ColdChainRecord> tree, int recordNumber)
    {
        List<BinaryNode<ColdChainRecord>> path = tree.SearchPath(recordNumber);
        bool found = path.Count > 0 && path.Last().Value.RecordNumber == recordNumber;

        Console.WriteLine($"Search {recordNumber}: {(found ? "found" : "not found")}");
        Console.WriteLine($"Path {recordNumber}: {string.Join(" -> ", path.Select(node => node.Value.RecordNumber))}");
    }
}
