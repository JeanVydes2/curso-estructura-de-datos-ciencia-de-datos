# Catálogo de estaciones de la red hidrometeorológica del Magdalena

## Actor y contexto

El Ideam y la Corporación Autónoma Regional del Magdalena operan una red de
estaciones hidrometeorológicas que miden caudal, nivel y precipitación en la cuenca
del río Magdalena, en la Ciénaga Grande y en la Sierra Nevada. Cada estación envía
una lectura cada quince minutos, y un pipeline de datos valida cada lectura antes de
publicarla en el visor de alertas tempranas.

El primer paso de la validación es consultar el catálogo de estaciones. Cada lectura
trae solo el código de la estación; con ese código el pipeline busca la ficha
(cuenca, municipio y variable). Si el código no existe, la lectura se rechaza: puede
venir de una estación dada de baja, de un error de configuración o de un equipo que
nadie registró.

El código tiene tres dígitos y el primero indica la cuenca: 1 es río Magdalena, 2 es
Ciénaga Grande y 3 es Sierra Nevada. Los códigos no se repiten. Las estaciones se
registran a medida que se instalan, así que el orden de llegada no sigue el orden de
los códigos.

Una búsqueda lenta retrasa todo el flujo, y una búsqueda equivocada tiene costo
público: si una lectura válida se rechaza, una alerta de creciente puede salir
tarde; si se acepta la de una estación dada de baja, el visor muestra datos de un
punto que ya no se mide.

## Necesidad o problema

El equipo de datos necesita un catálogo en memoria que permita:

1. buscar una estación por código en cada lectura;
2. registrar estaciones nuevas y dar de baja estaciones sin reconstruir todo el
   catálogo;
3. listar las estaciones ordenadas por código para el inventario trimestral;
4. obtener las estaciones de una cuenca, es decir, las de un rango de códigos, para
   el reporte diario;
5. obtener el menor y el mayor código del catálogo en cualquier momento.

## Datos y campos

**Estaciones en el orden en que se instalaron:**

| Orden | Código | Cuenca | Municipio | Variable | Caudal medio (m³/s) |
|---:|---:|---|---|---|---:|
| 1 | 201 | Ciénaga Grande | Ciénaga | Nivel | 12,0 |
| 2 | 105 | Río Magdalena | El Banco | Caudal | 480,0 |
| 3 | 302 | Sierra Nevada | Minca | Precipitación | 0,0 |
| 4 | 101 | Río Magdalena | Barrancabermeja | Caudal | 5 200,0 |
| 5 | 205 | Ciénaga Grande | Aracataca | Nivel | 8,5 |
| 6 | 305 | Sierra Nevada | Santa Marta | Precipitación | 0,0 |
| 7 | 103 | Río Magdalena | Gamarra | Caudal | 2 400,0 |
| 8 | 208 | Ciénaga Grande | Fundación | Nivel | 9,0 |
| 9 | 104 | Río Magdalena | Plato | Caudal | 3 300,0 |
| 10 | 303 | Sierra Nevada | La Tagua | Precipitación | 0,0 |
| 11 | 207 | Ciénaga Grande | Remolino | Nivel | 7,5 |
| 12 | 301 | Sierra Nevada | Pueblo Bello | Precipitación | 0,0 |

**Lote de lecturas de las 08:00** (solo interesa el código para validarlas): 103,
208 y 999.

**Cambios del año, en este orden:**

1. se instala la estación **204**, Ciénaga Grande, Zapayán, Nivel;
2. se da de baja la **301** (la estación de Pueblo Bello pasó a otra red);
3. se da de baja la **101** (la sede de Barrancabermeja se reubicó);
4. se da de baja la **201** (la estación de Ciénaga cambió de operador).

## Volumen y crecimiento

La red tiene 12 estaciones. Con una lectura cada quince minutos, son 1 152 lecturas
al día y cada una exige una búsqueda en el catálogo. El plan de expansión lleva la
red a 300 estaciones en tres años: 28 800 búsquedas diarias. Un compañero propone
cargar el catálogo desde el inventario exportado, que viene **ordenado por código**.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Buscar una estación por código | una vez por lectura: miles por día |
| Registrar una estación | pocas veces al año |
| Dar de baja una estación | pocas veces al año |
| Listar estaciones por código | trimestral |
| Estaciones de una cuenca (rango de códigos) | diario |
| Menor y mayor código del catálogo | al revisar el rango de alertas |

## Restricciones

- La búsqueda no puede crecer con el tamaño de la red al ritmo de una lista: debe
  recorrer un solo camino.
- Los códigos son únicos: un registro con un código existente se rechaza.
- Dar de baja una estación no puede dejar el catálogo desordenado ni perder otras
  estaciones.

## Pregunta o tarea

Trabaja en parejas y registra tus respuestas en un archivo antes de ejecutar el
programa.

1. Inserta las 12 estaciones en el orden de instalación y dibuja el árbol de
   búsqueda resultante, con el código como clave.
2. ¿Cuál es su altura? ¿Cuál sería la menor altura posible con 12 claves?
3. Para cada lectura del lote de las 08:00, escribe los códigos que se comparan,
   cuántas comparaciones hay y si la lectura se acepta o se rechaza.
4. ¿Dónde queda la estación 204? Escribe el camino que sigue la inserción.
5. Aplica los tres retiros en orden. Para cada uno indica el caso de eliminación
   (hoja, un hijo o dos hijos) y dibuja el árbol después del cambio. En el tercero,
   ¿qué código ocupa el lugar del 201 y por qué?
6. Con el árbol final, ¿qué estaciones tienen código entre 200 y 299? ¿Hace falta
   visitar los 10 nodos para responder?
7. ¿Qué altura tendría el árbol si se cargan las 12 estaciones originales ordenadas
   por código, como propone tu compañero? ¿Cuántas comparaciones costaría buscar el
   código mayor? ¿Qué pasaría con 300 estaciones?
8. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la salida.
   La opción 3 del menú muestra la búsqueda paso a paso.
9. Agrega a `ArbolBinarioBusqueda` los métodos `Minimo()` y `Maximo()`, que devuelven
   la estación de menor y de mayor código, y escribe una prueba para cada uno con el
   árbol del punto 1.
