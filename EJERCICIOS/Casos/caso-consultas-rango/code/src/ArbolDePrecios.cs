namespace IndiceDePrecios;

/// <summary>
/// Árbol binario de búsqueda de precios con tamaño por subárbol.
/// Invariante de orden: para todo nodo, los precios de su izquierda son menores y los de su derecha mayores.
/// Invariante de tamaño: <c>Tamano</c> es el número de nodos del subárbol, incluyéndolo.
/// </summary>
public class ArbolDePrecios
{
    /// <summary>
    /// Raíz del árbol; <c>null</c> si el índice está vacío.
    /// </summary>
    public NodoPrecio? Raiz { get; private set; }

    /// <summary>
    /// Cuenta los precios del índice.
    /// </summary>
    /// <returns>Número de precios; 0 si el índice está vacío.</returns>
    public int Contar()
    {
        return Raiz == null ? 0 : Raiz.Tamano;
    }

    /// <summary>
    /// Calcula la altura del árbol en aristas; -1 si está vacío.
    /// </summary>
    /// <returns>Aristas del camino más largo desde la raíz hasta una hoja.</returns>
    public int Altura()
    {
        return Altura(Raiz);
    }

    /// <summary>
    /// Registra un precio bajando desde la raíz hasta una referencia <c>null</c>.
    /// </summary>
    /// <remarks>
    /// El precio nuevo queda en una hoja y, al subir, se incrementa el tamaño de cada nodo del camino.
    /// </remarks>
    /// <param name="precio">Precio que se registra.</param>
    /// <param name="producto">Producto al que corresponde.</param>
    /// <returns><c>false</c> si el precio ya existe; en ese caso el árbol no cambia.</returns>
    public bool Insertar(int precio, string producto)
    {
        NodoPrecio nuevo = new NodoPrecio(precio, producto);
        if (Raiz == null)
        {
            Raiz = nuevo;
            return true;
        }

        NodoPrecio actual = Raiz;
        Stack<NodoPrecio> camino = new Stack<NodoPrecio>();
        while (true)
        {
            camino.Push(actual);
            if (precio == actual.Precio)
            {
                return false;
            }

            if (precio < actual.Precio)
            {
                if (actual.Izquierdo == null)
                {
                    actual.Izquierdo = nuevo;
                    break;
                }

                actual = actual.Izquierdo;
            }
            else
            {
                if (actual.Derecho == null)
                {
                    actual.Derecho = nuevo;
                    break;
                }

                actual = actual.Derecho;
            }
        }

        // El tamaño del nuevo nodo ya es 1: se suman los nodos del camino.
        foreach (NodoPrecio nodo in camino)
        {
            nodo.Tamano++;
        }

        return true;
    }

    /// <summary>
    /// Busca un precio por su valor.
    /// </summary>
    /// <param name="precio">Precio buscado.</param>
    /// <returns>El nodo del precio, o <c>null</c> si no está registrado.</returns>
    public NodoPrecio? Buscar(int precio)
    {
        NodoPrecio? actual = Raiz;
        while (actual != null)
        {
            if (precio == actual.Precio)
            {
                return actual;
            }

            actual = precio < actual.Precio ? actual.Izquierdo : actual.Derecho;
        }

        return null;
    }

    /// <summary>
    /// Obtiene el mayor precio que no supera <paramref name="valor"/>.
    /// </summary>
    /// <param name="valor">Valor de referencia.</param>
    /// <returns>El nodo del piso, o <c>null</c> si todos los precios son mayores.</returns>
    public NodoPrecio? Piso(int valor)
    {
        NodoPrecio? candidato = null;
        NodoPrecio? actual = Raiz;
        while (actual != null)
        {
            if (actual.Precio == valor)
            {
                return actual;
            }

            if (actual.Precio < valor)
            {
                candidato = actual;
                actual = actual.Derecho;
            }
            else
            {
                actual = actual.Izquierdo;
            }
        }

        return candidato;
    }

    /// <summary>
    /// Obtiene el menor precio que alcanza o supera <paramref name="valor"/>.
    /// </summary>
    /// <param name="valor">Valor de referencia.</param>
    /// <returns>El nodo del techo, o <c>null</c> si todos los precios son menores.</returns>
    public NodoPrecio? Techo(int valor)
    {
        NodoPrecio? candidato = null;
        NodoPrecio? actual = Raiz;
        while (actual != null)
        {
            if (actual.Precio == valor)
            {
                return actual;
            }

            if (actual.Precio > valor)
            {
                candidato = actual;
                actual = actual.Izquierdo;
            }
            else
            {
                actual = actual.Derecho;
            }
        }

        return candidato;
    }

