using ClasificacionBecas;
using ClasificacionBecas.Modelos;
using Xunit;

namespace ClasificacionBecasTests;

public class ClasificacionBecasTests
{
    /// <summary>
    /// Construye el árbol con la tabla de reglas del caso.
    /// </summary>
    private static ArbolBinario<Regla> ConstruirArbolDelCaso()
    {
        return ConstructorDeArbol.Construir(DatosDelCaso.Reglas());
    }

    /// <summary>
    /// Obtiene los identificadores de una lista de nodos.
    /// </summary>
    private static List<string> Ids(List<NodoBinario<Regla>> nodos)
    {
        List<string> ids = new List<string>();
        foreach (NodoBinario<Regla> nodo in nodos)
        {
            ids.Add(nodo.Valor.Id);
        }

        return ids;
    }

    /// <summary>
    /// Busca una solicitud del caso por su identificador.
    /// </summary>
    private static Solicitud SolicitudConId(string id)
    {
        foreach (Solicitud solicitud in DatosDelCaso.Solicitudes())
        {
            if (solicitud.Id == id)
            {
                return solicitud;
            }
        }

        throw new ArgumentException("No existe la solicitud " + id);
    }

    /// <summary>
    /// La raíz es R2, la única regla que no es salida de otra.
    /// </summary>
    [Fact]
    public void Construir_EncuentraR2ComoUnicaRaiz()
    {
        ArbolBinario<Regla> arbol = ConstruirArbolDelCaso();

        Assert.NotNull(arbol.Raiz);
        Assert.Equal("R2", arbol.Raiz!.Valor.Id);
    }

    /// <summary>
    /// La salida Sí queda en Izquierdo y la salida No en Derecho.
    /// </summary>
    [Fact]
    public void Construir_ConectaSiAIzquierdaYNoADerecha()
    {
        ArbolBinario<Regla> arbol = ConstruirArbolDelCaso();
        NodoBinario<Regla>? r4 = ConstructorDeArbol.BuscarPorId(arbol, "R4");

        Assert.NotNull(r4);
        Assert.Equal("D2", r4!.Izquierdo?.Valor.Id);
        Assert.Equal("R5", r4.Derecho?.Valor.Id);
    }

    /// <summary>
    /// Nodos, hojas, altura y árbol lleno coinciden con el análisis del caso.
    /// </summary>
    [Fact]
    public void Metricas_CoincidenConElCaso()
    {
        ArbolBinario<Regla> arbol = ConstruirArbolDelCaso();

        Assert.Equal(11, arbol.ContarNodos());
        Assert.Equal(6, arbol.ContarHojas());
        Assert.Equal(3, arbol.Altura());
        Assert.True(arbol.EsLleno());
    }

    /// <summary>
    /// Preorden, inorden y postorden siguen el orden de visita de cada recorrido.
    /// </summary>
    [Fact]
    public void Recorridos_SiguenSusReglas()
    {
        ArbolBinario<Regla> arbol = ConstruirArbolDelCaso();

        Assert.Equal(new List<string> { "R2", "R4", "D2", "R5", "D1", "D4", "R1", "R3", "D3", "D5", "D6" }, Ids(arbol.Preorden()));
        Assert.Equal(new List<string> { "D2", "R4", "D1", "R5", "D4", "R2", "D3", "R3", "D5", "R1", "D6" }, Ids(arbol.Inorden()));
        Assert.Equal(new List<string> { "D2", "D1", "D4", "R5", "R4", "D3", "D5", "R3", "D6", "R1", "R2" }, Ids(arbol.Postorden()));
    }

    /// <summary>
    /// El recorrido por niveles agrupa 1, 2, 4 y 4 nodos.
    /// </summary>
    [Fact]
    public void Niveles_CuentanNodosPorNivel()
    {
        List<List<NodoBinario<Regla>>> niveles = ConstruirArbolDelCaso().Niveles();
        List<int> cantidades = new List<int>();
        foreach (List<NodoBinario<Regla>> nivel in niveles)
        {
            cantidades.Add(nivel.Count);
        }

        Assert.Equal(new List<int> { 1, 2, 4, 4 }, cantidades);
    }

