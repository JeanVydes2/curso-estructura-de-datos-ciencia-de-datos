using System.Text;
using TriajeUrgencias.Modelos;

namespace TriajeUrgencias;

/// <summary>
/// Menú y modo de demostración del caso del triaje de la sala de urgencias.
/// </summary>
public class Program
{
    private static bool pausasActivas = true;

    /// <summary>
    /// Prepara la sala y los eventos de la mañana y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> procesa todos los eventos y vacía la sala, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        MonticuloBinario<Paciente> sala = new MonticuloBinario<Paciente>(new ComparadorDeTriaje());
        List<EventoDeSala> eventos = DatosDelCaso.EventosDeLaManana();
        List<Paciente> atendidos = new List<Paciente>();
        int siguienteEvento = 0;

        EscribirTitulo("Sala de urgencias: cola de triaje");
        Console.WriteLine("La mañana tiene " + eventos.Count + " eventos. La raíz de la cola es el paciente más urgente.");

        // --demo procesa todos los eventos y luego vacía la sala, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            while (siguienteEvento < eventos.Count)
            {
                ProcesarEvento(sala, eventos[siguienteEvento], atendidos);
                siguienteEvento++;
            }

            VaciarSala(sala, atendidos);
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Ver la sala (árbol y lista)");
            Console.WriteLine("2. Procesar el siguiente evento (" + (eventos.Count - siguienteEvento) + " pendientes)");
            Console.WriteLine("3. Procesar todos los eventos restantes");
            Console.WriteLine("4. Admitir un paciente");
            Console.WriteLine("5. Llamar al siguiente paciente");
            Console.WriteLine("6. Consultar el próximo paciente sin llamarlo");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");

            string? opcion = Console.ReadLine();
            if (opcion == null || opcion.Trim() == "0")
            {
                return;
            }

