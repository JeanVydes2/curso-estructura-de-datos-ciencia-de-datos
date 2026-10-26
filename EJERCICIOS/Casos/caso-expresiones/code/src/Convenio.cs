namespace TarifasExpresion;

/// <summary>
/// Datos de un convenio de tarifas: su nombre, la notación sufija de la fórmula y el valor esperado.
/// </summary>
/// <param name="Nombre">Nombre del convenio, por ejemplo "carga urbana".</param>
/// <param name="NotacionSufija">Fórmula en notación sufija, con tokens separados por espacios.</param>
/// <param name="ValorEsperado">Tarifa que la fórmula debe producir.</param>
public record Convenio(string Nombre, string NotacionSufija, double ValorEsperado);