    /// <summary>
    /// Cada solicitud del semestre llega a su apoyo visitando los nodos esperados.
    /// </summary>
    [Theory]
    [InlineData("S-1042", "D6", 3)]
    [InlineData("S-1043", "D2", 3)]
    [InlineData("S-1044", "D1", 4)]
    [InlineData("S-1045", "D2", 3)]
    [InlineData("S-1046", "D3", 4)]
    [InlineData("S-1047", "D5", 4)]
    [InlineData("S-1048", "D4", 4)]
    public void Clasificar_CadaSolicitudLlegaASuApoyo(string idSolicitud, string apoyo, int nodosVisitados)
    {
        List<NodoBinario<Regla>> camino = ClasificadorBecas.Clasificar(ConstruirArbolDelCaso(), SolicitudConId(idSolicitud));

        Assert.Equal(apoyo, camino[camino.Count - 1].Valor.Id);
        Assert.Equal(nodosVisitados, camino.Count);
    }

    /// <summary>
    /// El camino hasta D5 pasa por R2 y R1.
    /// </summary>
    [Fact]
    public void CaminoHacia_LaApelacionPasaPorDosRespuestasNegativas()
    {
        ArbolBinario<Regla> arbol = ConstruirArbolDelCaso();
        NodoBinario<Regla>? objetivo = ConstructorDeArbol.BuscarPorId(arbol, "D5");

        Assert.NotNull(objetivo);
        Assert.Equal(new List<string> { "R2", "R1", "R3", "D5" }, Ids(arbol.CaminoHacia(objetivo!)));
    }

    /// <summary>
    /// Una salida que apunta a una regla inexistente detiene la construcción.
    /// </summary>
    [Fact]
    public void Construir_RechazaSalidaInexistente()
    {
        List<Regla> reglas = new List<Regla>();
        reglas.Add(new Regla("R1", "¿Pregunta?", Comprobacion.PromedioAlcanzaMinimo, "D1", "D9"));
        reglas.Add(Regla.Apoyo("D1", "Apoyo"));

        Assert.Throws<InvalidOperationException>(() => ConstructorDeArbol.Construir(reglas));
    }

    /// <summary>
    /// Una tabla con dos reglas sin padre no describe un árbol.
    /// </summary>
    [Fact]
    public void Construir_RechazaDosRaices()
    {
        List<Regla> reglas = new List<Regla>();
        reglas.Add(new Regla("R1", "¿Pregunta?", Comprobacion.PromedioAlcanzaMinimo, "D1", "D2"));
        reglas.Add(Regla.Apoyo("D1", "Apoyo 1"));
        reglas.Add(Regla.Apoyo("D2", "Apoyo 2"));
        reglas.Add(Regla.Apoyo("D3", "Apoyo suelto"));

        Assert.Throws<InvalidOperationException>(() => ConstructorDeArbol.Construir(reglas));
    }

    /// <summary>
    /// Una regla que es salida de dos preguntas no describe un árbol.
    /// </summary>
    [Fact]
    public void Construir_RechazaReglaConDosPadres()
    {
        List<Regla> reglas = new List<Regla>();
        reglas.Add(new Regla("R1", "¿Pregunta 1?", Comprobacion.PromedioAlcanzaMinimo, "R2", "D1"));
        reglas.Add(new Regla("R2", "¿Pregunta 2?", Comprobacion.LlegoAntesDelCierre, "D1", "D2"));
        reglas.Add(Regla.Apoyo("D1", "Apoyo 1"));
        reglas.Add(Regla.Apoyo("D2", "Apoyo 2"));

        Assert.Throws<InvalidOperationException>(() => ConstructorDeArbol.Construir(reglas));
    }

    /// <summary>
    /// El dibujo empieza con la raíz y tiene una línea de nodos y otra de conectores por nivel.
    /// </summary>
    [Fact]
    public void Impresor_PoneLaRaizEnLaPrimeraLinea()
    {
        List<string> lineas = ImpresorConsola<Regla>.RenderizarSimple(ConstruirArbolDelCaso().Raiz);

        Assert.Contains("[R2]", lineas[0]);
        Assert.Equal(7, lineas.Count);
    }
}
