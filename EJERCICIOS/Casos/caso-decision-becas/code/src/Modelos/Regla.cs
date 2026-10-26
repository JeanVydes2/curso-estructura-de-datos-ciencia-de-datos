namespace ClasificacionBecas.Modelos;

/// <summary>
/// Una fila de la tabla de reglas.
/// Una pregunta tiene dos salidas (IdSi e IdNo); un apoyo no tiene salidas.
/// </summary>
public class Regla
{
    /// <summary>
    /// Identificador de la regla.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Pregunta o descripción del apoyo.
    /// </summary>
    public string Texto { get; }

    /// <summary>
    /// Comprobación de la pregunta; <c>Comprobacion.Ninguna</c> en un apoyo.
    /// </summary>
    public Comprobacion Chequeo { get; }

    /// <summary>
    /// Regla a la que lleva la respuesta Sí.
    /// </summary>
    public string IdSi { get; }

    /// <summary>
    /// Regla a la que lleva la respuesta No.
    /// </summary>
    public string IdNo { get; }

    /// <summary>
    /// Crea una fila de la tabla de reglas.
    /// </summary>
    /// <param name="id">Identificador de la regla.</param>
    /// <param name="texto">Pregunta o descripción del apoyo.</param>
    /// <param name="chequeo">Comprobación de la pregunta; <c>Comprobacion.Ninguna</c> en un apoyo.</param>
    /// <param name="idSi">Regla a la que lleva la respuesta Sí.</param>
    /// <param name="idNo">Regla a la que lleva la respuesta No.</param>
    public Regla(string id, string texto, Comprobacion chequeo, string idSi, string idNo)
    {
        Id = id;
        Texto = texto;
        Chequeo = chequeo;
        IdSi = idSi;
        IdNo = idNo;
    }

    /// <summary>
    /// Crea una fila de apoyo: no tiene pregunta ni salidas.
    /// </summary>
    /// <param name="id">Identificador del apoyo.</param>
    /// <param name="texto">Descripción del apoyo que se asigna.</param>
    /// <returns>La fila de apoyo.</returns>
    public static Regla Apoyo(string id, string texto)
    {
        return new Regla(id, texto, Comprobacion.Ninguna, "", "");
    }

    /// <summary>
    /// Indica si la regla es una pregunta (tiene dos salidas) o un apoyo.
    /// </summary>
    public bool EsPregunta
    {
        get
        {
            return Chequeo != Comprobacion.Ninguna;
        }
    }

    /// <summary>
    /// Devuelve el identificador, que es la etiqueta que se dibuja en el árbol.
    /// </summary>
    /// <returns>El identificador de la regla.</returns>
    public override string ToString()
    {
        return Id;
    }
}
