using CatalogoEstaciones.Modelos;

namespace CatalogoEstaciones;

/// <summary>
/// Árbol binario de búsqueda de estaciones con clave Codigo.
/// Invariante: para todo nodo, los códigos de su izquierda son menores y los de su derecha mayores.
/// Los códigos no se repiten.
/// </summary>
public class ArbolBinarioBusqueda
{
    /// <summary>
    /// Nombre del caso de eliminación de una estación que es una hoja.
    /// </summary>
    public const string CasoHoja = "hoja";

    /// <summary>
    /// Nombre del caso de eliminación de una estación con un solo hijo.
    /// </summary>
    public const string CasoUnHijo = "un hijo";

    /// <summary>
    /// Nombre del caso de eliminación de una estación con dos hijos.
    /// </summary>
    public const string CasoDosHijos = "dos hijos";

    /// <summary>
    /// Raíz del árbol; <c>null</c> si el catálogo está vacío.
    /// </summary>
    public NodoBinario<Estacion>? Raiz { get; private set; }

    /// <summary>
    /// Cuenta las estaciones del catálogo.
    /// </summary>
    /// <returns>Número de estaciones; 0 si el catálogo está vacío.</returns>
    public int Contar()
    {
        return Contar(Raiz);
    }

    /// <summary>
    /// Calcula la altura del árbol en aristas; -1 si está vacío.
    /// </summary>
    /// <remarks>
    /// Buscar un código hace como máximo altura + 1 comparaciones.
    /// </remarks>
    /// <returns>Aristas del camino más largo desde la raíz hasta una hoja.</returns>
    public int Altura()
    {
        return Altura(Raiz);
    }

    /// <summary>
    /// Devuelve el menor código del catálogo.
    /// </summary>
    /// <returns>La estación con el menor código, o <c>null</c> si el catálogo está vacío.</returns>
    public Estacion? Minimo()
    {
        NodoBinario<Estacion>? actual = Raiz;
        if (actual == null)
        {
            return null;
        }

        while (actual.Izquierdo != null)
        {
            actual = actual.Izquierdo;
        }

        return actual.Valor;
    }

    /// <summary>
    /// Devuelve el mayor código del catálogo.
    /// </summary>
    /// <returns>La estación con el mayor código, o <c>null</c> si el catálogo está vacío.</returns>
    public Estacion? Maximo()
    {
        NodoBinario<Estacion>? actual = Raiz;
        if (actual == null)
        {
            return null;
        }

        while (actual.Derecho != null)
        {
            actual = actual.Derecho;
        }

        return actual.Valor;
    }

    /// <summary>
    /// Registra una estación bajando desde la raíz hasta una referencia <c>null</c>.
    /// </summary>
    /// <remarks>
    /// La estación nueva siempre queda en una hoja y ningún otro nodo se mueve.
    /// </remarks>
    /// <param name="estacion">Estación que se registra.</param>
    /// <returns><c>false</c> si el código ya existe; en ese caso el árbol no cambia.</returns>
    public bool Insertar(Estacion estacion)
    {
        NodoBinario<Estacion> nuevo = new NodoBinario<Estacion>(estacion);
        if (Raiz == null)
        {
            Raiz = nuevo;
            return true;
        }

        NodoBinario<Estacion> actual = Raiz;
        while (true)
        {
            if (estacion.Codigo == actual.Valor.Codigo)
            {
                return false;
            }

            if (estacion.Codigo < actual.Valor.Codigo)
            {
                if (actual.Izquierdo == null)
                {
                    actual.Izquierdo = nuevo;
                    return true;
                }

                actual = actual.Izquierdo;
            }
            else
            {
                if (actual.Derecho == null)
                {
                    actual.Derecho = nuevo;
                    return true;
                }

                actual = actual.Derecho;
            }
        }
    }

    /// <summary>
    /// Busca una estación por código. En cada nodo, una comparación decide si seguir por la izquierda o por la derecha.
    /// </summary>
    /// <param name="codigo">Código de la estación.</param>
    /// <returns>El nodo de la estación, o <c>null</c> si no está registrada.</returns>
    public NodoBinario<Estacion>? Buscar(int codigo)
    {
        NodoBinario<Estacion>? actual = Raiz;
        while (actual != null)
        {
            if (codigo == actual.Valor.Codigo)
            {
                return actual;
            }

            if (codigo < actual.Valor.Codigo)
            {
                actual = actual.Izquierdo;
            }
            else
            {
                actual = actual.Derecho;
            }
        }

        return null;
    }

