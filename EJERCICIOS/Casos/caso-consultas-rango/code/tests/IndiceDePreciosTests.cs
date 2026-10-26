using IndiceDePrecios;
using Xunit;

namespace IndiceDePreciosTests;

public class IndiceDePreciosTests
{
    /// <summary>
    /// Inserta los precios del caso en el orden en que llegaron los lotes.
    /// </summary>
    private static ArbolDePrecios ConstruirIndice()
    {
        ArbolDePrecios indice = new ArbolDePrecios();
        foreach (Producto producto in DatosDelCaso.Precios())
        {
            indice.Insertar(producto.Precio, producto.Nombre);
        }

        return indice;
    }

    /// <summary>
    /// Obtiene los precios de una lista de nodos.
    /// </summary>
    private static List<int> Precios(List<NodoPrecio> nodos)
    {
        List<int> precios = new List<int>();
        foreach (NodoPrecio nodo in nodos)
        {
            precios.Add(nodo.Precio);
        }

        return precios;
    }

    /// <summary>
    /// El índice del caso tiene 12 precios, raíz 35000 y altura 4.
    /// </summary>
    [Fact]
    public void Construir_ElOrdenDeLlegadaDaRaiz35000YAlturaCuatro()
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.Equal(12, indice.Contar());
        Assert.Equal(4, indice.Altura());
        Assert.Equal(35000, indice.Raiz?.Precio);
        Assert.True(indice.EsValido());
    }

    /// <summary>
    /// El tamaño de la raíz es el número de precios y los tamaños cumplen el invariante.
    /// </summary>
    [Fact]
    public void Tamano_DeLaRaizEsElTotalYSeMantieneAlInsertar()
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.Equal(12, indice.Raiz?.Tamano);
        Assert.Equal(6, indice.Raiz?.Izquierdo?.Tamano);

        indice.Insertar(30000, "Guayaba");
        Assert.Equal(13, indice.Raiz?.Tamano);
        Assert.True(indice.EsValido());
    }

    /// <summary>
    /// El inorden lista los precios crecientes.
    /// </summary>
    [Fact]
    public void EnOrden_ListaLosPreciosCrecientes()
    {
        List<int> precios = Precios(ConstruirIndice().EnOrden());

        Assert.Equal(new List<int> { 3000, 8000, 12000, 19000, 23000, 27000, 35000, 41000, 48000, 50000, 55000, 62000 }, precios);
    }

    /// <summary>
    /// El piso y el techo de cada consulta del día.
    /// </summary>
    [Theory]
    [InlineData(30000, 27000, 35000)]
    [InlineData(3000, 3000, 3000)]
    [InlineData(62000, 62000, 62000)]
    [InlineData(50000, 50000, 50000)]
    [InlineData(12000, 12000, 12000)]
    public void PisoYTecho_DanElResultadoEsperado(int valor, int piso, int techo)
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.Equal(piso, indice.Piso(valor)?.Precio);
        Assert.Equal(techo, indice.Techo(valor)?.Precio);
    }

    /// <summary>
    /// Fuera del catálogo no hay piso ni techo.
    /// </summary>
    [Fact]
    public void PisoYTecho_FueraDelCatalogoNoHay()
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.Null(indice.Piso(1000));
        Assert.Null(indice.Techo(70000));
    }

    /// <summary>
    /// El k-ésimo más barato para las posiciones del día.
    /// </summary>
    [Theory]
    [InlineData(1, 3000)]
    [InlineData(6, 27000)]
    [InlineData(7, 35000)]
    [InlineData(12, 62000)]
    public void KEsimo_DaElPrecioEsperado(int k, int esperado)
    {
        Assert.Equal(esperado, ConstruirIndice().KEsimo(k)?.Precio);
    }

    /// <summary>
    /// Un k fuera del índice no tiene resultado.
    /// </summary>
    [Fact]
    public void KEsimo_FueraDelIndiceNoHay()
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.Null(indice.KEsimo(0));
        Assert.Null(indice.KEsimo(13));
    }

    /// <summary>
    /// El conteo por debajo coincide con el inorden.
    /// </summary>
    [Theory]
    [InlineData(41000, 7)]
    [InlineData(35000, 6)]
    [InlineData(10000, 2)]
    [InlineData(1000, 0)]
    [InlineData(62000, 11)]
    public void ConteoPorDebajo_CoincideConElInorden(int valor, int esperado)
    {
        ArbolDePrecios indice = ConstruirIndice();
        List<int> precios = Precios(indice.EnOrden());
        int contador = 0;
        foreach (int precio in precios)
        {
            if (precio < valor)
            {
                contador++;
            }
        }

        Assert.Equal(esperado, indice.ConteoPorDebajo(valor));
        Assert.Equal(esperado, contador);
    }

    /// <summary>
    /// Un precio repetido no se inserta y no cambia el tamaño de la raíz.
    /// </summary>
    [Fact]
    public void Insertar_RechazaPrecioRepetido()
    {
        ArbolDePrecios indice = ConstruirIndice();

        Assert.False(indice.Insertar(35000, "Repetido"));
        Assert.Equal(12, indice.Contar());
        Assert.Equal(12, indice.Raiz?.Tamano);
    }

    /// <summary>
    /// El k-ésimo y el conteo por debajo se pueden obtener sin el inorden completo.
    /// </summary>
    [Fact]
    public void KEsimoYConteo_DanElMismoOrdenQueElInorden()
    {
        ArbolDePrecios indice = ConstruirIndice();
        List<int> precios = Precios(indice.EnOrden());

        for (int k = 1; k <= precios.Count; k++)
        {
            Assert.Equal(precios[k - 1], indice.KEsimo(k)?.Precio);
        }
    }

    /// <summary>
    /// El dibujo muestra el precio y el tamaño de la raíz en la primera línea.
    /// </summary>
    [Fact]
    public void Impresor_PoneLaRaizConSuTamanoEnLaPrimeraLinea()
    {
        List<string> lineas = ImpresorPrecios.RenderizarSimple(ConstruirIndice().Raiz);

        Assert.Contains("[35000:12]", lineas[0]);
    }
}
