namespace ClasificacionBecas.Modelos;

/// <summary>
/// Comprobación que hace una pregunta del árbol sobre una solicitud. Ninguna identifica a un apoyo.
/// </summary>
public enum Comprobacion
{
    /// <summary>
    /// La regla no hace pregunta: es un apoyo.
    /// </summary>
    Ninguna,

    /// <summary>
    /// El promedio acumulado alcanza el mínimo exigido.
    /// </summary>
    PromedioAlcanzaMinimo,

    /// <summary>
    /// La solicitud llegó antes del cierre de la convocatoria.
    /// </summary>
    LlegoAntesDelCierre,

    /// <summary>
    /// Se aprobó al menos el porcentaje mínimo de créditos.
    /// </summary>
    CreditosSuficientes,

    /// <summary>
    /// El estrato del hogar es 1 o 2.
    /// </summary>
    EstratoBajo,

    /// <summary>
    /// La familia está registrada en el Sisbén.
    /// </summary>
    EnSisben
}
