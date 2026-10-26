using ClasificacionBecas.Modelos;

namespace ClasificacionBecas;

/// <summary>
/// Los datos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve la tabla de reglas del caso, en el orden en que salió de la herramienta de configuración.
    /// </summary>
    /// <remarks>
    /// Las filas no están en el orden del árbol: la raíz hay que encontrarla.
    /// </remarks>
    /// <returns>Las 11 filas de la tabla.</returns>
    public static List<Regla> Reglas()
    {
        List<Regla> reglas = new List<Regla>();
        reglas.Add(new Regla("R2", "¿El promedio acumulado es al menos 3,8?", Comprobacion.PromedioAlcanzaMinimo, "R4", "R1"));
        reglas.Add(Regla.Apoyo("D6", "Rechazar: cupos agotados para el segundo periodo"));
        reglas.Add(new Regla("R1", "¿La solicitud llegó antes del cierre de convocatoria?", Comprobacion.LlegoAntesDelCierre, "R3", "D6"));
        reglas.Add(Regla.Apoyo("D1", "Asignar beca completa"));
        reglas.Add(new Regla("R4", "¿El estrato del hogar es 1 o 2?", Comprobacion.EstratoBajo, "D2", "R5"));
        reglas.Add(new Regla("R3", "¿Aprobó al menos el 80 % de los créditos?", Comprobacion.CreditosSuficientes, "D3", "D5"));
        reglas.Add(Regla.Apoyo("D2", "Asignar beca completa y apoyo de transporte"));
        reglas.Add(new Regla("R5", "¿La familia está registrada en el Sisbén?", Comprobacion.EnSisben, "D1", "D4"));
        reglas.Add(Regla.Apoyo("D3", "Asignar media beca"));
        reglas.Add(Regla.Apoyo("D4", "Asignar apoyo de transporte"));
        reglas.Add(Regla.Apoyo("D5", "Asignar media beca condicionada a matrícula de honor"));
        return reglas;
    }

    /// <summary>
    /// Devuelve las solicitudes del semestre con los datos del sistema académico.
    /// </summary>
    /// <returns>Las 7 solicitudes, en orden de llegada.</returns>
    public static List<Solicitud> Solicitudes()
    {
        List<Solicitud> solicitudes = new List<Solicitud>();
        solicitudes.Add(new Solicitud("S-1042", "Andrea Mejía", 3.4, 76.0, 3, true, false, 1));
        solicitudes.Add(new Solicitud("S-1043", "Luis Barrios", 4.2, 92.0, 1, true, true, 2));
        solicitudes.Add(new Solicitud("S-1044", "Carla Ospino", 4.0, 85.0, 3, true, true, 3));
        solicitudes.Add(new Solicitud("S-1045", "Jorge Padilla", 3.9, 70.0, 2, true, true, 4));
        solicitudes.Add(new Solicitud("S-1046", "Yuliana Ríos", 3.2, 90.0, 2, true, true, 5));
        solicitudes.Add(new Solicitud("S-1047", "Moisés Cantillo", 3.6, 65.0, 2, false, true, 6));
        solicitudes.Add(new Solicitud("S-1048", "Valentina Sarmiento", 4.5, 88.0, 3, false, true, 7));
        return solicitudes;
    }
}
