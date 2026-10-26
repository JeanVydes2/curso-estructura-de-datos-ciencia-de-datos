namespace TriajeUrgencias.Modelos;

/// <summary>
/// Un evento de la mañana: llega un paciente o un médico queda libre y pide el siguiente.
/// </summary>
public class EventoDeSala
{
    /// <summary>
    /// Hora del evento, por ejemplo 08:15.
    /// </summary>
    public string Hora { get; }

    /// <summary>
    /// Paciente que llega, o <c>null</c> si el evento es un médico libre.
    /// </summary>
    public Paciente? PacienteQueLlega { get; }

    /// <summary>
    /// Crea un evento. Se usa a través de Llegada y MedicoLibre.
    /// </summary>
    private EventoDeSala(string hora, Paciente? pacienteQueLlega)
    {
        Hora = hora;
        PacienteQueLlega = pacienteQueLlega;
    }

    /// <summary>
    /// Crea el evento de llegada de un paciente a la sala.
    /// </summary>
    /// <param name="hora">Hora del evento.</param>
    /// <param name="paciente">Paciente que llega.</param>
    /// <returns>El evento de llegada.</returns>
    public static EventoDeSala Llegada(string hora, Paciente paciente)
    {
        return new EventoDeSala(hora, paciente);
    }

    /// <summary>
    /// Crea el evento de un médico que queda libre y pide el siguiente paciente.
    /// </summary>
    /// <param name="hora">Hora del evento.</param>
    /// <returns>El evento de médico libre.</returns>
    public static EventoDeSala MedicoLibre(string hora)
    {
        return new EventoDeSala(hora, null);
    }

    /// <summary>
    /// Indica si el evento es una llegada; si no, es un médico libre.
    /// </summary>
    public bool EsLlegada
    {
        get
        {
            return PacienteQueLlega != null;
        }
    }
}
