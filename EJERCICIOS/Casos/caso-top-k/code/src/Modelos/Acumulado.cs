namespace LluviaTopK.Modelos;

/// <summary>
/// Acumulado de lluvia de un pluviómetro. Milimetros es la clave que ordena el Top-K
/// y Id desempata para que el resultado sea estable.
/// </summary>
public class Acumulado
{
    /// <summary>
    /// Identificador del pluviómetro, por ejemplo P03.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Lluvia acumulada en el mes, en milímetros.
    /// </summary>
    public double Milimetros { get; }

    /// <summary>
    /// Crea el acumulado de un pluviómetro.
    /// </summary>
    /// <param name="id">Identificador del pluviómetro.</param>
    /// <param name="milimetros">Lluvia acumulada, en milímetros.</param>
    public Acumulado(string id, double milimetros)
    {
        Id = id;
        Milimetros = milimetros;
    }

    /// <summary>
    /// Devuelve la etiqueta corta del dibujo: identificador y milímetros, por ejemplo P09:65,7.
    /// </summary>
    /// <returns>El identificador y los milímetros del acumulado.</returns>
    public override string ToString()
    {
        return Id + ":" + Milimetros.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("es-CO"));
    }
}
