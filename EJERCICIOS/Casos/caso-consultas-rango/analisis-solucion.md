# Análisis y solución: consultas de orden sobre el índice de precios

## Por qué un árbol de búsqueda con tamaños

Las cuatro consultas dependen del orden de los precios, y el árbol de búsqueda ya
pone ese orden en su forma. Si además cada nodo guarda **cuántos nodos hay en su
subárbol**, entonces:

- el piso y el techo bajan por un solo camino comparando con el valor buscado;
- el k-ésimo más barato se decide con el tamaño del subárbol izquierdo: si el k-ésimo
  aún no cabe a la izquierda, se descarta ese subárbol completo y se ajusta k;
- el conteo por debajo de un valor suma el subárbol izquierdo cada vez que se gira a
  la derecha.

Sin el tamaño, el k-ésimo y el conteo tendrían que recorrer el árbol entero en inorden
hasta el k-ésimo, que es O(n).

## 1. El árbol con el orden de llegada

Precios en miles de COP, para abreviar: 35, 12, 48, 8, 27, 55, 19, 62, 3, 41, 23, 50.

```text
                      35
       ┌───────────────┴───────┐
      12                      48
   ┌───┴───────────┐       ┌───┴───────┐
   8              27      41          55
┌──┘       ┌───────┘               ┌───┴───┐
3         19                      50      62
           └───┐
              23
```

La raíz es **35** (el café, primer lote) y la altura es **4**, por el camino 35 → 12 →
8 → 3.

## 2. Tamaños de los subárboles

Bajo cada nodo, el tamaño de su subárbol es el número de productos que cuelgan de él,
incluyéndolo:

| Nodo | Subárbol | Tamaño |
|---:|---|---:|
| 35 | raíz | 12 |
| 12 | 12, 8, 3, 27, 19, 23 | 6 |
| 48 | 48, 41, 55, 50, 62 | 5 |
| 8 | 8, 3 | 2 |
| 27 | 27, 19, 23 | 3 |
| 55 | 55, 50, 62 | 3 |
| 3, 41, 50, 62 | hoja | 1 |
| 19 | 19, 23 | 2 |
| 23 | hoja | 1 |

El tamaño **no** es la altura: el nodo 55 tiene tamaño 3 y altura 1. El tamaño es el
número de nodos; la altura es la longitud del camino más largo.

## 3. Piso y techo

| Consulta | Resultado | Por qué |
|---|---|---|
| Piso de 30 000 | 27 000 (ají) | es el mayor precio que no supera 30 000 |
| Techo de 30 000 | 35 000 (café) | es el menor precio que alcanza o supera 30 000 |
| Piso de 3 000 | 3 000 (ñame) | el menor precio del catálogo es piso de su propio valor |
| Techo de 62 000 | 62 000 (camarón) | el mayor precio del catálogo es techo de su propio valor |
| Piso de 1 000 | (no hay) | 1 000 es menor que todo el catálogo |
| Techo de 70 000 | (no hay) | 70 000 es mayor que todo el catálogo |

El piso y el techo bajan por un solo camino, anotando el mejor candidato encontrado:

- Para el piso de 30 000: en 35 se va a la izquierda (35 > 30); en 12 el 12 ≤ 30 pasa a
  ser candidato y se va a la derecha; en 27, 27 ≤ 30 pasa a ser candidato y se va a la
  derecha, que es `null`. El último candidato es 27.
- Para el techo de 30 000: en 35, 35 ≥ 30 pasa a ser candidato y se va a la izquierda;
  en 12 se va a la derecha (12 < 30); en 27 se va a la derecha (27 < 30), que es `null`.
  El último candidato es 35.

## 4. K-ésimo más barato

En inorden, los precios quedan: 3, 8, 12, 19, 23, 27, 35, 41, 48, 50, 55, 62.

| k | Producto | Precio |
|---:|---|---:|
| 1.º | Ñame | 3 000 |
| 6.º | Ají | 27 000 |
| 7.º | Café pergamino | 35 000 |
| 12.º | Camarón | 62 000 |

Para el 7.º: en la raíz 35, el subárbol izquierdo tiene 6 nodos (tamaño 6). Como
7 = 6 + 1, el 7.º más barato es la raíz misma, 35. Si k hubiera sido 8, se descarta el
subárbol izquierdo completo (6 nodos más la raíz) y se busca el (8 − 7) = 1.º del
subárbol derecho. Así cada paso descarta un subárbol entero sin recorrerlo.

## 5. Cuántos precios hay por debajo de 41 000

Hay **7** productos más baratos que 41 000: 3, 8, 12, 19, 23, 27 y 35. El recorrido
suma el subárbol izquierdo cada vez que gira a la derecha:

- en 35 (35 < 41): suma 1 + tamaño de su izquierda (6) = 7 y gira a la derecha;
- en 48 (48 ≥ 41): gira a la izquierda;
- en 41 (41 ≥ 41): gira a la izquierda;
- en `null`: termina con 7.

No es el mismo cálculo que el rango: el rango devuelve la lista de productos del
intervalo; el conteo solo cuenta cuántos hay por debajo, sin construirlos.

## 6. Por qué el tamaño cambia el costo

Sin el tamaño guardado, para el k-ésimo habría que recorrer en inorden los primeros k
nodos, que en el peor caso es O(n); para el conteo por debajo, recorrer todo el árbol.
Con el tamaño, cada paso descarta un subárbol entero usando un número que ya está en el
nodo, así que ambos recorren un solo camino de la raíz a una hoja: O(h). Con 400
productos, la diferencia entre O(n) y O(h) es la diferencia entre revisar 400 nodos y
unos 9.

## 7. Contraste con el programa

`dotnet run --project src/IndiceDePrecios.csproj -- --demo` muestra el árbol con los
tamaños, cada consulta de piso y techo, la tabla de los k-ésimos y el conteo por
debajo. Las pruebas de `tests/` comprueban cada resultado.

## 8. Si `Insertar` no actualiza el tamaño

El árbol sigue cumpliendo la regla de orden —cada precio menor a la izquierda y mayor a
la derecha—, así que `EsValido()` sigue en verdadero y el piso y el techo, que no miran
el tamaño, siguen correctos. Pero los tamaños quedan en 1 o desactualizados, y el
k-ésimo baja por la izquierda cuando ya no cabía, así que devuelve un producto
equivocado; el conteo por debajo también queda mal. El error está en el mantenimiento
del invariante de tamaño, no en el de orden.

## Errores frecuentes

- Confundir el tamaño del subárbol con la altura. El tamaño es cuántos nodos hay; la
  altura, cuántos niveles.
- Actualizar el tamaño solo en el nodo insertado. Hay que actualizarlo en cada nodo del
  camino desde la raíz.
- Devolver 0 como piso o techo cuando no hay respuesta. Deben devolver "no hay".
- Comparar el k-ésimo con el tamaño del subárbol derecho en lugar del izquierdo.
- Calcular el conteo con `>` en vez de `>=`: el producto con el precio exacto no cuenta
  como "por debajo".
