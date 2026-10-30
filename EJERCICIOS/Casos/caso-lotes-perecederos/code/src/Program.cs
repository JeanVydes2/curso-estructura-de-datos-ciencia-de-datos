using System.Text;
using DataQualityIndex.Services;
using EtlScheduler;
using RotacionFefo.Models;

namespace RotacionFefo;

public static class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DataLocalService service = new();
        List<Lote> lotes = await service.GetDataLocalAsync<Lote>("lotes-en-bodega.json");
        List<Movimiento> movimientos = await service.GetDataLocalAsync<Movimiento>("movimientos.json");

        List<string> ordenConDesempate = Jornada(new LoteComparer(), lotes, movimientos);
        Console.WriteLine("con desempate: " + string.Join(", ", ordenConDesempate));
        Console.WriteLine("\n\n");
        // Inciso 10: misma jornada sin desempate 
        List<string> ordenSinDesempate = Jornada(new DiasComparer(), lotes, movimientos);
        Console.WriteLine("sin desempate: " + string.Join(", ", ordenSinDesempate));
    }

    private static List<string> Jornada(IComparer<Lote> comparer, List<Lote> lotes, List<Movimiento> movimientos)
    {
        BinaryHeap<Lote> monticulo = new BinaryHeap<Lote>(comparer);
        foreach (Lote lote in lotes)
        {
            monticulo.Insert(lote);
        }

        Console.WriteLine("Estado inicial:");
        MostrarEstado(monticulo);

        Lote proximo = monticulo.Peek();
        Console.WriteLine($"Raíz: {proximo.Code} (días restantes: {proximo.DaysToExpire})");
        Console.WriteLine($"Cantidad después de consultar la raíz: {monticulo.Count}");


        List<Lote> retirados = new List<Lote>();
        foreach (Movimiento movimiento in movimientos)
        {
            if (movimiento.EsLlegada)
            {
                monticulo.Insert(movimiento.Lot!);
                Console.WriteLine(
                    $"Movimiento {movimiento.Order}: llegada {movimiento.Lot!.Code} "
                    + $"(días restantes: {movimiento.Lot.DaysToExpire})"
                );

            }
            else if (monticulo.IsEmpty)
            {
                Console.WriteLine($"Movimiento {movimiento.Order}: retiro omitido porque el montículo está vacío");

            }
            else
            {
                Lote lote = monticulo.ExtractRoot();
                retirados.Add(lote);
                Console.WriteLine(
                    $"Movimiento {movimiento.Order}: retiro {lote.Code} (días restantes: {lote.DaysToExpire})"
                );

            }

            HeapPrinter.Print(monticulo.Snapshot(), -1);
        }

        Console.WriteLine("Estado final:");
        MostrarEstado(monticulo);

        List<Lote> restantes = new List<Lote>();
        while (!monticulo.IsEmpty)
        {
            restantes.Add(monticulo.ExtractRoot());
        }


        Console.WriteLine("Orden de retiro: " + string.Join(", ", retirados.Select(lote => lote.Code)));
        Console.WriteLine("Orden al vaciar el montículo: " + string.Join(", ", restantes.Select(lote => lote.Code)));

        return retirados.Concat(restantes).Select(lote => lote.Code).ToList();
    }

    private static void MostrarEstado(BinaryHeap<Lote> monticulo)
    {
        Console.WriteLine($"Lotes en el montículo: {monticulo.Count}");
        Console.WriteLine($"altura: {monticulo.Height}");
        Console.WriteLine($"intercambios de la última operación: {monticulo.LastSwaps}");
        Console.WriteLine($"cumple el invariante: {(monticulo.IsValid() ? "sí" : "no")}");
        Console.WriteLine();

        List<Lote> elementos = monticulo.Snapshot();
        Console.WriteLine("Elementos En el Montículo: " + string.Join("  ", elementos.Select((lote, indice) => $"[{indice}] {lote}")));
    }
}