    /// <summary>
    /// Hace la misma búsqueda que <see cref="Buscar"/>, pero guarda los nodos comparados.
    /// </summary>
    /// <param name="codigo">Código de la estación.</param>
    /// <returns>Los nodos comparados, en orden; su cantidad es el número de comparaciones.</returns>
    public List<NodoBinario<Estacion>> CaminoDeBusqueda(int codigo)
    {
        List<NodoBinario<Estacion>> camino = new List<NodoBinario<Estacion>>();
        NodoBinario<Estacion>? actual = Raiz;
        while (actual != null)
        {
            camino.Add(actual);
            if (codigo == actual.Valor.Codigo)
            {
                break;
            }

            if (codigo < actual.Valor.Codigo)
            {
                actual = actual.Izquierdo;
            }
            else
            {
                actual = actual.Derecho;
            }
        }

        return camino;
    }

    /// <summary>
    /// Indica qué caso de eliminación corresponde a un código, sin eliminarlo.
    /// </summary>
    /// <param name="codigo">Código de la estación.</param>
    /// <returns>"hoja", "un hijo", "dos hijos", o una cadena vacía si el código no existe.</returns>
    public string CasoDeEliminacion(int codigo)
    {
        NodoBinario<Estacion>? nodo = Buscar(codigo);
        if (nodo == null)
        {
            return "";
        }

        if (nodo.Izquierdo != null && nodo.Derecho != null)
        {
            return CasoDosHijos;
        }

        if (nodo.Izquierdo != null || nodo.Derecho != null)
        {
            return CasoUnHijo;
        }

        return CasoHoja;
    }

    /// <summary>
    /// Indica qué código ocuparía el lugar de una estación con dos hijos.
    /// </summary>
    /// <param name="codigo">Código de la estación que se retira.</param>
    /// <returns>El código del sucesor inorden, o -1 si el código no existe o no tiene dos hijos.</returns>
    public int SucesorInorden(int codigo)
    {
        NodoBinario<Estacion>? nodo = Buscar(codigo);
        if (nodo == null || nodo.Izquierdo == null || nodo.Derecho == null)
        {
            return -1;
        }

        NodoBinario<Estacion> sucesor = nodo.Derecho;
        while (sucesor.Izquierdo != null)
        {
            sucesor = sucesor.Izquierdo;
        }

        return sucesor.Valor.Codigo;
    }

    /// <summary>
    /// Retira una estación conservando la regla de orden.
    /// </summary>
    /// <remarks>
    /// Con dos hijos, copia en el nodo la ficha del sucesor inorden y luego elimina el sucesor,
    /// que tiene a lo sumo un hijo. Así los tres casos terminan en uno sencillo.
    /// </remarks>
    /// <param name="codigo">Código de la estación que se retira.</param>
    /// <returns><c>false</c> si el código no existe.</returns>
    public bool Eliminar(int codigo)
    {
        NodoBinario<Estacion>? padre = null;
        NodoBinario<Estacion>? actual = Raiz;

        while (actual != null && actual.Valor.Codigo != codigo)
        {
            padre = actual;
            if (codigo < actual.Valor.Codigo)
            {
                actual = actual.Izquierdo;
            }
            else
            {
                actual = actual.Derecho;
            }
        }

        if (actual == null)
        {
            return false;
        }

        if (actual.Izquierdo != null && actual.Derecho != null)
        {
            // Dos hijos: el sucesor inorden es el menor código del subárbol derecho.
            // Es mayor que todo el lado izquierdo y menor que el resto del derecho.
            NodoBinario<Estacion> padreDelSucesor = actual;
            NodoBinario<Estacion> sucesor = actual.Derecho;
            while (sucesor.Izquierdo != null)
            {
                padreDelSucesor = sucesor;
                sucesor = sucesor.Izquierdo;
            }

            actual.Valor = sucesor.Valor;
            padre = padreDelSucesor;
            actual = sucesor;
        }

        // Aquí actual tiene a lo sumo un hijo: es el caso hoja o el caso de un hijo.
        NodoBinario<Estacion>? hijo;
        if (actual.Izquierdo != null)
        {
            hijo = actual.Izquierdo;
        }
        else
        {
            hijo = actual.Derecho;
        }

        if (padre == null)
        {
            Raiz = hijo;
        }
        else if (padre.Izquierdo == actual)
        {
            padre.Izquierdo = hijo;
        }
        else
        {
            padre.Derecho = hijo;
        }

        return true;
    }

