namespace CatalogoEstaciones.Modelos;

/// <summary>
/// Ficha de una estación de la red hidrometeorológica. El código de tres dígitos
/// identifica la estación y ordena el catálogo: 1 = río Magdalena, 2 = Ciénaga
/// Grande y 3 = Sierra Nevada.
/// </summary>
public class Estacion
{
    /// <summary>
    /// Código de la estación, que es la clave del catálogo.
    /// </summary>
    public int Codigo { get; }

    /// <summary>
    /// Cuenca a la que pertenece la estación.
    /// </summary>
    public string Cuenca { get; }

    /// <summary>
    /// Municipio donde está instalada.
    /// </summary>
    public string Municipio { get; }

    /// <summary>
    /// Variable que mide la estación.
    /// </summary>
    public string Variable { get; }

    /// <summary>
    /// Caudal medio de referencia de la estación, en metros cúbicos por segundo.
    /// </summary>
    public double CaudalMedio { get; }

    /// <summary>
    /// Crea la ficha de una estación. La cuenca se deduce del primer dígito del código.
    /// </summary>
    /// <param name="codigo">Código de tres dígitos.</param>
    /// <param name="municipio">Municipio donde está instalada.</param>
    /// <param name="variable">Variable que mide.</param>
    /// <param name="caudalMedio">Caudal medio de referencia, en m³/s.</param>
    public Estacion(int codigo, string municipio, string variable, double caudalMedio)
    {
        Codigo = codigo;
        Cuenca = CuencaDe(codigo);
        Municipio = municipio;
        Variable = variable;
        CaudalMedio = caudalMedio;
    }

    /// <summary>
    /// Obtiene la cuenca a partir del primer dígito del código: 1 río Magdalena,
    /// 2 Ciénaga Grande y 3 Sierra Nevada.
    /// </summary>
    /// <param name="codigo">Código de la estación.</param>
    /// <returns>Nombre de la cuenca, o "Sin cuenca".</returns>
    public static string CuencaDe(int codigo)
    {
        int primerDigito = codigo / 100;
        if (primerDigito == 1)
        {
            return "Río Magdalena";
        }

        if (primerDigito == 2)
        {
            return "Ciénaga Grande";
        }

        if (primerDigito == 3)
        {
            return "Sierra Nevada";
        }

        return "Sin cuenca";
    }

    /// <summary>
    /// Devuelve el código, que es la clave y la etiqueta que se dibuja en el árbol.
    /// </summary>
    /// <returns>El código de la estación.</returns>
    public override string ToString()
    {
        return Codigo.ToString();
    }
}