            switch (opcion.Trim())
            {
                case "1":
                    MostrarSala(sala, -1);
                    break;
                case "2":
                    if (siguienteEvento < eventos.Count)
                    {
                        ProcesarEvento(sala, eventos[siguienteEvento], atendidos);
                        siguienteEvento++;
                    }
                    else
                    {
                        Console.WriteLine("Ya se procesaron todos los eventos de la mañana.");
                    }

                    break;
                case "3":
                    while (siguienteEvento < eventos.Count)
                    {
                        ProcesarEvento(sala, eventos[siguienteEvento], atendidos);
                        siguienteEvento++;
                    }

                    break;
                case "4":
                    Paciente? paciente = PedirPaciente(sala.Cantidad + atendidos.Count + 1);
                    if (paciente != null)
                    {
                        Llegar(sala, paciente, "ahora");
                    }

                    break;
                case "5":
                    Llamar(sala, atendidos, "ahora");
                    break;
                case "6":
                    if (sala.EstaVacio)
                    {
                        Console.WriteLine("La sala está vacía.");
                    }
                    else
                    {
                        Paciente proximo = sala.VerRaiz();
                        Console.WriteLine("Próximo: " + proximo.Id + " " + proximo.Motivo + " (prioridad " + proximo.Prioridad
                            + "). VerRaiz lee la posición 0 y no cambia la cola.");
                    }

                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Procesa un evento de la mañana: admite al paciente que llega o llama al siguiente.
    /// </summary>
    private static void ProcesarEvento(MonticuloBinario<Paciente> sala, EventoDeSala evento, List<Paciente> atendidos)
    {
        if (evento.PacienteQueLlega != null)
        {
            Llegar(sala, evento.PacienteQueLlega, evento.Hora);
        }
        else
        {
            Llamar(sala, atendidos, evento.Hora);
        }

        Pausar();
    }

    /// <summary>
    /// Admite un paciente y dibuja la cola resaltando dónde quedó.
    /// </summary>
    private static void Llegar(MonticuloBinario<Paciente> sala, Paciente paciente, string hora)
    {
        EscribirTitulo(hora + " · llega " + paciente.Id + " " + paciente.Motivo + " (prioridad " + paciente.Prioridad + ")");
        sala.Insertar(paciente);
        Console.WriteLine("Insertar: entra al final de la lista y sube mientras vaya antes que su padre. Intercambios: "
            + sala.UltimosIntercambios + ".");
        MostrarSala(sala, IndiceDe(sala, paciente));
    }

    /// <summary>
    /// Llama al paciente de la raíz para un médico libre y dibuja la cola resaltando quién bajó.
    /// </summary>
    private static void Llamar(MonticuloBinario<Paciente> sala, List<Paciente> atendidos, string hora)
    {
        if (sala.EstaVacio)
        {
            EscribirTitulo(hora + " · médico libre");
            Console.WriteLine("No hay pacientes en espera.");
            return;
        }

        List<Paciente> antes = sala.Copia();
        Paciente bajoALaRaiz = antes[antes.Count - 1];
        Paciente paciente = sala.ExtraerRaiz();
        atendidos.Add(paciente);

        EscribirTitulo(hora + " · médico libre → se llama a " + paciente.Id + " " + paciente.Motivo
            + " (prioridad " + paciente.Prioridad + ")");
        if (sala.EstaVacio)
        {
            Console.WriteLine("Era el único paciente: la sala queda vacía.");
        }
        else
        {
            Console.WriteLine("ExtraerRaiz: " + bajoALaRaiz.Id + " (el último de la lista) pasa a la raíz y baja por el hijo que va primero. Intercambios: "
                + sala.UltimosIntercambios + ".");
        }

        MostrarSala(sala, IndiceDe(sala, bajoALaRaiz));
        Console.WriteLine("Atendidos hasta ahora: " + UnirIds(atendidos));
    }

    /// <summary>
    /// Llama a todos los pacientes que quedan y muestra el orden completo de atención.
    /// </summary>
    private static void VaciarSala(MonticuloBinario<Paciente> sala, List<Paciente> atendidos)
    {
        EscribirTitulo("Fin de la mañana: se atiende lo que queda");
        while (!sala.EstaVacio)
        {
            atendidos.Add(sala.ExtraerRaiz());
        }

        Console.WriteLine("Orden completo de atención: " + UnirIds(atendidos));
    }

    /// <summary>
    /// Dibuja la cola como árbol y como lista, y muestra cantidad, altura e invariante.
    /// </summary>
    private static void MostrarSala(MonticuloBinario<Paciente> sala, int indiceResaltado)
    {
        Console.WriteLine();
        ImpresorMonticulo.Imprimir(sala.Copia(), indiceResaltado);
        Console.WriteLine("Pacientes: " + sala.Cantidad + " · altura: " + sala.Altura + " · ¿cumple el invariante? "
            + (sala.EsValido() ? "sí" : "no"));
    }

    /// <summary>
    /// Busca la posición de un paciente en la lista de la cola.
    /// </summary>
    /// <returns>La posición, o -1 si no está.</returns>
    private static int IndiceDe(MonticuloBinario<Paciente> sala, Paciente paciente)
    {
        List<Paciente> elementos = sala.Copia();
        for (int indice = 0; indice < elementos.Count; indice++)
        {
            if (elementos[indice] == paciente)
            {
                return indice;
            }
        }

        return -1;
    }

    /// <summary>
    /// Pide al usuario los datos de un paciente nuevo.
    /// </summary>
    /// <returns>El paciente, o <c>null</c> si los datos no son válidos.</returns>
    private static Paciente? PedirPaciente(int ordenDeLlegada)
    {
        Console.Write("Identificador: ");
        string? id = Console.ReadLine();
        Console.Write("Motivo: ");
        string? motivo = Console.ReadLine();
        Console.Write("Prioridad (1 a 5): ");
        string? textoPrioridad = Console.ReadLine();

        int prioridad;
        if (id == null || motivo == null || textoPrioridad == null || !int.TryParse(textoPrioridad.Trim(), out prioridad))
        {
            Console.WriteLine("Datos no válidos.");
            return null;
        }

        return new Paciente(id.Trim(), motivo.Trim(), prioridad, ordenDeLlegada);
    }

    /// <summary>
    /// Une los identificadores de una lista de pacientes separados por comas.
    /// </summary>
    private static string UnirIds(List<Paciente> pacientes)
    {
        List<string> ids = new List<string>();
        foreach (Paciente paciente in pacientes)
        {
            ids.Add(paciente.Id);
        }

        return string.Join(", ", ids);
    }

    /// <summary>
    /// Escribe un título de sección en verde.
    /// </summary>
    private static void EscribirTitulo(string texto)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("== " + texto + " ==");
        Console.ResetColor();
    }

    /// <summary>
    /// Espera Enter entre eventos, salvo en modo --demo o si la entrada viene redirigida.
    /// </summary>
    private static void Pausar()
    {
        if (!pausasActivas || Console.IsInputRedirected)
        {
            return;
        }

        Console.Write("Enter para continuar...");
        Console.ReadLine();
    }
}
