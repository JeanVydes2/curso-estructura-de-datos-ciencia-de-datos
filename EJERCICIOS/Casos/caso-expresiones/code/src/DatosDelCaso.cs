namespace TarifasExpresion;

/// <summary>
/// Los datos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve los convenios vigentes, cada uno con su fórmula en notación sufija.
    /// </summary>
    /// <returns>Los convenios del caso.</returns>
    public static List<Convenio> Convenios()
    {
        List<Convenio> convenios = new List<Convenio>();
        convenios.Add(new Convenio("Despacho urbano", "2.5 8 * 0.5 4 * + 6 -", 16));
        convenios.Add(new Convenio("Despacho intermunicipal", "5 3 + 4 *", 32));
        convenios.Add(new Convenio("Descuento por cliente frecuente", "10 2 3 * +", 16));
        return convenios;
    }

    /// <summary>
    /// Devuelve las notaciones mal formadas que el constructor debe rechazar.
    /// </summary>
    /// <returns>Notaciones y la explicación de lo que les falta.</returns>
    public static List<(string Notacion, string Explicacion)> NotacionesInvalidas()
    {
        List<(string, string)> invalidas = new List<(string, string)>();
        invalidas.Add(("3 +", "el operador + no tiene dos operandos"));
        invalidas.Add(("3 4 5 +", "sobran operandos: quedan dos árboles en la pila"));
        invalidas.Add(("3 4 ?", "el token ? no es un número ni un operador"));
        return invalidas;
    }
}
