using CatalogoEstaciones;
using CatalogoEstaciones.Modelos;
using Xunit;

namespace CatalogoEstacionesTests;

public class CatalogoEstacionesTests
{
    /// <summary>
    /// Inserta las estaciones del caso en el orden de instalación.
    /// </summary>
    private static ArbolBinarioBusqueda ConstruirCatalogo()
    {
        ArbolBinarioBusqueda catalogo = new ArbolBinarioBusqueda();
        foreach (Estacion estacion in DatosDelCaso.EstacionesInstaladas())
        {
            catalogo.Insertar(estacion);
        }

        return catalogo;
    }

    /// <summary>
    /// Obtiene los códigos de una lista de estaciones.
    /// </summary>
    private static List<int> Codigos(List<Estacion> estaciones)
    {
        List<int> codigos = new List<int>();
        foreach (Estacion estacion in estaciones)
        {
            codigos.Add(estacion.Codigo);
        }

        return codigos;
    }

    /// <summary>
    /// Insertar en el orden de instalación produce un árbol válido de altura 4 con raíz 201.
    /// </summary>
    [Fact]
    public void Construir_ElOrdenDeInstalacionDaAlturaCuatro()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();

        Assert.Equal(12, catalogo.Contar());
        Assert.Equal(4, catalogo.Altura());
        Assert.Equal(201, catalogo.Raiz?.Valor.Codigo);
        Assert.True(catalogo.EsValido());
    }

    /// <summary>
    /// El inorden lista los códigos en orden creciente.
    /// </summary>
    [Fact]
    public void EnOrden_ListaLosCodigosCrecientes()
    {
        List<int> codigos = Codigos(ConstruirCatalogo().EnOrden());

        Assert.Equal(new List<int> { 101, 103, 104, 105, 201, 205, 207, 208, 301, 302, 303, 305 }, codigos);
    }

    /// <summary>
    /// El inorden queda ordenado porque el árbol cumple el invariante y Min y Max son sus extremos.
    /// </summary>
    [Fact]
    public void MinimoYMaximo_SonLosExtremosDelInorden()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();
        List<int> codigos = Codigos(catalogo.EnOrden());

        Assert.Equal(codigos[0], catalogo.Minimo()?.Codigo);
        Assert.Equal(codigos[codigos.Count - 1], catalogo.Maximo()?.Codigo);
        Assert.Equal(101, catalogo.Minimo()?.Codigo);
        Assert.Equal(305, catalogo.Maximo()?.Codigo);
    }

    /// <summary>
    /// Un catálogo vacío no tiene mínimo ni máximo.
    /// </summary>
    [Fact]
    public void MinimoYMaximo_DeUnCatalogoVacioSonNulos()
    {
        ArbolBinarioBusqueda catalogo = new ArbolBinarioBusqueda();

        Assert.Null(catalogo.Minimo());
        Assert.Null(catalogo.Maximo());
    }

    /// <summary>
    /// Cada búsqueda del lote hace las comparaciones esperadas.
    /// </summary>
    [Theory]
    [InlineData(103, 4, true)]
    [InlineData(208, 4, true)]
    [InlineData(999, 3, false)]
    public void CaminoDeBusqueda_CuentaLasComparaciones(int codigo, int comparaciones, bool existe)
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();

        Assert.Equal(comparaciones, catalogo.CaminoDeBusqueda(codigo).Count);
        Assert.Equal(existe, catalogo.Buscar(codigo) != null);
    }

    /// <summary>
    /// La estación 204 queda como hijo izquierdo de 205 y en la Ciénaga Grande.
    /// </summary>
    [Fact]
    public void Insertar_LaEstacionNuevaEsHijoIzquierdoDe205()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();

        Assert.True(catalogo.Insertar(DatosDelCaso.EstacionNueva()));
        Assert.Equal(204, catalogo.Buscar(205)?.Izquierdo?.Valor.Codigo);
        Assert.Equal("Ciénaga Grande", catalogo.Buscar(204)?.Valor.Cuenca);
    }

    /// <summary>
    /// Un código repetido no se inserta.
    /// </summary>
    [Fact]
    public void Insertar_RechazaCodigoRepetido()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();

        Assert.False(catalogo.Insertar(new Estacion(201, "Repetida", "Nivel", 0)));
        Assert.Equal(12, catalogo.Contar());
    }

    /// <summary>
    /// Los tres retiros del caso corresponden a hoja, un hijo y dos hijos.
    /// </summary>
    [Fact]
    public void CasoDeEliminacion_CoincideConLosTresRetiros()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();
        catalogo.Insertar(DatosDelCaso.EstacionNueva());

        Assert.Equal(ArbolBinarioBusqueda.CasoHoja, catalogo.CasoDeEliminacion(301));
        catalogo.Eliminar(301);
        Assert.Equal(ArbolBinarioBusqueda.CasoUnHijo, catalogo.CasoDeEliminacion(101));
        catalogo.Eliminar(101);
        Assert.Equal(ArbolBinarioBusqueda.CasoDosHijos, catalogo.CasoDeEliminacion(201));
        Assert.Equal(204, catalogo.SucesorInorden(201));
    }

    /// <summary>
    /// Después de los retiros, el inorden sigue creciente y 204 ocupa el lugar de 201.
    /// </summary>
    [Fact]
    public void Eliminar_LaSecuenciaConservaElOrdenYUsaElSucesor()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();
        catalogo.Insertar(DatosDelCaso.EstacionNueva());
        foreach (int codigo in DatosDelCaso.CodigosQueSeRetiran())
        {
            Assert.True(catalogo.Eliminar(codigo));
        }

        Assert.Equal(new List<int> { 103, 104, 105, 204, 205, 207, 208, 302, 303, 305 }, Codigos(catalogo.EnOrden()));
        Assert.Equal(204, catalogo.Raiz?.Valor.Codigo);
        Assert.Equal(105, catalogo.Raiz?.Izquierdo?.Valor.Codigo);
        Assert.Equal(302, catalogo.Raiz?.Derecho?.Valor.Codigo);
        Assert.Equal(4, catalogo.Altura());
        Assert.True(catalogo.EsValido());
    }

    /// <summary>
    /// Retirar un código inexistente no cambia el catálogo.
    /// </summary>
    [Fact]
    public void Eliminar_UnCodigoDesconocidoDevuelveFalso()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();

        Assert.False(catalogo.Eliminar(999));
        Assert.Equal(12, catalogo.Contar());
    }

    /// <summary>
    /// El reporte de una cuenca devuelve sus estaciones sin visitar todo el árbol.
    /// </summary>
    [Fact]
    public void Rango_LaCuencaCienagaVisitaSoloLasRamasPosibles()
    {
        ArbolBinarioBusqueda catalogo = ConstruirCatalogo();
        catalogo.Insertar(DatosDelCaso.EstacionNueva());
        List<NodoBinario<Estacion>> visitados = new List<NodoBinario<Estacion>>();

        List<Estacion> cienaga = catalogo.Rango(200, 299, visitados);

        Assert.Equal(new List<int> { 201, 204, 205, 207, 208 }, Codigos(cienaga));
        Assert.True(visitados.Count < catalogo.Contar());
    }

    /// <summary>
    /// Insertar los códigos ordenados produce una cadena de altura 11.
    /// </summary>
    [Fact]
    public void CargaOrdenada_DegeneraEnUnaCadena()
    {
        List<Estacion> ordenadas = DatosDelCaso.EstacionesInstaladas();
        ordenadas.Sort(new ComparadorDeCodigo());
        ArbolBinarioBusqueda cadena = new ArbolBinarioBusqueda();
        foreach (Estacion estacion in ordenadas)
        {
            cadena.Insertar(estacion);
        }

        Assert.Equal(11, cadena.Altura());
    }

    /// <summary>
    /// La cuenca se obtiene del primer dígito del código.
    /// </summary>
    [Fact]
    public void CuencaDe_UsaElPrimerDigito()
    {
        Assert.Equal("Río Magdalena", Estacion.CuencaDe(103));
        Assert.Equal("Ciénaga Grande", Estacion.CuencaDe(204));
        Assert.Equal("Sierra Nevada", Estacion.CuencaDe(305));
        Assert.Equal("Sin cuenca", Estacion.CuencaDe(999));
    }

    /// <summary>
    /// El dibujo empieza con la raíz y tiene una línea de nodos y otra de conectores por nivel.
    /// </summary>
    [Fact]
    public void Impresor_PoneLaRaizEnLaPrimeraLinea()
    {
        List<string> lineas = ImpresorArbol<Estacion>.RenderizarSimple(ConstruirCatalogo().Raiz);

        Assert.Contains("[201]", lineas[0]);
        Assert.Equal(9, lineas.Count);
    }
}
