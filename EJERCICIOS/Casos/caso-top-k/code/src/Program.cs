using System.Text;
using LluviaTopK.Modelos;

namespace LluviaTopK;

/// <summary>
/// Menú y modo de demostración del caso del Top-K de pluviómetros.
/// </summary>
public class Program
{
    private static readonly ComparadorPorMilimetros Comparador = new ComparadorPorMilimetros();
    private static bool pausasActivas = true;

    /// <summary>
    /// Prepara el lote y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> recorre el lote y muestra los tres resultados, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        EscribirTitulo("Boletín de lluvia: los K puntos con más acumulado");
        Console.WriteLine("El lote de prueba tiene " + DatosDelCaso.Lote().Count
            + " lecturas; el archivo real del mes tiene 6 000 000.");

        // --demo recorre el lote y muestra los resultados del caso, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            RecorridoPasoAPaso(DatosDelCaso.Lote(), DatosDelCaso.K());
            MostrarTopK(DatosDelCaso.K());
            MostrarBottomK(DatosDelCaso.K());
            CompararConstruccion();
            return;
        }

        EjecutarMenu();
    }

    /// <summary>
    /// Muestra el menú y ejecuta la opción elegida hasta que se escoja 0 o termine la entrada.
    /// </summary>
    private static void EjecutarMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Recorrer el lote paso a paso (K = 3)");
            Console.WriteLine("2. Ver el Top-K del lote");
            Console.WriteLine("3. Ver el Bottom-K del lote");
            Console.WriteLine("4. Comparar construcción por Floyd contra inserción");
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
                    RecorridoPasoAPaso(DatosDelCaso.Lote(), DatosDelCaso.K());
                    break;
                case "2":
                    MostrarTopK(DatosDelCaso.K());
                    break;
                case "3":
                    MostrarBottomK(DatosDelCaso.K());
                    break;
                case "4":
                    CompararConstruccion();
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Recorre el lote con un montículo de mínimos de tamaño K y muestra el estado tras cada lectura.
    /// </summary>
    private static void RecorridoPasoAPaso(List<Acumulado> lote, int k)
    {
        EscribirTitulo("Recorrido del lote con K = " + k);
        Console.WriteLine("Montículo de mínimos: la raíz es el menor de los K y es el que sale cuando llega una lectura mayor.");

        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        List<string> salidas = new List<string>();

        foreach (Acumulado lectura in lote)
        {
            if (monticulo.Cantidad < k)
            {
                monticulo.Insertar(lectura);
                Console.WriteLine();
                Console.WriteLine("Llega " + lectura + " · entran menos de " + k + " · intercambios: " + monticulo.UltimosIntercambios);
            }
            else if (Comparador.Compare(lectura, monticulo.VerRaiz()) > 0)
            {
                Acumulado sale = monticulo.VerRaiz();
                monticulo.ExtraerRaiz();
                monticulo.Insertar(lectura);
                salidas.Add(sale.Id + ":" + sale.Milimetros.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("es-CO")));
                Console.WriteLine();
                Console.WriteLine("Llega " + lectura + " · es mayor que la raíz " + sale + " · sale " + sale.Id
                    + " · intercambios: " + monticulo.UltimosIntercambios);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Llega " + lectura + " · no supera la raíz " + monticulo.VerRaiz() + " · no cambia el montículo.");
                Pausar();
                continue;
            }

            ImpresorMonticulo.Imprimir(monticulo.Copia(), -1);
            Console.WriteLine("  Cumple el invariante: " + (monticulo.EsValido() ? "sí" : "no")
                + " · altura: " + monticulo.Altura);
            Pausar();
        }

        EscribirTitulo("Salieron del montículo (no quedaron entre los K mayores)");
        Console.WriteLine(salidas.Count == 0 ? "(ninguno)" : string.Join(", ", salidas));
    }

    /// <summary>
    /// Muestra los K puntos con más lluvia, de mayor a menor.
    /// </summary>
    private static void MostrarTopK(int k)
    {
        EscribirTitulo("Top " + k + " del lote (más lluvia)");
        foreach (Acumulado acumulado in SeleccionTopK.TopK(DatosDelCaso.Lote(), k))
        {
            Console.WriteLine("  " + acumulado.Id + "  " + acumulado.Milimetros.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("es-CO")) + " mm");
        }

        Pausar();
    }

    /// <summary>
    /// Muestra los K puntos con menos lluvia, de menor a mayor.
    /// </summary>
    private static void MostrarBottomK(int k)
    {
        EscribirTitulo("Bottom " + k + " del lote (menos lluvia)");
        foreach (Acumulado acumulado in SeleccionTopK.BottomK(DatosDelCaso.Lote(), k))
        {
            Console.WriteLine("  " + acumulado.Id + "  " + acumulado.Milimetros.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("es-CO")) + " mm");
        }

        Pausar();
    }

    /// <summary>
    /// Compara construir un montículo por Floyd contra insertar los elementos uno por uno.
    /// </summary>
    private static void CompararConstruccion()
    {
        List<Acumulado> lote = DatosDelCaso.Lote();

        MonticuloBinario<Acumulado> porInsercion = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        int intercambiosInsercion = 0;
        foreach (Acumulado acumulado in lote)
        {
            porInsercion.Insertar(acumulado);
            intercambiosInsercion += porInsercion.UltimosIntercambios;
        }

        List<Acumulado> copiaDelLote = new List<Acumulado>(lote);
        MonticuloBinario<Acumulado> porFloyd = new MonticuloBinario<Acumulado>(new ComparadorPorMilimetros());
        int intercambiosFloyd = porFloyd.ConstruirDesde(copiaDelLote);

        EscribirTitulo("Construir el montículo de " + lote.Count + " elementos");
        Console.WriteLine("Insertando uno por uno: " + intercambiosInsercion + " intercambios.");
        Console.WriteLine("Por Floyd (desde el último nodo interno): " + intercambiosFloyd + " intercambios.");
        Console.WriteLine("Raíz con inserción: " + porInsercion.VerRaiz().Id + " · raíz con Floyd: " + porFloyd.VerRaiz().Id);
        Console.WriteLine("La lista interna no es única: varios arreglos representan el mismo montículo.");
        Pausar();
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
    /// Espera Enter entre pasos, salvo en modo --demo o si la entrada viene redirigida.
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