    /// <summary>
    /// Obtiene el <paramref name="k"/>-ésimo precio más barato, con k desde 1.
    /// </summary>
    /// <remarks>
    /// En cada nodo, el tamaño del subárbol izquierdo dice cuántos precios van antes:
    /// si k cabe allí se baja a la izquierda; si es uno más que ese tamaño, el nodo es la respuesta;
    /// si no, se descarta el subárbol izquierdo y el nodo, y se sigue por la derecha con el k restante.
    /// </remarks>
    /// <param name="k">Posición buscada, desde 1.</param>
    /// <returns>El nodo del k-ésimo más barato, o <c>null</c> si k está fuera del índice.</returns>
    public NodoPrecio? KEsimo(int k)
    {
        if (k < 1 || k > Contar())
        {
            return null;
        }

        NodoPrecio actual = Raiz!;
        int restante = k;
        while (true)
        {
            int izquierda = actual.Izquierdo == null ? 0 : actual.Izquierdo.Tamano;
            if (restante == izquierda + 1)
            {
                return actual;
            }

            if (restante <= izquierda)
            {
                actual = actual.Izquierdo!;
            }
            else
            {
                restante = restante - izquierda - 1;
                actual = actual.Derecho!;
            }
        }
    }

    /// <summary>
    /// Cuenta cuántos precios hay estrictamente por debajo de <paramref name="valor"/>.
    /// </summary>
    /// <param name="valor">Valor de referencia.</param>
    /// <returns>Cantidad de precios menores que el valor.</returns>
    public int ConteoPorDebajo(int valor)
    {
        int contador = 0;
        NodoPrecio? actual = Raiz;
        while (actual != null)
        {
            if (valor <= actual.Precio)
            {
                actual = actual.Izquierdo;
            }
            else
            {
                // El nodo y todo su subárbol izquierdo son menores que el valor.
                contador = contador + 1 + (actual.Izquierdo == null ? 0 : actual.Izquierdo.Tamano);
                actual = actual.Derecho;
            }
        }

        return contador;
    }

    /// <summary>
    /// Lista los precios en inorden, que es el orden creciente.
    /// </summary>
    /// <returns>Los nodos ordenados por precio.</returns>
    public List<NodoPrecio> EnOrden()
    {
        List<NodoPrecio> precios = new List<NodoPrecio>();
        RecorrerEnOrden(Raiz, precios);
        return precios;
    }

    /// <summary>
    /// Obtiene los precios con valor entre <paramref name="bajo"/> y <paramref name="alto"/>, ambos incluidos.
    /// </summary>
    /// <param name="bajo">Precio inicial.</param>
    /// <param name="alto">Precio final.</param>
    /// <returns>Los precios del rango, ordenados.</returns>
    public List<NodoPrecio> Rango(int bajo, int alto)
    {
        List<NodoPrecio> resultado = new List<NodoPrecio>();
        RecorrerRango(Raiz, bajo, alto, resultado);
        return resultado;
    }

    /// <summary>
    /// Verifica la regla de orden y la coherencia del tamaño en todo el árbol.
    /// </summary>
    /// <returns><c>true</c> si se cumplen los dos invariantes.</returns>
    public bool EsValido()
    {
        return EsValido(Raiz, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Calcula la altura del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    private int Altura(NodoPrecio? nodo)
    {
        if (nodo == null)
        {
            return -1;
        }

        return 1 + Math.Max(Altura(nodo.Izquierdo), Altura(nodo.Derecho));
    }

    /// <summary>
    /// Agrega a la lista los nodos del subárbol en inorden.
    /// </summary>
    private void RecorrerEnOrden(NodoPrecio? nodo, List<NodoPrecio> precios)
    {
        if (nodo == null)
        {
            return;
        }

        RecorrerEnOrden(nodo.Izquierdo, precios);
        precios.Add(nodo);
        RecorrerEnOrden(nodo.Derecho, precios);
    }

    /// <summary>
    /// Recorre en inorden solo las ramas que pueden tener precios del rango.
    /// </summary>
    private void RecorrerRango(NodoPrecio? nodo, int bajo, int alto, List<NodoPrecio> resultado)
    {
        if (nodo == null)
        {
            return;
        }

        if (bajo < nodo.Precio)
        {
            RecorrerRango(nodo.Izquierdo, bajo, alto, resultado);
        }

        if (bajo <= nodo.Precio && nodo.Precio <= alto)
        {
            resultado.Add(nodo);
        }

        if (nodo.Precio < alto)
        {
            RecorrerRango(nodo.Derecho, bajo, alto, resultado);
        }
    }

    /// <summary>
    /// Comprueba la regla de orden y que el tamaño de cada nodo sea 1 + sus dos subárboles.
    /// </summary>
    private bool EsValido(NodoPrecio? nodo, int limiteInferior, int limiteSuperior)
    {
        if (nodo == null)
        {
            return true;
        }

        if (nodo.Precio <= limiteInferior || nodo.Precio >= limiteSuperior)
        {
            return false;
        }

        int esperado = 1 + (nodo.Izquierdo == null ? 0 : nodo.Izquierdo.Tamano)
            + (nodo.Derecho == null ? 0 : nodo.Derecho.Tamano);
        if (nodo.Tamano != esperado)
        {
            return false;
        }

        return EsValido(nodo.Izquierdo, limiteInferior, nodo.Precio) && EsValido(nodo.Derecho, nodo.Precio, limiteSuperior);
    }
}
