using System.Globalization;
using System.Text;
using ClasificacionBecas.Modelos;

namespace ClasificacionBecas;

/// <summary>
/// Menú y modo de demostración del caso de clasificación de solicitudes de beca.
/// </summary>
public class Program
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-CO");
    private static bool pausasActivas = true;

    /// <summary>
    /// Construye el árbol con los datos del caso, lo dibuja y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> ejecuta todas las opciones seguidas, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<Regla> reglas = DatosDelCaso.Reglas();
        List<Solicitud> solicitudes = DatosDelCaso.Solicitudes();
        ArbolBinario<Regla> arbol = ConstructorDeArbol.Construir(reglas);

        EscribirTitulo("Árbol de decisión de las becas");
        Console.WriteLine("Se construyó el árbol con las " + reglas.Count + " filas de la tabla de reglas.");
        if (arbol.Raiz != null)
        {
            Console.WriteLine("Raíz encontrada: " + arbol.Raiz.Valor.Id + " (la única regla que no es salida de otra).");
        }

        MostrarArbol(arbol);

        // --demo ejecuta todas las opciones seguidas, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            MostrarTablaDeReglas(arbol);
            MostrarMetricas(arbol);
            MostrarRecorridos(arbol);
            foreach (Solicitud solicitud in solicitudes)
            {
                ClasificarPasoAPaso(arbol, solicitud);
            }

            MostrarCondicionesHacia(arbol, "D5");
            return;
        }

        EjecutarMenu(arbol, solicitudes);
    }

    /// <summary>
    /// Muestra el menú y ejecuta la opción elegida hasta que se escoja 0 o termine la entrada.
    /// </summary>
    private static void EjecutarMenu(ArbolBinario<Regla> arbol, List<Solicitud> solicitudes)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Ver el árbol");
            Console.WriteLine("2. Ver la tabla de reglas");
            Console.WriteLine("3. Métricas del árbol");
            Console.WriteLine("4. Recorridos");
            Console.WriteLine("5. Clasificar una solicitud paso a paso");
            Console.WriteLine("6. Condiciones para llegar a un apoyo");
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
                    MostrarArbol(arbol);
                    break;
                case "2":
                    MostrarTablaDeReglas(arbol);
                    break;
                case "3":
                    MostrarMetricas(arbol);
                    break;
                case "4":
                    MostrarRecorridos(arbol);
                    break;
                case "5":
                    Solicitud? solicitud = PedirSolicitud(solicitudes);
                    if (solicitud != null)
                    {
                        ClasificarPasoAPaso(arbol, solicitud);
                    }

                    break;
                case "6":
                    Console.Write("Apoyo (D1 a D6): ");
                    string? id = Console.ReadLine();
                    if (id != null)
                    {
                        MostrarCondicionesHacia(arbol, id.Trim().ToUpperInvariant());
                    }

                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Dibuja el árbol completo y la leyenda de las marcas.
    /// </summary>
    private static void MostrarArbol(ArbolBinario<Regla> arbol)
    {
        Console.WriteLine();
        ImpresorConsola<Regla>.Imprimir(arbol.Raiz);
        Console.WriteLine("Sí → hijo izquierdo · No → hijo derecho · R = pregunta · D = apoyo · (X) recorrido · <X> actual");
    }

    /// <summary>
    /// Lista las reglas en preorden, como se documentan para auditoría.
    /// </summary>
    private static void MostrarTablaDeReglas(ArbolBinario<Regla> arbol)
    {
        EscribirTitulo("Tabla de reglas (en preorden)");
        foreach (NodoBinario<Regla> nodo in arbol.Preorden())
        {
            Regla regla = nodo.Valor;
            if (regla.EsPregunta)
            {
                Console.WriteLine(regla.Id + "  " + regla.Texto + "  Sí → " + regla.IdSi + " · No → " + regla.IdNo);
            }
            else
            {
                Console.WriteLine(regla.Id + "  " + regla.Texto);
            }
        }
    }

    /// <summary>
    /// Muestra nodos, aristas, hojas, nodos internos, altura y si el árbol es lleno.
    /// </summary>
    private static void MostrarMetricas(ArbolBinario<Regla> arbol)
    {
        int nodos = arbol.ContarNodos();
        int hojas = arbol.ContarHojas();
        int internos = nodos - hojas;

        EscribirTitulo("Métricas del árbol");
        Console.WriteLine("Nodos: " + nodos + " · aristas: " + (nodos - 1));
        Console.WriteLine("Preguntas (nodos internos): " + internos + " · apoyos (hojas): " + hojas);
        Console.WriteLine("Altura: " + arbol.Altura() + " → ninguna solicitud responde más de " + arbol.Altura() + " preguntas.");
        Console.WriteLine("¿Lleno? " + (arbol.EsLleno() ? "sí" : "no") + " · hojas = internos + 1: " + hojas + " = " + internos + " + 1");
    }

    /// <summary>
    /// Muestra los recorridos en preorden, inorden, postorden y por niveles.
    /// </summary>
    private static void MostrarRecorridos(ArbolBinario<Regla> arbol)
    {
        EscribirTitulo("Recorridos");
        Console.WriteLine("Preorden   " + Unir(arbol.Preorden()) + "   (documentar las reglas de arriba hacia abajo)");
        Console.WriteLine("Inorden    " + Unir(arbol.Inorden()) + "   (orden de izquierda a derecha del dibujo)");
        Console.WriteLine("Postorden  " + Unir(arbol.Postorden()) + "   (cada pregunta aparece después de sus dos salidas)");

        List<List<NodoBinario<Regla>>> niveles = arbol.Niveles();
        Console.WriteLine("Por niveles:");
        for (int profundidad = 0; profundidad < niveles.Count; profundidad++)
        {
            Console.WriteLine("  nivel " + profundidad + ": " + Unir(niveles[profundidad]) + "  (" + niveles[profundidad].Count + ")");
        }
    }

    /// <summary>
    /// Clasifica una solicitud y dibuja el árbol en cada pregunta, con el camino recorrido y la respuesta.
    /// </summary>
    private static void ClasificarPasoAPaso(ArbolBinario<Regla> arbol, Solicitud solicitud)
    {
        EscribirTitulo("Clasificar la solicitud " + solicitud.Id + " (" + solicitud.Estudiante + ")");
        Console.WriteLine("Promedio: " + Decimal(solicitud.Promedio) + " · créditos aprobados: " + Decimal(solicitud.PorcentajeCreditos)
            + " % · estrato: " + solicitud.Estrato);
        Console.WriteLine("Sisbén: " + SiNo(solicitud.EnSisben) + " · llegó antes del cierre: " + SiNo(solicitud.LlegoAntesDelCierre));

        List<NodoBinario<Regla>> camino = ClasificadorBecas.Clasificar(arbol, solicitud);
        HashSet<NodoBinario<Regla>> visitados = new HashSet<NodoBinario<Regla>>();

        for (int paso = 0; paso < camino.Count; paso++)
        {
            NodoBinario<Regla> nodo = camino[paso];
            visitados.Add(nodo);
            Console.WriteLine();
            ImpresorConsola<Regla>.Imprimir(arbol.Raiz, visitados, nodo);

            Regla regla = nodo.Valor;
            if (regla.EsPregunta)
            {
                bool respuesta = solicitud.Responde(regla.Chequeo);
                string siguiente = respuesta ? regla.IdSi : regla.IdNo;
                Console.WriteLine("Paso " + (paso + 1) + ": " + regla.Id + " " + regla.Texto);
                Console.WriteLine("        " + Evidencia(solicitud, regla.Chequeo) + " → "
                    + (respuesta ? "Sí: baja por la izquierda a " : "No: baja por la derecha a ") + siguiente);
                Pausar();
            }
            else
            {
                Console.WriteLine("Apoyo " + regla.Id + ": " + regla.Texto + ". Se respondieron " + (camino.Count - 1) + " preguntas.");
            }
        }
    }

    /// <summary>
    /// Muestra el camino hacia un apoyo y la respuesta que exige cada pregunta de ese camino.
    /// </summary>
    private static void MostrarCondicionesHacia(ArbolBinario<Regla> arbol, string id)
    {
        NodoBinario<Regla>? objetivo = ConstructorDeArbol.BuscarPorId(arbol, id);
        if (objetivo == null || objetivo.Valor.EsPregunta)
        {
            Console.WriteLine(id + " no es un apoyo del árbol.");
            return;
        }

        List<NodoBinario<Regla>> camino = arbol.CaminoHacia(objetivo);
        EscribirTitulo("Condiciones para llegar a " + id + ": " + objetivo.Valor.Texto);
        ImpresorConsola<Regla>.Imprimir(arbol.Raiz, new HashSet<NodoBinario<Regla>>(camino), objetivo);

        for (int indice = 0; indice < camino.Count - 1; indice++)
        {
            NodoBinario<Regla> pregunta = camino[indice];
            string respuesta = pregunta.Izquierdo == camino[indice + 1] ? "Sí" : "No";
            Console.WriteLine("  " + pregunta.Valor.Id + " " + pregunta.Valor.Texto + " → " + respuesta);
        }
    }

    /// <summary>
    /// Pide al usuario el número de una solicitud del semestre.
    /// </summary>
    /// <returns>La solicitud elegida, o <c>null</c> si el número no es válido.</returns>
    private static Solicitud? PedirSolicitud(List<Solicitud> solicitudes)
    {
        for (int indice = 0; indice < solicitudes.Count; indice++)
        {
            Console.WriteLine("  " + (indice + 1) + ". " + solicitudes[indice].Id + " " + solicitudes[indice].Estudiante);
        }

        Console.Write("Solicitud: ");
        string? texto = Console.ReadLine();
        int numero;
        if (texto != null && int.TryParse(texto.Trim(), out numero) && numero >= 1 && numero <= solicitudes.Count)
        {
            return solicitudes[numero - 1];
        }

        Console.WriteLine("Número no válido.");
        return null;
    }

    /// <summary>
    /// Describe el dato de la solicitud que responde una pregunta, por ejemplo el promedio y su mínimo.
    /// </summary>
    private static string Evidencia(Solicitud solicitud, Comprobacion chequeo)
    {
        switch (chequeo)
        {
            case Comprobacion.PromedioAlcanzaMinimo:
                return "promedio " + Decimal(solicitud.Promedio) + " (mínimo " + Decimal(Solicitud.PromedioMinimo) + ")";
            case Comprobacion.LlegoAntesDelCierre:
                return "llegó antes del cierre: " + SiNo(solicitud.LlegoAntesDelCierre);
            case Comprobacion.CreditosSuficientes:
                return "créditos aprobados " + Decimal(solicitud.PorcentajeCreditos) + " % (mínimo " + Decimal(Solicitud.CreditosMinimos) + " %)";
            case Comprobacion.EstratoBajo:
                return "estrato " + solicitud.Estrato + " (bajo si es " + Solicitud.EstratoMaximoBajo + " o menos)";
            default:
                return "Sisbén: " + SiNo(solicitud.EnSisben);
        }
    }

    /// <summary>
    /// Une los identificadores de una lista de nodos separados por comas.
    /// </summary>
    private static string Unir(List<NodoBinario<Regla>> nodos)
    {
        List<string> ids = new List<string>();
        foreach (NodoBinario<Regla> nodo in nodos)
        {
            ids.Add(nodo.Valor.Id);
        }

        return string.Join(", ", ids);
    }

    /// <summary>
    /// Da formato decimal con coma, como se escribe en Colombia.
    /// </summary>
    private static string Decimal(double valor)
    {
        return valor.ToString("0.0", Espanol);
    }

    /// <summary>
    /// Convierte un valor lógico en "sí" o "no".
    /// </summary>
    private static string SiNo(bool valor)
    {
        return valor ? "sí" : "no";
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
