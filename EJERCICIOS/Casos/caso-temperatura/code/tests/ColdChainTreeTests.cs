using ColdChainIndex.Models;
using DataQualityIndex;
using Xunit;

namespace ColdChainIndex.Tests;

public sealed class ColdChainTreeTests
{
    [Fact]
    public void InsertarDuplicado_NoAumentaLaCantidad()
    {
        BinarySearchTree<int, ColdChainRecord> tree = BuildTree();

        Assert.False(tree.Insert(new ColdChainRecord { RecordNumber = 4500 }));
        Assert.Equal(10, tree.Count);
    }

    [Fact]
    public void RecorridoPorNiveles_ConservaLaEstructura()
    {
        BinarySearchTree<int, ColdChainRecord> tree = BuildTree();
        int[] numbers = tree.Tree.LevelOrder().Select(node => node.Value.RecordNumber).ToArray();

        Assert.Equal(new[] { 4500, 2300, 6700, 1200, 3100, 5400, 7600, 800, 1500, 5800 }, numbers);
    }

    private static BinarySearchTree<int, ColdChainRecord> BuildTree()
    {
        int[] numbers = { 4500, 2300, 6700, 1200, 3100, 5400, 800, 5800, 7600, 1500 };
        BinarySearchTree<int, ColdChainRecord> tree = new(record => record.RecordNumber);

        foreach (int number in numbers)
        {
            tree.Insert(new ColdChainRecord { RecordNumber = number });
        }

        return tree;
    }
}
