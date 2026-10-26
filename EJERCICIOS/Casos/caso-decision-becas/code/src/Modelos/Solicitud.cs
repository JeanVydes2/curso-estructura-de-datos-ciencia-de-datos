namespace ClasificacionBecas.Modelos;

/// <summary>
/// Una solicitud de beca con los datos que revisa el sistema académico.
/// </summary>
public class Solicitud
{
    /// <summary>
    /// Promedio acumulado mínimo que exige el comité.
    /// </summary>
    public const double PromedioMinimo = 3.8;

    /// <summary>
    /// Porcentaje mínimo de créditos aprobados que exige el comité.
    /// </summary>
    public const double CreditosMinimos = 80.0;

    /// <summary>
    /// Estrato del hogar que cuenta como bajo para el apoyo de transporte.
    /// </summary>
    public const int EstratoMaximoBajo = 2;

    /// <summary>
    /// Identificador de la solicitud.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Nombre del estudiante que solicita la beca.
    /// </summary>
    public string Estudiante { get; }

    /// <summary>
    /// Promedio acumulado del estudiante.
    /// </summary>
    public double Promedio { get; }

    /// <summary>
    /// Porcentaje de créditos aprobados sobre los matriculados.
    /// </summary>
    public double PorcentajeCreditos { get; }

    /// <summary>
    /// Estrato del hogar, de 1 a 6.
    /// </summary>
    public int Estrato { get; }

    /// <summary>
    /// Indica si la familia está registrada en el Sisbén.
    /// </summary>
    public bool EnSisben { get; }

    /// <summary>
    /// Indica si la solicitud llegó antes del cierre de la convocatoria.
    /// </summary>
    public bool LlegoAntesDelCierre { get; }

    /// <summary>
    /// Orden de llegada de la solicitud; la tabla de reglas se lee en ese orden.
    /// </summary>
    public int OrdenDeLlegada { get; }

    /// <summary>
    /// Registra una solicitud con los datos del sistema académico.
    /// </summary>
    /// <param name="id">Identificador de la solicitud.</param>
    /// <param name="estudiante">Nombre del estudiante.</param>
    /// <param name="promedio">Promedio acumulado.</param>
    /// <param name="porcentajeCreditos">Porcentaje de créditos aprobados.</param>
    /// <param name="estrato">Estrato del hogar.</param>
    /// <param name="enSisben">Indica si la familia está en el Sisbén.</param>
    /// <param name="llegoAntesDelCierre">Indica si llegó antes del cierre de la convocatoria.</param>
    /// <param name="ordenDeLlegada">Orden de llegada de la solicitud.</param>
    public Solicitud(string id, string estudiante, double promedio, double porcentajeCreditos, int estrato,
        bool enSisben, bool llegoAntesDelCierre, int ordenDeLlegada)
    {
        Id = id;
        Estudiante = estudiante;
        Promedio = promedio;
        PorcentajeCreditos = porcentajeCreditos;
        Estrato = estrato;
        EnSisben = enSisben;
        LlegoAntesDelCierre = llegoAntesDelCierre;
        OrdenDeLlegada = ordenDeLlegada;
    }

    /// <summary>
    /// Responde la pregunta de una regla para esta solicitud.
    /// </summary>
    /// <param name="chequeo">Comprobación que hace la pregunta.</param>
    /// <returns><c>true</c> si la respuesta es Sí; <c>false</c> si es No.</returns>
    /// <exception cref="ArgumentException">Si la comprobación es la de un apoyo.</exception>
    public bool Responde(Comprobacion chequeo)
    {
        switch (chequeo)
        {
            case Comprobacion.PromedioAlcanzaMinimo:
                return Promedio >= PromedioMinimo;
            case Comprobacion.LlegoAntesDelCierre:
                return LlegoAntesDelCierre;
            case Comprobacion.CreditosSuficientes:
                return PorcentajeCreditos >= CreditosMinimos;
            case Comprobacion.EstratoBajo:
                return Estrato <= EstratoMaximoBajo;
            case Comprobacion.EnSisben:
                return EnSisben;
            default:
                throw new ArgumentException("Un apoyo no hace preguntas.", nameof(chequeo));
        }
    }
}
