# Análisis y solución: los K pluviómetros con más lluvia

## Por qué un montículo de tamaño K

Para quedarse con los K mayores no hace falta ordenar los 6 000 000 de filas
(O(n log n)). Basta con mantener un montículo de **mínimos** de tamaño K: la raíz es el
**menor de los K mayores**, que es el candidato a salir. Por cada fila:

- si el montículo tiene menos de K elementos, la fila entra;
- si ya tiene K, se compara la fila con la raíz; si es mayor, se saca la raíz y entra la
  fila.

Cada fila toca a lo sumo un camino del montículo, de altura ⌊log₂ K⌋, así que el
recorrido cuesta O(n log K) y la memoria queda en K puntos. Es la razón por la que no
hay que ordenar el archivo.

## 1. Criterio del montículo

Un montículo de **mínimos** por el acumulado: en la raíz está el menor de los K
guardados. Se compara la fila nueva con la raíz; si la fila es mayor, la raíz es la que
sobra. Si se usara un montículo de máximos, la raíz sería el mayor, y no habría forma
barata de saber cuál de los K es el que está en el límite.

## 2. Recorrido del lote con K = 3

| Lectura | Punto:mm | Lista después | Intercambios | Sale |
|---:|---|---|---:|---|
| 1 | P01:12,4 | [P01:12,4] | 0 | |
| 2 | P02:0,0 | [P02:0,0, P01:12,4] | 1 | |
| 3 | P03:35,8 | [P02:0,0, P01:12,4, P03:35,8] | 0 | |
| 4 | P04:7,2 | [P04:7,2, P03:35,8, P01:12,4] | 1 | P02 |
| 5 | P05:51,3 | [P01:12,4, P03:35,8, P05:51,3] | 0 | P04 |
| 6 | P06:4,6 | [P01:12,4, P03:35,8, P05:51,3] | 0 | no sale |
| 7 | P07:28,9 | [P07:28,9, P05:51,3, P03:35,8] | 1 | P01 |
| 8 | P08:0,3 | [P07:28,9, P05:51,3, P03:35,8] | 0 | no sale |
| 9 | P09:65,7 | [P03:35,8, P05:51,3, P09:65,7] | 0 | P07 |
| 10 | P10:18,1 | [P03:35,8, P05:51,3, P09:65,7] | 0 | no sale |
| 11 | P11:9,5 | [P03:35,8, P05:51,3, P09:65,7] | 0 | no sale |
| 12 | P12:42,0 | [P12:42,0, P09:65,7, P05:51,3] | 1 | P03 |

El algoritmo compara la lectura con la raíz **sin sacarla**; solo si la supera la saca e
inserta la nueva. Cuando la lectura no supera a la raíz (lecturas 6, 8, 10 y 11), el
montículo no cambia y no sale nadie: la columna "Sale" queda vacía y los intercambios
son 0. Los puntos que salen son los que alguna lectura mayor desplazó: P02, P04, P01,
P07 y P03.

## 3. Por qué la lista final no está ordenada

El montículo solo garantiza que cada padre no es mayor que sus hijos; no ordena entre
hermanos. La lista final es [P12:42,0, P09:65,7, P05:51,3]: la raíz es el menor de los
tres (42,0), no el mayor. Para obtenerlos de mayor a menor hay que **vaciar** el
montículo: en cada paso sale la raíz, que es el menor de los que quedan, y se van
guardando al frente. Al extraer los tres y ordenar el resultado queda
P09 (65,7), P05 (51,3), P12 (42,0). El proyecto lo hace con `TopK`; la estructura por sí
sola no entrega el resultado ordenado.

## 4. Los K menores en vez de los K mayores

Cambia **el comparador**, no el recorrido. Para los K menores se usa un montículo de
**máximos**: la raíz es el mayor de los K menores, y la fila nueva entra solo si es
menor que la raíz. El algoritmo es el mismo (recorrer, comparar con la raíz, reemplazar
si conviene); lo único que se invierte es el signo del comparador. Así se obtiene
bottom-K con exactamente el mismo código.

## 5. Construir un montículo desde una lista: uno por uno contra Floyd

Insertar uno por uno hace que cada elemento suba por su camino hasta encontrar su sitio;
los elementos que entran cerca de las hojas recorren varios niveles, y el total de
trabajo es O(n log n). El método de **Floyd** (o *heapify*) empieza desde el último nodo
interno y ajusta hacia abajo: en un montículo la mayoría de los nodos están cerca del
fondo, y un nodo a un nivel k baja a lo sumo (altura − k) niveles. Sumado, da O(n), y en
la práctica hace menos intercambios: con los 12 elementos del lote, insertar uno por uno
cuesta **7** intercambios y Floyd, **6**. La lista final **no** es la misma, porque hay
varias formas válidas de montículo; lo que sí coincide es la propiedad de que la raíz es
el menor.

## 6. Cuántas comparaciones hace el recorrido

Cada fila hace una comparación con la raíz y, si entra, a lo sumo ⌊log₂ K⌋
comparaciones para bajar hasta su sitio. En total son **O(n log K)** comparaciones:
lineal en el tamaño del archivo y logarítmico en K. Ordenar el archivo costaría
O(n log n), que con n = 6 000 000 y K = 10 es mucho más: log₂ n ≈ 22 contra log₂ K ≈ 3.
Por eso el recorrido de una sola pasada es la elección.

## 7. Contraste con el programa

`dotnet run --project src/TopK.csproj -- --demo` recorre el lote y muestra la lista del
montículo tras cada lectura, los intercambios, los puntos que salen, el Top-3 de mayor a
menor, el Bottom-3 con el comparador invertido y la construcción por Floyd contra la
inserción uno por uno. Las pruebas de `tests/` comprueban cada uno de estos resultados.

## 8. Si `ConstruirDesde` inserta uno por uno

La propiedad del montículo se mantiene, así que el resultado sigue siendo un montículo
válido y su raíz sigue siendo el menor, pero la **lista interna** cambia y el número de
intercambios sube de 6 a 7 con el lote. Como la lista de un montículo no es única
—varios arreglos representan el mismo montículo—, el dibujo del árbol puede verse
distinto aunque la raíz y la propiedad sean las mismas.

## Errores frecuentes

- Usar un montículo de máximos para el Top-K: la raíz queda en el mayor y no se sabe
  cuál sobra.
- Comparar con el último en vez de con la raíz, que es el menor de los K.
- Olvidar sacar la raíz después de insertar la fila que la supera: el montículo crece
  más allá de K.
- Mostrar la lista del montículo como si estuviera ordenada: solo la raíz está en su
  sitio.
- Sacar de un montículo vacío en un lote más corto que K: `ExtraerRaiz` lanza
  `InvalidOperationException`.
