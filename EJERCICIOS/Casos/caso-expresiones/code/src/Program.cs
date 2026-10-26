using System.Text;

namespace TarifasExpresion;

/// <summary>
/// Menú y modo de demostración del caso de árboles de expresión para liquidar tarifas.
/// </summary>
public class Program
{
    private static bool pausasActivas = true;

    /// <summary>
    /// Prepara los convenios del caso y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> recorre todos los convenios, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        EscribirTitulo("Árboles de expresión de las tarifas");
        Console.WriteLine("Los convenios están en notación sufija: los operandos primero y el operador después.");

        // --demo recorre todos los convenios y las notaciones inválidas, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            foreach (Convenio convenio in DatosDelCaso.Convenios())
            {
                AnalizarConvenio(convenio);
            }

            MostrarNotacionesInvalidas();
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
            Console.WriteLine("1. Analizar un convenio (cuatro escrituras y valor)");
            Console.WriteLine("2. Analizar todos los convenios");
            Console.WriteLine("3. Evaluar la fórmula de un convenio");
            Console.WriteLine("4. Métricas de la fórmula de un convenio");
            Console.WriteLine("5. Ver una notación mal formada");
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
                    Convenio? uno = PedirConvenio();
                    if (uno != null)
                    {
                        AnalizarConvenio(uno);
                    }

                    break;
                case "2":
                    foreach (Convenio convenio in DatosDelCaso.Convenios())
                    {
                        AnalizarConvenio(convenio);
                    }

                    break;
                case "3":
                    Convenio? paraValor = PedirConvenio();
                    if (paraValor != null)
                    {
                        EvaluarConvenio(paraValor);
                    }

                    break;
                case "4":
                    Convenio? paraMetricas = PedirConvenio();
                    if (paraMetricas != null)
                    {
                        MostrarMetricas(paraMetricas);
                    }

                    break;
                case "5":
                    MostrarNotacionesInvalidas();
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Construye la fórmula de un convenio y muestra sus cuatro escrituras, su valor y sus métricas.
    /// </summary>
    private static void AnalizarConvenio(Convenio convenio)
    {
        NodoExpresion raiz = ConstructorDeExpresion.Construir(convenio.NotacionSufija);
        double valor = ArbolDeExpresion.Evaluar(raiz);

        EscribirTitulo("Convenio: " + convenio.Nombre);
        Console.WriteLine("Notación sufija: " + convenio.NotacionSufija);
        Console.WriteLine();
        ImpresorExpresion.Imprimir(raiz);
        Console.WriteLine("Prefijo (preorden):  " + ArbolDeExpresion.Prefijo(raiz));
        Console.WriteLine("Infijo (inorden):    " + ArbolDeExpresion.Infijo(raiz));
        Console.WriteLine("Sufijo (postorden):  " + ArbolDeExpresion.Sufijo(raiz));
        Console.WriteLine("Valor: " + ArbolDeExpresion.Formato(valor) + " (esperado " + ArbolDeExpresion.Formato(convenio.ValorEsperado) + ")");
        Console.WriteLine("Nodos: " + ArbolDeExpresion.ContarNodos(raiz)
            + " · hojas (números): " + ArbolDeExpresion.ContarHojas(raiz)
            + " · operadores: " + ArbolDeExpresion.ContarOperadores(raiz)
            + " · altura: " + ArbolDeExpresion.Altura(raiz)
            + " · ancho: " + ArbolDeExpresion.Ancho(raiz));
        Pausar();
    }

    /// <summary>
    /// Muestra el valor de la fórmula de un convenio y el valor esperado.
    /// </summary>
    private static void EvaluarConvenio(Convenio convenio)
    {
        NodoExpresion raiz = ConstructorDeExpresion.Construir(convenio.NotacionSufija);

        EscribirTitulo("Tarifa del convenio " + convenio.Nombre);
        Console.WriteLine("Fórmula: " + convenio.NotacionSufija);
        Console.WriteLine("Tarifa: " + ArbolDeExpresion.Formato(ArbolDeExpresion.Evaluar(raiz))
            + " · esperada: " + ArbolDeExpresion.Formato(convenio.ValorEsperado));
        Pausar();
    }

    /// <summary>
    /// Muestra las métricas de forma de la fórmula de un convenio.
    /// </summary>
    private static void MostrarMetricas(Convenio convenio)
    {
        NodoExpresion raiz = ConstructorDeExpresion.Construir(convenio.NotacionSufija);

        EscribirTitulo("Métricas de la fórmula del convenio " + convenio.Nombre);
        Console.WriteLine("Nodos: " + ArbolDeExpresion.ContarNodos(raiz)
            + " · hojas (números): " + ArbolDeExpresion.ContarHojas(raiz)
            + " · operadores: " + ArbolDeExpresion.ContarOperadores(raiz));
        Console.WriteLine("Altura: " + ArbolDeExpresion.Altura(raiz) + " · ancho (nivel más poblado): " + ArbolDeExpresion.Ancho(raiz));

        List<List<NodoExpresion>> niveles = ArbolDeExpresion.Niveles(raiz);
        Console.WriteLine("Por niveles:");
        for (int profundidad = 0; profundidad < niveles.Count; profundidad++)
        {
            List<string> textos = new List<string>();
            foreach (NodoExpresion nodo in niveles[profundidad])
            {
                textos.Add(nodo.Texto);
            }

            Console.WriteLine("  nivel " + profundidad + ": " + string.Join(", ", textos) + "  (" + niveles[profundidad].Count + ")");
        }

        Pausar();
    }

    /// <summary>
    /// Muestra qué error produce cada notación mal formada.
    /// </summary>
    private static void MostrarNotacionesInvalidas()
    {
        EscribirTitulo("Notaciones mal formadas");
        foreach ((string notacion, string explicacion) in DatosDelCaso.NotacionesInvalidas())
        {
            try
            {
                ConstructorDeExpresion.Construir(notacion);
                Console.WriteLine(notacion + " → no se detectó el error.");
            }
            catch (InvalidOperationException error)
            {
                Console.WriteLine(notacion + " → " + error.Message + " (" + explicacion + ")");
            }
        }
    }

    /// <summary>
    /// Pide al usuario el número de un convenio.
    /// </summary>
    /// <returns>El convenio elegido, o <c>null</c> si el número no es válido.</returns>
    private static Convenio? PedirConvenio()
    {
        List<Convenio> convenios = DatosDelCaso.Convenios();
        for (int indice = 0; indice < convenios.Count; indice++)
        {
            Console.WriteLine("  " + (indice + 1) + ". " + convenios[indice].Nombre);
        }

        Console.Write("Convenio: ");
        string? texto = Console.ReadLine();
        int numero;
        if (texto != null && int.TryParse(texto.Trim(), out numero) && numero >= 1 && numero <= convenios.Count)
        {
            return convenios[numero - 1];
        }

        Console.WriteLine("Número no válido.");
        return null;
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
