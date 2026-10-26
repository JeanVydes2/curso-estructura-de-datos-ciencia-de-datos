using TriajeUrgencias;
using TriajeUrgencias.Modelos;
using Xunit;

namespace TriajeUrgenciasTests;

public class TriajeUrgenciasTests
{
    /// <summary>
    /// Crea una cola vacía con el comparador del caso.
    /// </summary>
    private static MonticuloBinario<Paciente> NuevaSala()
    {
        return new MonticuloBinario<Paciente>(new ComparadorDeTriaje());
    }

    /// <summary>
    /// Obtiene los identificadores de una lista de pacientes.
    /// </summary>
    private static List<string> Ids(List<Paciente> pacientes)
    {
        List<string> ids = new List<string>();
        foreach (Paciente paciente in pacientes)
        {
            ids.Add(paciente.Id);
        }

        return ids;
    }

    /// <summary>
    /// Procesa los primeros eventos de la mañana, comprueba el invariante después de cada uno y devuelve los pacientes atendidos.
    /// </summary>
    private static List<Paciente> Procesar(MonticuloBinario<Paciente> sala, int cantidadDeEventos)
    {
        List<Paciente> atendidos = new List<Paciente>();
        List<EventoDeSala> eventos = DatosDelCaso.EventosDeLaManana();
        for (int indice = 0; indice < cantidadDeEventos; indice++)
        {
            EventoDeSala evento = eventos[indice];
            if (evento.PacienteQueLlega != null)
            {
                sala.Insertar(evento.PacienteQueLlega);
            }
            else
            {
                atendidos.Add(sala.ExtraerRaiz());
            }

            Assert.True(sala.EsValido());
        }

        return atendidos;
    }

    /// <summary>
    /// Después de las cuatro admisiones, la lista coincide con la traza.
    /// </summary>
    [Fact]
    public void CuatroAdmisiones_ConstruyenLaListaEsperada()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        Procesar(sala, 4);

        Assert.Equal(new List<string> { "P02", "P01", "P03", "P04" }, Ids(sala.Copia()));
    }

    /// <summary>
    /// Con la misma prioridad que su padre, P04 no sube porque llegó después.
    /// </summary>
    [Fact]
    public void EmpateConElPadre_NoSubeAlQueLlegoDespues()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        Procesar(sala, 4);

        // P04 llega con prioridad 2, igual que su padre P01, pero llegó después: no sube.
        Assert.Equal(0, sala.UltimosIntercambios);
        Assert.Equal("P04", sala.Copia()[3].Id);
    }

    /// <summary>
    /// Las cinco llamadas de la mañana siguen la prioridad y el desempate por llegada.
    /// </summary>
    [Fact]
    public void EventosDeLaManana_LlamanEnOrdenDePrioridadConDesempate()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        List<Paciente> atendidos = Procesar(sala, DatosDelCaso.EventosDeLaManana().Count);

        Assert.Equal(new List<string> { "P02", "P01", "P06", "P04", "P05" }, Ids(atendidos));
        Assert.Equal(new List<string> { "P03" }, Ids(sala.Copia()));
    }

    /// <summary>
    /// El desempate decide la llamada del evento 6: entre P01 y P04, ambos de prioridad 2, gana el que llegó antes.
    /// </summary>
    [Fact]
    public void Desempate_EnElEvento6GanaElQueLlegoAntes()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        List<Paciente> atendidos = Procesar(sala, 6);

        Assert.Equal(new List<string> { "P02", "P01" }, Ids(atendidos));
    }

    /// <summary>
    /// Al vaciar la sala se obtiene el orden completo de atención.
    /// </summary>
    [Fact]
    public void VaciarSala_ProduceElOrdenCompleto()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        List<Paciente> atendidos = Procesar(sala, DatosDelCaso.EventosDeLaManana().Count);
        while (!sala.EstaVacio)
        {
            atendidos.Add(sala.ExtraerRaiz());
        }

        Assert.Equal(new List<string> { "P02", "P01", "P06", "P04", "P05", "P03" }, Ids(atendidos));
    }

    /// <summary>
    /// VerRaiz devuelve la raíz sin modificar la lista.
    /// </summary>
    [Fact]
    public void VerRaiz_NoCambiaLaCola()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        Procesar(sala, 8);
        List<string> antes = Ids(sala.Copia());

        Assert.Equal("P06", sala.VerRaiz().Id);
        Assert.Equal(antes, Ids(sala.Copia()));
    }

    /// <summary>
    /// Consultar o sacar de una sala vacía lanza una excepción.
    /// </summary>
    [Fact]
    public void SalaVacia_LanzaAlConsultarYSacar()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();

        Assert.Throws<InvalidOperationException>(() => sala.VerRaiz());
        Assert.Throws<InvalidOperationException>(() => sala.ExtraerRaiz());
        Assert.Equal(-1, sala.Altura);
    }

    /// <summary>
    /// Con un solo paciente, llamarlo deja la sala vacía.
    /// </summary>
    [Fact]
    public void UnSoloPaciente_SacarLoDejaVacia()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        sala.Insertar(new Paciente("P99", "prueba", 1, 1));

        Assert.Equal("P99", sala.ExtraerRaiz().Id);
        Assert.True(sala.EstaVacio);
    }

    /// <summary>
    /// El dibujo de la cola empieza con la raíz y tiene las líneas esperadas para tres niveles.
    /// </summary>
    [Fact]
    public void Impresor_DibujaLaRaizEnLaPrimeraLinea()
    {
        MonticuloBinario<Paciente> sala = NuevaSala();
        Procesar(sala, 8);

        List<string> lineas = ImpresorMonticulo.RenderizarArbol(sala.Copia(), -1);

        Assert.Contains("[P06:1]", lineas[0]);
        Assert.Equal(5, lineas.Count);
    }
}
