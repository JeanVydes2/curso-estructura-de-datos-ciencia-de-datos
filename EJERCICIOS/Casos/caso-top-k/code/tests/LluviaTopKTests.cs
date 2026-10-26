using LluviaTopK;
using LluviaTopK.Modelos;
using Xunit;

namespace LluviaTopKTests;

public class LluviaTopKTests
{
    /// <summary>
    /// Obtiene los identificadores de una lista de acumulados.
    /// </summary>
    private static List<string> Ids(List<Acumulado> acumulados)
    {
        List<string> ids = new List<string>();
        foreach (Acumulado acumulado in acumulados)
        {
            ids.Add(acumulado.Id);
        }

        return ids;
    }

    /// <summary>
    /// El Top-3 del lote son los tres acumulados mayores, de mayor a menor.
    /// </summary>
    [Fact]
    public void TopK_DaLosTresMayoresEnOrdenDescendente()
    {
        List<Acumulado> top = SeleccionTopK.TopK(DatosDelCaso.Lote(), 3);

        Assert.Equal(new List<string> { "P09", "P05", "P12" }, Ids(top));
    }

    /// <summary>
    /// El Bottom-3 del lote son los tres acumulados menores, de menor a mayor.
    /// </summary>
    [Fact]
    public void BottomK_DaLosTresMenoresEnOrdenAscendente()
    {
        List<Acumulado> bottom = SeleccionTopK.BottomK(DatosDelCaso.Lote(), 3);

        Assert.Equal(new List<string> { "P02", "P08", "P06" }, Ids(bottom));
    }

    /// <summary>
    /// El Top-K y el Bottom-K coinciden con ordenar el lote completo.
    /// </summary>
    [Fact]
    public void TopKYBottomK_CoincidenConOrdenarElLote()
    {
        List<Acumulado> lote = DatosDelCaso.Lote();
        lote.Sort(new ComparadorPorMilimetros());
        List<Acumulado> ascendente = new List<Acumulado>(lote);

        List<Acumulado> top = SeleccionTopK.TopK(DatosDelCaso.Lote(), 5);
        List<Acumulado> bottom = SeleccionTopK.BottomK(DatosDelCaso.Lote(), 5);

        for (int indice = 0; indice < 5; indice++)
        {
            Assert.Equal(ascendente[ascendente.Count - 1 - indice].Id, top[indice].Id);
            Assert.Equal(ascendente[indice].Id, bottom[indice].Id);
        }
    }

    /// <summary>
    /// Con K mayor que el lote, se devuelven todos los elementos.
    /// </summary>
    [Fact]
    public void TopK_ConKMayorQueElLoteDevuelveTodo()
    {
        List<Acumulado> top = SeleccionTopK.TopK(DatosDelCaso.Lote(), 20);

        Assert.Equal(DatosDelCaso.Lote().Count, top.Count);
    }

    /// <summary>
    /// Con K en cero no se devuelve nada.
    /// </summary>
    [Fact]
    public void TopK_ConKCeroNoDevuelveNada()
    {
        Assert.Empty(SeleccionTopK.TopK(DatosDelCaso.Lote(), 0));
    }

    /// <summary>
    /// La construcción por Floyd mantiene el invariante y hace menos intercambios que insertar uno por uno.
    /// </summary>
    [Fact]
    public void ConstruccionPorFloyd_HaceMenosIntercambiosQueLaInsercion()
    {
        MonticuloBinario<Acumulado> porInsercion = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        int intercambiosInsercion = 0;
        foreach (Acumulado acumulado in DatosDelCaso.Lote())
        {
            porInsercion.Insertar(acumulado);
            intercambiosInsercion += porInsercion.UltimosIntercambios;
        }

        List<Acumulado> copia = new List<Acumulado>(DatosDelCaso.Lote());
        MonticuloBinario<Acumulado> porFloyd = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        int intercambiosFloyd = porFloyd.ConstruirDesde(copia);

        Assert.True(porFloyd.EsValido());
        Assert.True(intercambiosFloyd < intercambiosInsercion);
        Assert.Equal(porInsercion.VerRaiz().Id, porFloyd.VerRaiz().Id);
    }

    /// <summary>
    /// El montículo del recorrido conserva el invariante y su raíz es el menor de los K.
    /// </summary>
    [Fact]
    public void Monticulo_SuRaizEsElMenorDeLosK()
    {
        List<Acumulado> top = SeleccionTopK.TopK(DatosDelCaso.Lote(), 3);
        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros(), top);

        Assert.True(monticulo.EsValido());
        Assert.Equal("P12", monticulo.VerRaiz().Id);
        Assert.Equal(3, monticulo.Cantidad);
    }

    /// <summary>
    /// Sacar de un montículo vacío lanza una excepción.
    /// </summary>
    [Fact]
    public void MonticuloVacio_LanzaAlSacar()
    {
        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());

        Assert.Throws<InvalidOperationException>(() => monticulo.VerRaiz());
        Assert.Throws<InvalidOperationException>(() => monticulo.ExtraerRaiz());
        Assert.Equal(-1, monticulo.Altura);
    }

    /// <summary>
    /// Con un solo elemento, sacarlo deja el montículo vacío.
    /// </summary>
    [Fact]
    public void UnSoloElemento_SacarloDejaElMonticuloVacio()
    {
        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        monticulo.Insertar(new Acumulado("P99", 10.0));

        Assert.Equal("P99", monticulo.ExtraerRaiz().Id);
        Assert.True(monticulo.EstaVacio);
    }

    /// <summary>
    /// El dibujo de la lista con los 12 acumulados empieza con la raíz, que es el menor.
    /// </summary>
    [Fact]
    public void Impresor_DibujaLaRaizEnLaPrimeraLinea()
    {
        List<Acumulado> copia = new List<Acumulado>(DatosDelCaso.Lote());
        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros(), copia);

        List<string> lineas = ImpresorMonticulo.RenderizarArbol(monticulo.Copia(), -1);

        Assert.Contains("[P02:0,0]", lineas[0]);
    }
}
