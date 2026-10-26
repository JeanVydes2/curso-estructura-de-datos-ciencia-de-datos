namespace TriajeUrgencias.Modelos;

/// <summary>
/// Un paciente admitido en la sala de urgencias. Prioridad es el nivel de triaje
/// (1 es lo más urgente) y OrdenDeLlegada desempata.
/// </summary>
public class Paciente
{
    /// <summary>
    /// Identificador del paciente, por ejemplo P03.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Motivo de la consulta.
    /// </summary>
    public string Motivo { get; }

    /// <summary>
    /// Nivel de triaje: 1 emergencia vital, 5 atención general.
    /// </summary>
    public int Prioridad { get; }

    /// <summary>
    /// Orden de llegada a la sala; se usa para desempatar.
    /// </summary>
    public int OrdenDeLlegada { get; }

    /// <summary>
    /// Crea un paciente admitido en la sala.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="motivo">Motivo de la consulta.</param>
    /// <param name="prioridad">Nivel de triaje, de 1 a 5.</param>
    /// <param name="ordenDeLlegada">Orden de llegada a la sala.</param>
    public Paciente(string id, string motivo, int prioridad, int ordenDeLlegada)
    {
        Id = id;
        Motivo = motivo;
        Prioridad = prioridad;
        OrdenDeLlegada = ordenDeLlegada;
    }

    /// <summary>
    /// Devuelve la etiqueta corta del dibujo: identificador y prioridad, por ejemplo P06:1.
    /// </summary>
    /// <returns>El identificador y la prioridad del paciente.</returns>
    public override string ToString()
    {
        return Id + ":" + Prioridad;
    }
}
