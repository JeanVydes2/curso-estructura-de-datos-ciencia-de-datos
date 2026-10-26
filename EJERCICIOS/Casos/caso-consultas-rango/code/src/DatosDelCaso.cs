namespace IndiceDePrecios;

/// <summary>
/// Los datos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve los precios registrados esta semana, en el orden en que llegaron los lotes.
    /// </summary>
    /// <returns>Los 12 productos con su precio.</returns>
    public static List<Producto> Precios()
    {
        List<Producto> precios = new List<Producto>();
        precios.Add(new Producto("Café pergamino", 35000));
        precios.Add(new Producto("Plátano", 12000));
        precios.Add(new Producto("Cacao", 48000));
        precios.Add(new Producto("Yuca", 8000));
        precios.Add(new Producto("Ají", 27000));
        precios.Add(new Producto("Aguacate Hass", 55000));
        precios.Add(new Producto("Mango", 19000));
        precios.Add(new Producto("Camarón", 62000));
        precios.Add(new Producto("Ñame", 3000));
        precios.Add(new Producto("Panela", 41000));
        precios.Add(new Producto("Ahuyama", 23000));
        precios.Add(new Producto("Queso costeño", 50000));
        return precios;
    }

    /// <summary>
    /// Devuelve los valores de referencia de las consultas de piso y techo.
    /// </summary>
    /// <returns>Valores de las consultas del día.</returns>
    public static List<int> ValoresDePisoYTecho()
    {
        List<int> valores = new List<int>();
        valores.Add(30000);
        valores.Add(3000);
        valores.Add(62000);
        valores.Add(1000);
        valores.Add(70000);
        return valores;
    }

    /// <summary>
    /// Devuelve las posiciones k que se consultan del más barato al más caro.
    /// </summary>
    /// <returns>Posiciones k de las consultas del día.</returns>
    public static List<int> PosicionesK()
    {
        List<int> posiciones = new List<int>();
        posiciones.Add(1);
        posiciones.Add(6);
        posiciones.Add(7);
        posiciones.Add(12);
        return posiciones;
    }

    /// <summary>
    /// Devuelve el valor de referencia del conteo por debajo.
    /// </summary>
    /// <returns>Valor de referencia, en pesos.</returns>
    public static int ValorDeConteo()
    {
        return 41000;
    }
}
