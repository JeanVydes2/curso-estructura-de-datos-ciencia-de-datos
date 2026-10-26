using CatalogoEstaciones.Modelos;

namespace CatalogoEstaciones;

/// <summary>
/// Los datos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve las 12 estaciones en el orden en que se instalaron, que es el orden de inserción.
    /// </summary>
    /// <returns>Las estaciones del catálogo inicial.</returns>
    public static List<Estacion> EstacionesInstaladas()
    {
        List<Estacion> estaciones = new List<Estacion>();
        estaciones.Add(new Estacion(201, "Ciénaga", "Nivel", 12.0));
        estaciones.Add(new Estacion(105, "El Banco", "Caudal", 480.0));
        estaciones.Add(new Estacion(302, "Minca", "Precipitación", 0.0));
        estaciones.Add(new Estacion(101, "Barrancabermeja", "Caudal", 5200.0));
        estaciones.Add(new Estacion(205, "Aracataca", "Nivel", 8.5));
        estaciones.Add(new Estacion(305, "Santa Marta", "Precipitación", 0.0));
        estaciones.Add(new Estacion(103, "Gamarra", "Caudal", 2400.0));
        estaciones.Add(new Estacion(208, "Fundación", "Nivel", 9.0));
        estaciones.Add(new Estacion(104, "Plato", "Caudal", 3300.0));
        estaciones.Add(new Estacion(303, "La Tagua", "Precipitación", 0.0));
        estaciones.Add(new Estacion(207, "Remolino", "Nivel", 7.5));
        estaciones.Add(new Estacion(301, "Pueblo Bello", "Precipitación", 0.0));
        return estaciones;
    }

    /// <summary>
    /// Devuelve la estación que se instala este año.
    /// </summary>
    /// <returns>La estación nueva.</returns>
    public static Estacion EstacionNueva()
    {
        return new Estacion(204, "Zapayán", "Nivel", 10.0);
    }

    /// <summary>
    /// Devuelve las estaciones que se dan de baja, en el orden del caso.
    /// </summary>
    /// <returns>Códigos de las estaciones que se retiran.</returns>
    public static List<int> CodigosQueSeRetiran()
    {
        List<int> codigos = new List<int>();
        codigos.Add(301);
        codigos.Add(101);
        codigos.Add(201);
        return codigos;
    }

    /// <summary>
    /// Devuelve las lecturas del reporte diario: solo interesa el código para validarlas.
    /// </summary>
    /// <returns>Códigos de las lecturas del lote.</returns>
    public static List<int> CodigosDeLecturas()
    {
        List<int> codigos = new List<int>();
        codigos.Add(103);
        codigos.Add(208);
        codigos.Add(999);
        return codigos;
    }
}
