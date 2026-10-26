using TriajeUrgencias.Modelos;

namespace TriajeUrgencias;

/// <summary>
/// Los eventos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve los 11 eventos de la mañana: llegadas de pacientes y médicos que quedan libres.
    /// </summary>
    /// <returns>Los eventos de la mañana, en orden de hora.</returns>
    public static List<EventoDeSala> EventosDeLaManana()
    {
        List<EventoDeSala> eventos = new List<EventoDeSala>();
        eventos.Add(EventoDeSala.Llegada("08:00", new Paciente("P01", "apendicitis", 2, 1)));
        eventos.Add(EventoDeSala.Llegada("08:02", new Paciente("P02", "infarto", 1, 2)));
        eventos.Add(EventoDeSala.Llegada("08:05", new Paciente("P03", "consulta_general", 5, 3)));
        eventos.Add(EventoDeSala.Llegada("08:07", new Paciente("P04", "fractura_expuesta", 2, 4)));
        eventos.Add(EventoDeSala.MedicoLibre("08:09"));
        eventos.Add(EventoDeSala.MedicoLibre("08:11"));
        eventos.Add(EventoDeSala.Llegada("08:13", new Paciente("P05", "fiebre_alta", 3, 5)));
        eventos.Add(EventoDeSala.Llegada("08:15", new Paciente("P06", "trauma_severo", 1, 6)));
        eventos.Add(EventoDeSala.MedicoLibre("08:18"));
        eventos.Add(EventoDeSala.MedicoLibre("08:20"));
        eventos.Add(EventoDeSala.MedicoLibre("08:25"));
        return eventos;
    }
}
