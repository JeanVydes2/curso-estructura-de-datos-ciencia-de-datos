using System.Globalization;
using System.Text;

namespace IndiceDePrecios;

/// <summary>
/// Menú y modo de demostración del caso de consultas de orden sobre el índice de precios.
/// </summary>
public class Program
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-CO");
    private static bool pausasActivas = true;

    /// <summary>
    /// Inserta los precios del caso, dibuja el índice y abre el menú.
    /// </summary>
    /// <param name="args">Con <c>--demo</c> ejecuta las consultas del día, sin pausas.</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        ArbolDePrecios indice = new ArbolDePrecios();
        foreach (Producto producto in DatosDelCaso.Precios())
        {
            indice.Insertar(producto.Precio, producto.Nombre);
        }

        EscribirTitulo("Índice de precios de la lonja");
        Console.WriteLine("Se registraron los " + indice.Contar() + " precios en el orden en que llegaron los lotes.");
        MostrarArbol(indice);
        Console.WriteLine("Cada nodo se etiqueta [precio:tamaño]; el tamaño es cuántos nodos cuelgan de él.");

        // --demo responde las consultas del día, sin pausas.
        if (args.Length > 0 && args[0] == "--demo")
        {
            pausasActivas = false;
            MostrarEnOrden(indice);
            foreach (int valor in DatosDelCaso.ValoresDePisoYTecho())
            {
                MostrarPisoYTecho(indice, valor);
            }

            MostrarKesimos(indice);
            MostrarConteoPorDebajo(indice, DatosDelCaso.ValorDeConteo());
            MostrarRango(indice, 20000, 50000);
            return;
        }

        EjecutarMenu(indice);
    }

    /// <summary>
    /// Muestra el menú y ejecuta la opción elegida hasta que se escoja 0 o termine la entrada.
    /// </summary>
    private static void EjecutarMenu(ArbolDePrecios indice)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Ver el árbol (con tamaños)");
            Console.WriteLine("2. Listar precios en orden (inorden)");
            Console.WriteLine("3. Piso y techo de un valor");
            Console.WriteLine("4. K-ésimo precio más barato");
            Console.WriteLine("5. Cuántos precios hay por debajo de un valor");
            Console.WriteLine("6. Reporte por rango de precios");
            Console.WriteLine("7. Registrar un precio");
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
                    MostrarArbol(indice);
                    break;
                case "2":
                    MostrarEnOrden(indice);
                    break;
                case "3":
                    int valor;
                    if (PedirNumero("Valor de referencia: ", out valor))
                    {
                        MostrarPisoYTecho(indice, valor);
                    }

                    break;
                case "4":
                    int k;
                    if (PedirNumero("Posición k (desde 1): ", out k))
                    {
                        MostrarKesimo(indice, k);
                    }

                    break;
                case "5":
                    int referencia;
                    if (PedirNumero("Valor de referencia: ", out referencia))
                    {
                        MostrarConteoPorDebajo(indice, referencia);
                    }

                    break;
                case "6":
                    int bajo;
                    int alto;
                    if (PedirNumero("Precio inicial: ", out bajo) && PedirNumero("Precio final: ", out alto))
                    {
                        MostrarRango(indice, bajo, alto);
                    }

                    break;
                case "7":
                    int precio;
                    if (PedirNumero("Precio: ", out precio))
                    {
                        string nombre = PedirTexto("Producto: ");
                        if (indice.Insertar(precio, nombre))
                        {
                            Console.WriteLine("Registrado. El árbol ahora tiene " + indice.Contar() + " precios.");
                        }
                        else
                        {
                            Console.WriteLine("El precio " + Pesos(precio) + " ya existe: no se registra un duplicado.");
                        }
                    }

                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    /// <summary>
    /// Dibuja el árbol con sus tamaños y muestra su altura.
    /// </summary>
    private static void MostrarArbol(ArbolDePrecios indice)
    {
        Console.WriteLine();
        ImpresorPrecios.Imprimir(indice.Raiz);
        Console.WriteLine("Precios: " + indice.Contar() + " · altura: " + indice.Altura()
            + " · ¿cumple orden y tamaños? " + (indice.EsValido() ? "sí" : "no"));
    }

    /// <summary>
    /// Lista los precios en inorden, del más barato al más caro.
    /// </summary>
    private static void MostrarEnOrden(ArbolDePrecios indice)
    {
        EscribirTitulo("Precios en orden (inorden)");
        int posicion = 1;
        foreach (NodoPrecio nodo in indice.EnOrden())
        {
            Console.WriteLine("  " + posicion + ".º " + Pesos(nodo.Precio) + "  " + nodo.Producto);
            posicion++;
        }
    }

    /// <summary>
    /// Muestra el piso y el techo de un valor.
    /// </summary>
    private static void MostrarPisoYTecho(ArbolDePrecios indice, int valor)
    {
        EscribirTitulo("Piso y techo de " + Pesos(valor));
        NodoPrecio? piso = indice.Piso(valor);
        NodoPrecio? techo = indice.Techo(valor);

        Console.WriteLine("Piso  (el mayor precio que no supera " + Pesos(valor) + "): " + Describir(piso));
        Console.WriteLine("Techo (el menor precio que alcanza o supera " + Pesos(valor) + "): " + Describir(techo));
        Pausar();
    }

    /// <summary>
    /// Muestra el k-ésimo precio más barato.
    /// </summary>
    private static void MostrarKesimo(ArbolDePrecios indice, int k)
    {
        EscribirTitulo("El " + k + ".º precio más barato");
        NodoPrecio? nodo = indice.KEsimo(k);
        Console.WriteLine(Describir(nodo));
        Pausar();
    }

    /// <summary>
    /// Muestra la tabla de los k-ésimos consultados.
    /// </summary>
    private static void MostrarKesimos(ArbolDePrecios indice)
    {
        EscribirTitulo("K-ésimos del día");
        foreach (int k in DatosDelCaso.PosicionesK())
        {
            NodoPrecio? nodo = indice.KEsimo(k);
            Console.WriteLine("  " + k + ".º " + Describir(nodo));
        }
    }

    /// <summary>
    /// Muestra cuántos precios hay estrictamente por debajo de un valor.
    /// </summary>
    private static void MostrarConteoPorDebajo(ArbolDePrecios indice, int valor)
    {
        EscribirTitulo("Precios por debajo de " + Pesos(valor));
        int cantidad = indice.ConteoPorDebajo(valor);
        Console.WriteLine(cantidad + " productos cuestan menos de " + Pesos(valor) + ".");
        Pausar();
    }

    /// <summary>
    /// Muestra los precios de un rango.
    /// </summary>
    private static void MostrarRango(ArbolDePrecios indice, int bajo, int alto)
    {
        EscribirTitulo("Precios entre " + Pesos(bajo) + " y " + Pesos(alto));
        foreach (NodoPrecio nodo in indice.Rango(bajo, alto))
        {
            Console.WriteLine("  " + Pesos(nodo.Precio) + "  " + nodo.Producto);
        }

        Pausar();
    }

    /// <summary>
    /// Describe un nodo con su precio y producto, o dice que no hay resultado.
    /// </summary>
    private static string Describir(NodoPrecio? nodo)
    {
        return nodo == null ? "no hay" : Pesos(nodo.Precio) + " (" + nodo.Producto + ")";
    }

    /// <summary>
    /// Da formato a un precio en pesos, con separador de miles.
    /// </summary>
    private static string Pesos(int valor)
    {
        return valor.ToString("N0", Espanol) + " COP";
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
