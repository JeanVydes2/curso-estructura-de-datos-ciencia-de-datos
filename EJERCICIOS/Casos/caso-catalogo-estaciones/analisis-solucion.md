# Análisis y solución: catálogo de estaciones de la red hidrometeorológica

## Por qué un árbol de búsqueda

La operación dominante es buscar por código, y el catálogo cambia pocas veces al
año. Un árbol binario de búsqueda guarda los códigos con una regla: los menores a la
izquierda y los mayores a la derecha. Así, cada comparación descarta un subárbol
completo y la búsqueda recorre un solo camino, de costo O(h). Además, el inorden
entrega las estaciones ordenadas por código sin ordenar nada, un rango de códigos se
obtiene sin visitar las ramas que no pueden contenerlo, y el mínimo y el máximo
están al final de un solo camino.

## 1. El árbol con el orden de instalación

```text
                        201
             ┌───────────┴───────────┐
           105                       302
        ┌────┴───┐              ┌─────┴─────┐
      101      205            301         305
        \     ┌──┴──┐                     ┌─┘
        103 207   208                  303
          \
          104
```

Por ejemplo, el 208 es mayor que 201 (derecha), menor que 302 (izquierda), mayor
que 205 (derecha) y menor que 301 (izquierda): queda como hijo derecho de 205. El
primer dígito no decide la rama; solo cuentan las comparaciones. Por eso el 103, que
es de la zona del río Magdalena como el 101, no está pegado a él: cuelga de su lado
derecho porque se instaló después.

## 2. Altura

La altura es **4**, por los caminos 201 → 105 → 101 → 103 → 104 y
201 → 302 → 205 → 208 → 207. Con 12 claves, la menor altura posible es
⌊log₂ 12⌋ = 3.

## 3. Lecturas de las 08:00

| Código | Comparaciones | Resultado |
|---:|---|---|
| 103 | 201 → 105 → 101 → 103 (4) | aceptada: Río Magdalena, Gamarra |
| 208 | 201 → 302 → 205 → 208 (4) | aceptada: Ciénaga Grande, Fundación |
| 999 | 201 → 302 → 305 → null (3) | rechazada: la estación no está registrada |

Para rechazar el 999 bastan 3 comparaciones, no 12: la búsqueda termina al llegar a
una referencia `null`.

## 4. Inserción de la 204

204 > 201 (derecha), 204 < 302 (izquierda), 204 < 205 (izquierda) y `205.Izquierdo`
es `null`: la 204 queda como **hijo izquierdo de 205**. Ningún otro nodo se mueve. El
árbol sigue con altura 4.

## 5. Retiros

**301: hoja.** Cuelga a la derecha de 208. Su padre deja de apuntarla
(`208.Derecho = null`).

**101: un hijo.** Después del retiro anterior, 101 solo tiene a 103 (a su derecha).
Su padre, 105, pasa a apuntar a 103, y el subárbol de 103 sube un nivel. Ese subárbol
estaba a la derecha de 101 y sigue a la derecha de 105, así que el orden se conserva.

**201: dos hijos.** Es la raíz y tiene a 105 y a 302. Si se desconectara, se perdería
uno de sus subárboles. Se busca el **sucesor inorden**, el menor código del subárbol
derecho: desde 302 se baja por la izquierda a 205 y luego a 204, que no tiene hijo
izquierdo. Se copia la ficha del 204 en el nodo del 201 y se elimina el 204 original,
que era hoja. El 204 puede ocupar la raíz porque es mayor que todo el lado izquierdo
(105 y su subárbol) y menor que el resto del derecho (205, 207, 208, 302, 303, 305).

Árbol final:

```text
                        204
             ┌───────────┴───────────┐
           105                       302
          ┌─┴──┐              ┌──────┴──────┐
        103   205            (301 retirado) 305
          \     \                          ┌─┘
          104   208                      303
               ┌─┘
             207
```

Quedan 10 estaciones y el inorden sigue creciente: 103, 104, 105, 204, 205, 207,
208, 302, 303, 305. La altura no bajó de 4 porque el camino 204 → 302 → 205 → 208 →
207 sigue teniendo cuatro aristas.

## 6. Cuenca Ciénaga Grande (200 a 299)

Resultado: 204, 205, 207 y 208. El recorrido es un inorden que poda ramas: no baja a
la izquierda de un nodo con código ≤ 200 ni a la derecha de uno con código ≥ 299. El
subárbol izquierdo de 105 (103, 104) y el de la Sierra Nevada (302, 303, 305) quedan
fuera, así que no hace falta visitar los 10 nodos. Con 300 estaciones, la diferencia
entre visitar una cuenca y visitar todo el catálogo es mucho mayor.

## 7. Carga ordenada por código

Si los códigos llegan ordenados, cada uno es mayor que todos los anteriores y se
cuelga siempre a la derecha: el árbol se vuelve una cadena de altura n − 1 = **11**.
Buscar el código mayor cuesta 12 comparaciones, igual que recorrer una lista. Con 300
estaciones serían hasta 300 comparaciones por lectura. Un árbol de 300 claves con la
menor altura posible (⌊log₂ 300⌋ = 8) necesita como máximo 9, y si los códigos llegan
sin un orden particular, la altura esperada crece en proporción a log n.

## 8. Contraste con el programa

`dotnet run --project src/CatalogoEstaciones.csproj -- --demo` muestra las búsquedas
comparación por comparación, el mínimo y el máximo, la inserción, los tres retiros
con el árbol antes y después, el reporte de la Ciénaga Grande (con los nodos
visitados) y la carga ordenada. Las pruebas de `tests/` comprueban cada uno de estos
resultados.

## 9. Min y Max

```csharp
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
```

`Maximo()` es simétrico: sigue `Derecho`. Con el árbol del punto 1, `Minimo()`
devuelve 101 y `Maximo()` devuelve 305. Cada uno recorre un solo camino: O(h).

## Errores frecuentes

- Ubicar una estación por su cuenca en lugar de compararla con cada nodo. El 103 está
  en el subárbol izquierdo de 201 aunque sea del río Magdalena; la cuenca solo agrupa
  rangos de códigos.
- Reemplazar un nodo con dos hijos por cualquiera de ellos. Con la 201, subir la 105
  dejaría a 302 sin lugar.
- Suponer que buscar siempre cuesta log n. Cuesta la altura, y la altura depende del
  orden de llegada.
- Ordenar el resultado del inorden: ya sale ordenado.
- Recorrer todo el árbol para un rango de códigos. El inorden podado evita las ramas
  que no pueden contenerlo.
