using EtlScheduler;
using RotacionFefo.Models;
using Xunit;

namespace RotacionFefo.Tests;

public class MonticuloLoteTests
{
    /// <summary>
    /// Crea un montículo vacío con el comparador del caso.
    /// </summary>
    private static BinaryHeap<Lote> NuevoMonticulo()
    {
        return new BinaryHeap<Lote>(new LoteComparer());
    }

    /// <summary>
    /// Los siete lotes que están en la bodega al inicio de la jornada.
    /// </summary>
    private static List<Lote> LotesIniciales()
    {
        return new List<Lote>
        {
            new Lote("L01", "Leche entera 1 L", 9, 1),
            new Lote("L02", "Yogur natural 500 g", 4, 2),
            new Lote("L03", "Jugo de naranja 1 L", 12, 3),
            new Lote("L04", "Queso fresco 250 g", 3, 4),
            new Lote("L05", "Mantequilla 250 g", 4, 5),
            new Lote("L06", "Conserva de tomate 400 g", 20, 6),
            new Lote("L07", "Crema de leche 200 ml", 1, 7)
        };
    }

    /// <summary>
    /// Los siete movimientos de la jornada, en el mismo orden del dataset.
    /// </summary>
    private static List<Movimiento> MovimientosDeLaJornada()
    {
        return new List<Movimiento>
        {
            Movimiento.Retiro(1),
            Movimiento.Retiro(2),
            Movimiento.Llegada(3, new Lote("L08", "Harina de trigo 1 kg", 30, 8)),
            Movimiento.Llegada(4, new Lote("L09", "Pollo fresco 1 kg", 2, 9)),
            Movimiento.Retiro(5),
            Movimiento.Retiro(6),
            Movimiento.Retiro(7)
        };
    }

    /// <summary>
    /// Obtiene los códigos de una lista de lotes.
    /// </summary>
    private static List<string> Codigos(List<Lote> lotes)
    {
        return lotes.Select(lote => lote.Code).ToList();
    }

    /// <summary>
    /// Carga los siete lotes iniciales en el montículo.
    /// </summary>
    private static void CargarIniciales(BinaryHeap<Lote> monticulo)
    {
        foreach (Lote lote in LotesIniciales())
        {
            monticulo.Insert(lote);
        }
    }

    /// <summary>
    /// Procesa los primeros movimientos, comprueba el invariante después de cada uno y devuelve los retirados.
    /// </summary>
    private static List<Lote> Ejecutar(BinaryHeap<Lote> monticulo, int cantidadMovimientos)
    {
        List<Lote> retirados = new List<Lote>();
        List<Movimiento> movimientos = MovimientosDeLaJornada();
        for (int indice = 0; indice < cantidadMovimientos; indice++)
        {
            Movimiento movimiento = movimientos[indice];
            if (movimiento.EsLlegada)
            {
                monticulo.Insert(movimiento.Lot!);
            }
            else
            {
                retirados.Add(monticulo.ExtractRoot());
            }

            Assert.True(monticulo.IsValid());
        }

        return retirados;
    }

    /// <summary>
    /// Después de cargar los siete lotes iniciales, la lista y los intercambios coinciden con la traza.
    /// </summary>
    [Fact]
    public void CargaInicial_ConstruyeLaListaEsperada()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        CargarIniciales(monticulo);

        Assert.Equal(new List<string> { "L07", "L02", "L04", "L01", "L05", "L06", "L03" }, Codigos(monticulo.Snapshot()));
        Assert.Equal(2, monticulo.LastSwaps);
        Assert.Equal(2, monticulo.Height);
    }

    /// <summary>
    /// Con los mismos días que su padre, L05 no sube porque llegó después.
    /// </summary>
    [Fact]
    public void EmpateEnDias_NoIntercambiaLlegadaPosterior()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        foreach (Lote lote in LotesIniciales().Take(5))
        {
            monticulo.Insert(lote);
        }

        // L05 llega con 4 días, igual que su padre L02, pero llegó después: no sube.
        Assert.Equal(0, monticulo.LastSwaps);
        Assert.Equal("L05", monticulo.Snapshot()[4].Code);
    }

    /// <summary>
    /// Los cinco retiros de la jornada siguen los días restantes y el desempate por llegada.
    /// </summary>
    [Fact]
    public void Movimientos_RetiranEnOrdenDeVencimientoConDesempate()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        CargarIniciales(monticulo);
        List<Lote> retirados = Ejecutar(monticulo, MovimientosDeLaJornada().Count);

        Assert.Equal(new List<string> { "L07", "L04", "L09", "L02", "L05" }, Codigos(retirados));
        Assert.Equal(new List<string> { "L01", "L06", "L03", "L08" }, Codigos(monticulo.Snapshot()));
    }

    /// <summary>
    /// Al vaciar el montículo se obtiene el orden completo de retiro.
    /// </summary>
    [Fact]
    public void Vaciar_ProduceElOrdenCompleto()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        CargarIniciales(monticulo);
        List<Lote> retirados = Ejecutar(monticulo, MovimientosDeLaJornada().Count);
        while (!monticulo.IsEmpty)
        {
            retirados.Add(monticulo.ExtractRoot());
        }

        Assert.Equal(
            new List<string> { "L07", "L04", "L09", "L02", "L05", "L01", "L03", "L06", "L08" },
            Codigos(retirados)
        );
    }

    /// <summary>
    /// Consultar la raíz no modifica la lista del montículo.
    /// </summary>
    [Fact]
    public void ConsultarRaiz_NoCambiaElMonticulo()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        CargarIniciales(monticulo);
        List<string> antes = Codigos(monticulo.Snapshot());

        Assert.Equal("L07", monticulo.Peek().Code);
        Assert.Equal(antes, Codigos(monticulo.Snapshot()));
    }

    /// <summary>
    /// Consultar o retirar de un montículo vacío lanza una excepción.
    /// </summary>
    [Fact]
    public void MonticuloVacio_LanzaExcepcionAlConsultarYRetirar()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();

        Assert.Throws<InvalidOperationException>(() => monticulo.Peek());
        Assert.Throws<InvalidOperationException>(() => monticulo.ExtractRoot());
        Assert.Equal(-1, monticulo.Height);
    }

    /// <summary>
    /// Con un solo lote, retirarlo deja el montículo vacío.
    /// </summary>
    [Fact]
    public void LoteUnico_RetirarloDejaElMonticuloVacio()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        monticulo.Insert(new Lote("L99", "Prueba", 10, 1));

        Assert.Equal("L99", monticulo.ExtractRoot().Code);
        Assert.True(monticulo.IsEmpty);
    }

    /// <summary>
    /// El dibujo del montículo empieza con la raíz y tiene cinco líneas para tres niveles.
    /// </summary>
    [Fact]
    public void Impresor_DibujaLaRaizEnLaPrimeraLinea()
    {
        BinaryHeap<Lote> monticulo = NuevoMonticulo();
        CargarIniciales(monticulo);

        List<string> lineas = HeapPrinter.RenderTree(monticulo.Snapshot(), -1);

        Assert.Contains("[L07:1]", lineas[0]);
        Assert.Equal(5, lineas.Count);
    }
}
