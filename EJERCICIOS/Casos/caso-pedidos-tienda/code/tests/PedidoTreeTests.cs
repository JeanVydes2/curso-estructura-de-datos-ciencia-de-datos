using DataQualityIndex;
using PedidosTienda.Models;
using Xunit;

namespace PedidosTienda.Tests;

public sealed class PedidoTreeTests
{
    [Fact]
    public void InsertarDuplicado_NoAumentaLaCantidad()
    {
        BinarySearchTree<int, PedidoRecord> tree = BuildTree();

        Assert.False(tree.Insert(new PedidoRecord { OrderId = 4500 }));
        Assert.Equal(10, tree.Count);
    }

    [Fact]
    public void RecorridoPorNiveles_ConservaLaEstructura()
    {
        BinarySearchTree<int, PedidoRecord> tree = BuildTree();
        int[] numbers = tree.Tree.LevelOrder().Select(node => node.Value.OrderId).ToArray();

        Assert.Equal(new[] { 4500, 2300, 6700, 1200, 3100, 5400, 7600, 800, 1500, 5800 }, numbers);
    }

    private static BinarySearchTree<int, PedidoRecord> BuildTree()
    {
        int[] numbers = { 4500, 2300, 6700, 1200, 3100, 5400, 800, 5800, 7600, 1500 };
        BinarySearchTree<int, PedidoRecord> tree = new(record => record.OrderId);

        foreach (int number in numbers)
        {
            tree.Insert(new PedidoRecord { OrderId = number });
        }

        return tree;
    }
}