    /// <summary>
    /// Lista las estaciones en inorden, que en este árbol es el orden creciente de código.
    /// </summary>
    /// <returns>Las estaciones ordenadas por código.</returns>
    public List<Estacion> EnOrden()
    {
        List<Estacion> estaciones = new List<Estacion>();
        RecorrerEnOrden(Raiz, estaciones);
        return estaciones;
    }

    /// <summary>
    /// Obtiene las estaciones con código entre <paramref name="bajo"/> y <paramref name="alto"/>, ambos incluidos.
    /// </summary>
    /// <param name="bajo">Código inicial.</param>
    /// <param name="alto">Código final.</param>
    /// <param name="visitados">Recibe los nodos que se visitaron, para mostrar qué ramas se recorrieron.</param>
    /// <returns>Las estaciones del rango, ordenadas por código.</returns>
    public List<Estacion> Rango(int bajo, int alto, List<NodoBinario<Estacion>> visitados)
    {
        List<Estacion> resultado = new List<Estacion>();
        RecorrerRango(Raiz, bajo, alto, resultado, visitados);
        return resultado;
    }

    /// <summary>
    /// Verifica la regla de orden en todo el árbol usando intervalos.
    /// </summary>
    /// <remarks>
    /// La regla es global: no basta con comparar cada nodo con sus hijos.
    /// </remarks>
    /// <returns><c>true</c> si el árbol cumple el invariante de orden.</returns>
    public bool EsValido()
    {
        return EsValido(Raiz, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Cuenta los nodos del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    private int Contar(NodoBinario<Estacion>? nodo)
    {
        if (nodo == null)
        {
            return 0;
        }

        return 1 + Contar(nodo.Izquierdo) + Contar(nodo.Derecho);
    }

    /// <summary>
    /// Calcula la altura del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    private int Altura(NodoBinario<Estacion>? nodo)
    {
        if (nodo == null)
        {
            return -1;
        }

        int alturaIzquierda = Altura(nodo.Izquierdo);
        int alturaDerecha = Altura(nodo.Derecho);
        return 1 + Math.Max(alturaIzquierda, alturaDerecha);
    }

    /// <summary>
    /// Agrega a la lista las estaciones del subárbol en inorden.
    /// </summary>
    private void RecorrerEnOrden(NodoBinario<Estacion>? nodo, List<Estacion> estaciones)
    {
        if (nodo == null)
        {
            return;
        }

        RecorrerEnOrden(nodo.Izquierdo, estaciones);
        estaciones.Add(nodo.Valor);
        RecorrerEnOrden(nodo.Derecho, estaciones);
    }

    /// <summary>
    /// Recorre en inorden solo las ramas que pueden tener códigos del rango.
    /// </summary>
    private void RecorrerRango(NodoBinario<Estacion>? nodo, int bajo, int alto, List<Estacion> resultado,
        List<NodoBinario<Estacion>> visitados)
    {
        if (nodo == null)
        {
            return;
        }

        visitados.Add(nodo);
        int codigo = nodo.Valor.Codigo;

        if (bajo < codigo)
        {
            RecorrerRango(nodo.Izquierdo, bajo, alto, resultado, visitados);
        }

        if (bajo <= codigo && codigo <= alto)
        {
            resultado.Add(nodo.Valor);
        }

        if (codigo < alto)
        {
            RecorrerRango(nodo.Derecho, bajo, alto, resultado, visitados);
        }
    }

    /// <summary>
    /// Comprueba que todos los códigos del subárbol estén estrictamente entre los dos límites.
    /// </summary>
    private bool EsValido(NodoBinario<Estacion>? nodo, int limiteInferior, int limiteSuperior)
    {
        if (nodo == null)
        {
            return true;
        }

        int codigo = nodo.Valor.Codigo;
        if (codigo <= limiteInferior || codigo >= limiteSuperior)
        {
            return false;
        }

        return EsValido(nodo.Izquierdo, limiteInferior, codigo) && EsValido(nodo.Derecho, codigo, limiteSuperior);
    }
}
