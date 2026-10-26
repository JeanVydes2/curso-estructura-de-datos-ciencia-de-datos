using System.Globalization;
using System.Text;
using CatalogoEstaciones.Modelos;

namespace CatalogoEstaciones;

/// <summary>
/// Menú y modo de demostración del caso del catálogo de estaciones.
/// </summary>
public class Program
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-CO");
    private static bool pausasActivas = true;

    /// <summary>
    /// Inserta las estaciones del caso, dibuja el catálogo y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> ejecuta la secuencia completa del caso, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        ArbolBinarioBusqueda catalogo = new ArbolBinarioBusqueda();
        foreach (Estacion estacion in DatosDelCaso.EstacionesInstaladas())
        {
            catalogo.Insertar(estacion);
        }

        EscribirTitulo("Catálogo de estaciones de la red hidrometeorológica");
        Console.WriteLine("Se insertaron las " + catalogo.Contar() + " estaciones en el orden en que se instalaron.");
        MostrarArbol(catalogo);

        // --demo ejecuta la secuencia completa del caso, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            foreach (int codigo in DatosDelCaso.CodigosDeLecturas())
            {
                ValidarLectura(catalogo, codigo);
            }

            MostrarMinimoYMaximo(catalogo);
            RegistrarEstacion(catalogo, DatosDelCaso.EstacionNueva());
            foreach (int codigo in DatosDelCaso.CodigosQueSeRetiran())
            {
                RetirarEstacion(catalogo, codigo);
            }

            MostrarRango(catalogo, 200, 299);
            MostrarEnOrden(catalogo);
            CompararCargaOrdenada();
            return;
        }

        EjecutarMenu(catalogo);
    }

    /// <summary>
    /// Muestra el menú y ejecuta la opción elegida hasta que se escoja 0 o termine la entrada.
    /// </summary>
    private static void EjecutarMenu(ArbolBinarioBusqueda catalogo)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Ver el árbol");
            Console.WriteLine("2. Listar estaciones en orden (inorden)");
            Console.WriteLine("3. Validar una lectura (buscar un código)");
            Console.WriteLine("4. Registrar una estación (insertar)");
            Console.WriteLine("5. Retirar una estación (eliminar)");
            Console.WriteLine("6. Reporte por rango de códigos");
            Console.WriteLine("7. Menor y mayor código (Min y Max)");
            Console.WriteLine("8. Comparar con una carga ordenada por código");
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
                    MostrarArbol(catalogo);
                    break;
                case "2":
                    MostrarEnOrden(catalogo);
                    break;
                case "3":
                    int codigo;
                    if (PedirNumero("Código de la lectura: ", out codigo))
                    {
                        ValidarLectura(catalogo, codigo);
                    }

                    break;
                case "4":
                    int codigoNuevo;
                    if (PedirNumero("Código de la estación nueva: ", out codigoNuevo))
                    {
                        string municipio = PedirTexto("Municipio: ");
                        string variable = PedirTexto("Variable (Caudal, Nivel o Precipitación): ");
                        RegistrarEstacion(catalogo, new Estacion(codigoNuevo, municipio, variable, 0));
                    }

                    break;
                case "5":
                    int codigoRetirado;
                    if (PedirNumero("Código de la estación que se retira: ", out codigoRetirado))
                    {
                        RetirarEstacion(catalogo, codigoRetirado);
                    }

                    break;
                case "6":
                    int bajo;
                    int alto;
                    if (PedirNumero("Código inicial: ", out bajo) && PedirNumero("Código final: ", out alto))
                    {
                        MostrarRango(catalogo, bajo, alto);
                    }

                    break;
                case "7":
                    MostrarMinimoYMaximo(catalogo);
                    break;
                case "8":
                    CompararCargaOrdenada();
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Dibuja el árbol y muestra su altura junto a la menor altura posible.
    /// </summary>
    private static void MostrarArbol(ArbolBinarioBusqueda catalogo)
    {
        Console.WriteLine();
        ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz);
        int cantidad = catalogo.Contar();
        Console.WriteLine("Estaciones: " + cantidad + " · altura: " + catalogo.Altura()
            + " · altura mínima posible con " + cantidad + " claves: " + AlturaMinima(cantidad));
    }

    /// <summary>
    /// Valida una lectura: dibuja el árbol en cada comparación de la búsqueda y dice si se acepta o se rechaza.
    /// </summary>
    private static void ValidarLectura(ArbolBinarioBusqueda catalogo, int codigo)
    {
        EscribirTitulo("Validar lectura de la estación " + codigo);
        List<NodoBinario<Estacion>> camino = catalogo.CaminoDeBusqueda(codigo);
        HashSet<NodoBinario<Estacion>> visitados = new HashSet<NodoBinario<Estacion>>();

        for (int paso = 0; paso < camino.Count; paso++)
        {
            NodoBinario<Estacion> nodo = camino[paso];
            visitados.Add(nodo);
            Console.WriteLine();
            ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz, visitados, nodo);

            int codigoDelNodo = nodo.Valor.Codigo;
            if (codigo == codigoDelNodo)
            {
                Console.WriteLine("Comparación " + (paso + 1) + ": " + codigo + " = " + codigoDelNodo + " → encontrado.");
            }
            else if (codigo < codigoDelNodo)
            {
                Console.WriteLine("Comparación " + (paso + 1) + ": " + codigo + " < " + codigoDelNodo + " → izquierda"
                    + (nodo.Izquierdo == null ? ", que es null." : "."));
            }
            else
            {
                Console.WriteLine("Comparación " + (paso + 1) + ": " + codigo + " > " + codigoDelNodo + " → derecha"
                    + (nodo.Derecho == null ? ", que es null." : "."));
            }

            Pausar();
        }

        NodoBinario<Estacion>? encontrado = catalogo.Buscar(codigo);
        if (encontrado != null)
        {
            Estacion estacion = encontrado.Valor;
            Console.WriteLine("Lectura aceptada: " + estacion.Codigo + " · " + estacion.Cuenca + " · " + estacion.Municipio
                + " · " + estacion.Variable + " (" + camino.Count + " comparaciones).");
        }
        else
        {
            Console.WriteLine("Lectura rechazada: la estación " + codigo + " no está registrada (" + camino.Count + " comparaciones).");
        }
    }

    /// <summary>
    /// Muestra el menor y el mayor código del catálogo.
    /// </summary>
    private static void MostrarMinimoYMaximo(ArbolBinarioBusqueda catalogo)
    {
        EscribirTitulo("Menor y mayor código del catálogo");
        Estacion? minimo = catalogo.Minimo();
        Estacion? maximo = catalogo.Maximo();
        Console.WriteLine("Mínimo: " + (minimo == null ? "(catálogo vacío)" : minimo.Codigo + " " + minimo.Municipio + " · " + minimo.Cuenca));
        Console.WriteLine("Máximo: " + (maximo == null ? "(catálogo vacío)" : maximo.Codigo + " " + maximo.Municipio + " · " + maximo.Cuenca));
        Console.WriteLine("Cada uno se obtiene bajando siempre por el mismo lado: O(h).");
    }

    /// <summary>
    /// Inserta una estación y dibuja el camino que siguió hasta quedar como hoja.
    /// </summary>
    private static void RegistrarEstacion(ArbolBinarioBusqueda catalogo, Estacion estacion)
    {
        EscribirTitulo("Registrar la estación " + estacion.Codigo + " (" + estacion.Cuenca + ", " + estacion.Municipio + ")");
        List<NodoBinario<Estacion>> camino = catalogo.CaminoDeBusqueda(estacion.Codigo);

        if (!catalogo.Insertar(estacion))
        {
            Console.WriteLine("El código " + estacion.Codigo + " ya existe: no se registra un duplicado.");
            return;
        }

        NodoBinario<Estacion>? agregado = catalogo.Buscar(estacion.Codigo);
        HashSet<NodoBinario<Estacion>> marcados = new HashSet<NodoBinario<Estacion>>(camino);
        ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz, marcados, agregado);
        Console.WriteLine("Camino: " + UnirCodigos(camino) + " → queda como hoja. Ningún otro nodo se movió.");
        MostrarResumen(catalogo);
    }

    /// <summary>
    /// Retira una estación y dibuja el árbol antes y después, indicando el caso de eliminación.
    /// </summary>
    private static void RetirarEstacion(ArbolBinarioBusqueda catalogo, int codigo)
    {
        string caso = catalogo.CasoDeEliminacion(codigo);
        EscribirTitulo("Retirar la estación " + codigo);
        if (caso == "")
        {
            Console.WriteLine("La estación " + codigo + " no está en el catálogo.");
            return;
        }

        NodoBinario<Estacion>? nodo = catalogo.Buscar(codigo);
        string sucesor = caso == ArbolBinarioBusqueda.CasoDosHijos
            ? " Sucesor inorden: " + catalogo.SucesorInorden(codigo) + "."
            : "";
        Console.WriteLine("Antes. Caso: " + caso + "." + sucesor);
        ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz, new HashSet<NodoBinario<Estacion>>(), nodo);

        string explicacion = Explicar(nodo, caso);
        catalogo.Eliminar(codigo);
        Pausar();

        Console.WriteLine();
        Console.WriteLine("Después. " + explicacion);
        ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz);
        MostrarResumen(catalogo);
    }

    /// <summary>
    /// Explica qué cambia en el árbol según el caso de eliminación.
    /// </summary>
    private static string Explicar(NodoBinario<Estacion>? nodo, string caso)
    {
        if (nodo == null)
        {
            return "";
        }

        if (caso == ArbolBinarioBusqueda.CasoHoja)
        {
            return "Su padre deja de apuntarla.";
        }

        if (caso == ArbolBinarioBusqueda.CasoUnHijo)
        {
            NodoBinario<Estacion>? hijo = nodo.Izquierdo != null ? nodo.Izquierdo : nodo.Derecho;
            return "Su padre apunta ahora a su único hijo" + (hijo != null ? " (" + hijo.Valor.Codigo + ")" : "") + ".";
        }

        NodoBinario<Estacion>? sucesor = nodo.Derecho;
        while (sucesor != null && sucesor.Izquierdo != null)
        {
            sucesor = sucesor.Izquierdo;
        }

        return "El sucesor inorden (" + (sucesor != null ? sucesor.Valor.Codigo.ToString() : "")
            + ", el menor código del subárbol derecho) ocupa su lugar.";
    }

    /// <summary>
    /// Muestra las estaciones de un rango de códigos y los nodos que se visitaron para encontrarlas.
    /// </summary>
    private static void MostrarRango(ArbolBinarioBusqueda catalogo, int bajo, int alto)
    {
        EscribirTitulo("Reporte de estaciones con código entre " + bajo + " y " + alto);
        List<NodoBinario<Estacion>> visitados = new List<NodoBinario<Estacion>>();
        List<Estacion> estaciones = catalogo.Rango(bajo, alto, visitados);

        ImpresorArbol<Estacion>.Imprimir(catalogo.Raiz, new HashSet<NodoBinario<Estacion>>(visitados), null);
        foreach (Estacion estacion in estaciones)
        {
            Console.WriteLine("  " + estacion.Codigo + "  " + estacion.Cuenca + "  " + estacion.Municipio + "  " + estacion.Variable
                + "  caudal medio " + Caudal(estacion));
        }

        Console.WriteLine(estaciones.Count + " estaciones. Se visitaron " + visitados.Count + " de " + catalogo.Contar()
            + " nodos: las ramas que no pueden tener códigos del rango no se recorren.");
    }

    /// <summary>
    /// Lista todas las estaciones en inorden, ordenadas por código.
    /// </summary>
    private static void MostrarEnOrden(ArbolBinarioBusqueda catalogo)
    {
        EscribirTitulo("Estaciones en orden de código (inorden)");
        foreach (Estacion estacion in catalogo.EnOrden())
        {
            Console.WriteLine("  " + estacion.Codigo + "  " + estacion.Cuenca + "  " + estacion.Municipio + "  " + estacion.Variable);
        }
    }

    /// <summary>
    /// Carga las estaciones ordenadas por código y muestra la cadena que resulta.
    /// </summary>
    private static void CompararCargaOrdenada()
    {
        ArbolBinarioBusqueda instaladas = new ArbolBinarioBusqueda();
        foreach (Estacion estacion in DatosDelCaso.EstacionesInstaladas())
        {
            instaladas.Insertar(estacion);
        }

        List<Estacion> ordenadas = DatosDelCaso.EstacionesInstaladas();
        ordenadas.Sort(new ComparadorDeCodigo());

        ArbolBinarioBusqueda cadena = new ArbolBinarioBusqueda();
        foreach (Estacion estacion in ordenadas)
        {
            cadena.Insertar(estacion);
        }

        EscribirTitulo("Las mismas 12 estaciones cargadas en orden de código");
        ImpresorArbol<Estacion>.Imprimir(cadena.Raiz);
        Console.WriteLine("Altura: " + cadena.Altura() + ". Buscar el código mayor exige " + (cadena.Altura() + 1)
            + " comparaciones. Con el orden de instalación, las mismas estaciones forman un árbol de altura "
            + instaladas.Altura() + ".");
    }

    /// <summary>
    /// Muestra la cantidad de estaciones, la altura, el inorden y si se cumple la regla de orden.
    /// </summary>
    private static void MostrarResumen(ArbolBinarioBusqueda catalogo)
    {
        Console.WriteLine("Estaciones: " + catalogo.Contar() + " · altura: " + catalogo.Altura()
            + " · inorden: " + UnirEstaciones(catalogo.EnOrden()) + " · ¿cumple la regla de orden? " + (catalogo.EsValido() ? "sí" : "no"));
    }

    /// <summary>
    /// Calcula ⌊log₂ n⌋, la menor altura posible de un árbol binario con n nodos.
    /// </summary>
    private static int AlturaMinima(int cantidad)
    {
        int altura = -1;
        int restante = cantidad;
        while (restante > 0)
        {
            restante = restante / 2;
            altura++;
        }

        return altura;
    }

    /// <summary>
    /// Une los códigos de una lista de nodos con flechas.
    /// </summary>
    private static string UnirCodigos(List<NodoBinario<Estacion>> nodos)
    {
        List<string> codigos = new List<string>();
        foreach (NodoBinario<Estacion> nodo in nodos)
        {
            codigos.Add(nodo.Valor.Codigo.ToString());
        }

        return string.Join(" → ", codigos);
    }

    /// <summary>
    /// Une los códigos de una lista de estaciones separados por comas.
    /// </summary>
    private static string UnirEstaciones(List<Estacion> estaciones)
    {
        List<string> codigos = new List<string>();
        foreach (Estacion estacion in estaciones)
        {
            codigos.Add(estacion.Codigo.ToString());
        }

        return string.Join(", ", codigos);
    }

    /// <summary>
    /// Da formato al caudal medio, con coma decimal.
    /// </summary>
    private static string Caudal(Estacion estacion)
    {
        return estacion.CaudalMedio.ToString("0.0", Espanol) + " m³/s";
    }

    /// <summary>
    /// Pide un número entero al usuario.
    /// </summary>
    /// <returns><c>true</c> si el texto escrito es un entero válido.</returns>
    private static bool PedirNumero(string mensaje, out int numero)
    {
        Console.Write(mensaje);
        string? texto = Console.ReadLine();
        if (texto != null && int.TryParse(texto.Trim(), out numero))
        {
            return true;
        }

        numero = 0;
        Console.WriteLine("Escriba un número entero.");
        return false;
    }

    /// <summary>
    /// Pide un texto al usuario.
    /// </summary>
    /// <returns>El texto sin espacios al inicio ni al final; vacío si no hay entrada.</returns>
    private static string PedirTexto(string mensaje)
    {
        Console.Write(mensaje);
        string? texto = Console.ReadLine();
        return texto == null ? "" : texto.Trim();
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
