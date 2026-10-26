using TarifasExpresion;
using Xunit;

namespace TarifasExpresionTests;

public class TarifasExpresionTests
{
    /// <summary>
    /// Construye el árbol de un convenio del caso por su nombre.
    /// </summary>
    private static NodoExpresion ArbolDelConvenio(string nombre)
    {
        foreach (Convenio convenio in DatosDelCaso.Convenios())
        {
            if (convenio.Nombre == nombre)
            {
                return ConstructorDeExpresion.Construir(convenio.NotacionSufija);
            }
        }

        throw new ArgumentException("No existe el convenio " + nombre);
    }

    /// <summary>
    /// El sufijo del árbol coincide con la notación de entrada.
    /// </summary>
    [Theory]
    [InlineData("Despacho urbano", "2.5 8 * 0.5 4 * + 6 -")]
    [InlineData("Despacho intermunicipal", "5 3 + 4 *")]
    [InlineData("Descuento por cliente frecuente", "10 2 3 * +")]
    public void Sufijo_CoincideConLaNotacionDeEntrada(string nombre, string notacion)
    {
        Assert.Equal(notacion, ArbolDeExpresion.Sufijo(ArbolDelConvenio(nombre)));
    }

    /// <summary>
    /// El prefijo y el infijo siguen el orden de cada recorrido.
    /// </summary>
    [Fact]
    public void PrefijoEInfijo_SiguenSuRecorrido()
    {
        NodoExpresion urbano = ArbolDelConvenio("Despacho urbano");
        NodoExpresion intermunicipal = ArbolDelConvenio("Despacho intermunicipal");

        Assert.Equal("- + * 2.5 8 * 0.5 4 6", ArbolDeExpresion.Prefijo(urbano));
        Assert.Equal("((2.5 * 8) + (0.5 * 4)) - 6", ArbolDeExpresion.Infijo(urbano));
        Assert.Equal("* + 5 3 4", ArbolDeExpresion.Prefijo(intermunicipal));
        Assert.Equal("(5 + 3) * 4", ArbolDeExpresion.Infijo(intermunicipal));
    }

    /// <summary>
    /// La tarifa de cada convenio es la del caso.
    /// </summary>
    [Theory]
    [InlineData("Despacho urbano", 16.0)]
    [InlineData("Despacho intermunicipal", 32.0)]
    [InlineData("Descuento por cliente frecuente", 16.0)]
    public void Evaluar_DaLaTarifaDelConvenio(string nombre, double esperado)
    {
        Assert.Equal(esperado, ArbolDeExpresion.Evaluar(ArbolDelConvenio(nombre)), 6);
    }

    /// <summary>
    /// Las métricas de cada fórmula coinciden con el análisis del caso.
    /// </summary>
    [Fact]
    public void Metricas_CoincidenConElAnalisis()
    {
        NodoExpresion urbano = ArbolDelConvenio("Despacho urbano");
        NodoExpresion intermunicipal = ArbolDelConvenio("Despacho intermunicipal");

        Assert.Equal(9, ArbolDeExpresion.ContarNodos(urbano));
        Assert.Equal(5, ArbolDeExpresion.ContarHojas(urbano));
        Assert.Equal(4, ArbolDeExpresion.ContarOperadores(urbano));
        Assert.Equal(3, ArbolDeExpresion.Altura(urbano));
        Assert.Equal(4, ArbolDeExpresion.Ancho(urbano));

        Assert.Equal(5, ArbolDeExpresion.ContarNodos(intermunicipal));
        Assert.Equal(3, ArbolDeExpresion.ContarHojas(intermunicipal));
        Assert.Equal(2, ArbolDeExpresion.ContarOperadores(intermunicipal));
        Assert.Equal(2, ArbolDeExpresion.Altura(intermunicipal));
        Assert.Equal(2, ArbolDeExpresion.Ancho(intermunicipal));
    }

    /// <summary>
    /// En las fórmulas del caso, hojas = operadores + 1.
    /// </summary>
    [Fact]
    public void FormulasDelCaso_SonArbolesLlenos()
    {
        foreach (Convenio convenio in DatosDelCaso.Convenios())
        {
            NodoExpresion raiz = ConstructorDeExpresion.Construir(convenio.NotacionSufija);

            Assert.Equal(1 + ArbolDeExpresion.ContarOperadores(raiz), ArbolDeExpresion.ContarHojas(raiz));
        }
    }

    /// <summary>
    /// El primer operando que sale de la pila es el derecho: restar en el otro sentido cambia el valor.
    /// </summary>
    [Fact]
    public void Resta_RespetaElOrdenDeLosOperandos()
    {
        NodoExpresion raiz = ConstructorDeExpresion.Construir("10 4 -");

        Assert.Equal(6.0, ArbolDeExpresion.Evaluar(raiz), 6);
        Assert.Equal("10 4 -", ArbolDeExpresion.Sufijo(raiz));
    }

    /// <summary>
    /// Un operador sin sus dos operandos detiene la construcción.
    /// </summary>
    [Fact]
    public void Construir_RechazaOperadorSinOperandos()
    {
        Assert.Throws<InvalidOperationException>(() => ConstructorDeExpresion.Construir("3 +"));
    }

    /// <summary>
    /// Una notación con operandos de más detiene la construcción.
    /// </summary>
    [Fact]
    public void Construir_RechazaOperandosSobrantes()
    {
        Assert.Throws<InvalidOperationException>(() => ConstructorDeExpresion.Construir("3 4 5 +"));
    }

    /// <summary>
    /// Un token que no es número ni operador detiene la construcción.
    /// </summary>
    [Fact]
    public void Construir_RechazaTokenDesconocido()
    {
        Assert.Throws<InvalidOperationException>(() => ConstructorDeExpresion.Construir("3 4 ?"));
    }

    /// <summary>
    /// El dibujo empieza con la raíz y tiene una línea de nodos y otra de conectores por nivel.
    /// </summary>
    [Fact]
    public void Impresor_PoneLaRaizEnLaPrimeraLinea()
    {
        List<string> lineas = ImpresorExpresion.RenderizarSimple(ArbolDelConvenio("Despacho urbano"));

        Assert.Contains("[-]", lineas[0]);
        Assert.Equal(7, lineas.Count);
    }
}
