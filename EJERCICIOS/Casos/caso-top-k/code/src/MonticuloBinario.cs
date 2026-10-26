namespace LluviaTopK;

/// <summary>
/// Montículo binario sobre List&lt;T&gt;. La raíz es el menor según el comparador.
/// Índices: hijos de i en 2i + 1 y 2i + 2; padre de i en (i - 1) / 2.
/// Invariante: ningún elemento es menor que su padre según el comparador.
/// </summary>
/// <typeparam name="T">Tipo de los elementos guardados.</typeparam>
public class MonticuloBinario<T>
{
    private readonly List<T> elementos = new List<T>();
    private readonly IComparer<T> comparador;

    /// <summary>
    /// Crea un montículo vacío.
    /// </summary>
    /// <param name="comparador">Criterio que decide qué elemento va primero.</param>
    public MonticuloBinario(IComparer<T> comparador)
    {
        this.comparador = comparador;
    }

    /// <summary>
    /// Crea un montículo a partir de una lista, con el método de Floyd.
    /// </summary>
    /// <remarks>
    /// Ajusta hacia abajo desde el último nodo interno, que es el trabajo O(n) de construir un montículo.
    /// </remarks>
    /// <param name="comparador">Criterio que decide qué elemento va primero.</param>
    /// <param name="elementos">Elementos con los que se llena el montículo.</param>
    public MonticuloBinario(IComparer<T> comparador, IEnumerable<T> elementos)
    {
        this.comparador = comparador;
        ConstruirDesde(elementos);
    }

    /// <summary>
    /// Cantidad de elementos guardados.
    /// </summary>
    public int Cantidad
    {
        get
        {
            return elementos.Count;
        }
    }

    /// <summary>
    /// Indica si el montículo no tiene elementos.
    /// </summary>
    public bool EstaVacio
    {
        get
        {
            return elementos.Count == 0;
        }
    }

    /// <summary>
    /// Altura del árbol que forma la lista: ⌊log₂ Cantidad⌋, o -1 si está vacío.
    /// </summary>
    public int Altura
    {
        get
        {
            int altura = -1;
            int restante = elementos.Count;
            while (restante > 0)
            {
                restante = restante / 2;
                altura++;
            }

            return altura;
        }
    }

    /// <summary>
    /// Intercambios que hizo la última operación.
    /// </summary>
    public int UltimosIntercambios { get; private set; }

    /// <summary>
    /// Consulta el elemento de la raíz sin sacarlo.
    /// </summary>
    /// <remarks>
    /// Lanza <c>InvalidOperationException</c> si el montículo está vacío.
    /// </remarks>
    /// <returns>El elemento que va primero según el comparador.</returns>
    public T VerRaiz()
    {
        if (elementos.Count == 0)
        {
            throw new InvalidOperationException("El montículo está vacío.");
        }

        return elementos[0];
    }

    /// <summary>
    /// Agrega un elemento al final de la lista y lo sube mientras vaya antes que su padre.
    /// </summary>
    /// <param name="elemento">Elemento que se agrega.</param>
    public void Insertar(T elemento)
    {
        UltimosIntercambios = 0;
        elementos.Add(elemento);
        Subir(elementos.Count - 1);
    }

    /// <summary>
    /// Saca la raíz, lleva el último elemento a la raíz y lo baja por el hijo que va primero.
    /// </summary>
    /// <remarks>
    /// Lanza <c>InvalidOperationException</c> si el montículo está vacío.
    /// </remarks>
    /// <returns>El elemento que estaba en la raíz.</returns>
    public T ExtraerRaiz()
    {
        if (elementos.Count == 0)
        {
            throw new InvalidOperationException("El montículo está vacío.");
        }

        UltimosIntercambios = 0;
        T raiz = elementos[0];
        int ultimo = elementos.Count - 1;
        elementos[0] = elementos[ultimo];
        elementos.RemoveAt(ultimo);

        if (elementos.Count > 0)
        {
            Bajar(0);
        }

        return raiz;
    }

    /// <summary>
    /// Devuelve una copia de la lista en el orden en que está guardada.
    /// </summary>
    /// <returns>Una lista con los elementos del montículo.</returns>
    public List<T> Copia()
    {
        return new List<T>(elementos);
    }

    /// <summary>
    /// Comprueba que ningún elemento vaya antes que su padre, que está en la posición (i - 1) / 2.
    /// </summary>
    /// <returns><c>true</c> si se cumple el invariante del montículo.</returns>
    public bool EsValido()
    {
        for (int indice = 1; indice < elementos.Count; indice++)
        {
            int padre = (indice - 1) / 2;
            if (comparador.Compare(elementos[indice], elementos[padre]) < 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Llena el montículo con una lista y lo ajusta con el método de Floyd.
    /// </summary>
    /// <remarks>
    /// El último nodo interno es <c>Count / 2 - 1</c>: los nodos posteriores son hojas y ya cumplen la
    /// propiedad. Recorrer de ese nodo hacia arriba y ajustar hacia abajo deja el montículo en O(n),
    /// en vez del O(n log n) de insertar los elementos uno por uno.
    /// </remarks>
    /// <param name="lista">Elementos que se cargan en el montículo; la lista recibida no se modifica.</param>
    /// <returns>Intercambios que hizo la construcción.</returns>
    public int ConstruirDesde(IEnumerable<T> lista)
    {
        UltimosIntercambios = 0;
        elementos.Clear();
        elementos.AddRange(lista);
        for (int indice = elementos.Count / 2 - 1; indice >= 0; indice--)
        {
            Bajar(indice);
        }

        return UltimosIntercambios;
    }

    /// <summary>
    /// Sube el elemento de <paramref name="indice"/> mientras vaya estrictamente antes que su padre.
    /// </summary>
    private void Subir(int indice)
    {
        int actual = indice;
        while (actual > 0)
        {
            int padre = (actual - 1) / 2;
            if (comparador.Compare(elementos[actual], elementos[padre]) >= 0)
            {
                break;
            }

            Intercambiar(actual, padre);
            actual = padre;
        }
    }

    /// <summary>
    /// Baja el elemento de <paramref name="indice"/> intercambiándolo con el hijo que va primero, mientras ese hijo vaya antes que él.
    /// </summary>
    private void Bajar(int indice)
    {
        int actual = indice;
        while (true)
        {
            int izquierdo = 2 * actual + 1;
            int derecho = 2 * actual + 2;
            int menor = actual;

            if (izquierdo < elementos.Count && comparador.Compare(elementos[izquierdo], elementos[menor]) < 0)
            {
                menor = izquierdo;
            }

            if (derecho < elementos.Count && comparador.Compare(elementos[derecho], elementos[menor]) < 0)
            {
                menor = derecho;
            }

            if (menor == actual)
            {
                break;
            }

            Intercambiar(actual, menor);
            actual = menor;
        }
    }

    /// <summary>
    /// Intercambia dos posiciones de la lista interna y cuenta el intercambio.
    /// </summary>
    private void Intercambiar(int primero, int segundo)
    {
        T temporal = elementos[primero];
        elementos[primero] = elementos[segundo];
        elementos[segundo] = temporal;
        UltimosIntercambios++;
    }
}
