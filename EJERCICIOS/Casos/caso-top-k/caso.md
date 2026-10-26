# Los diez pluviómetros con más lluvia de un archivo de un millón de filas

## Actor y contexto

La Corporación Autónoma Regional del Magdalena publica cada mes un boletín con los
puntos de la red que registraron **más lluvia**. La red tiene 240 pluviómetros y cada
uno envía una lectura cada diez minutos. El archivo mensual del exportador trae las
6 000 000 de lecturas del mes, pero el boletín solo necesita los 10 puntos con mayor
acumulado.

Un ingeniero de datos propone cargar el archivo en una lista, ordenarla de mayor a menor
y quedarse con los primeros 10. Funciona, pero ordena 6 000 000 de elementos para
responder una pregunta sobre 10, y con el crecimiento de la red el archivo será más
grande cada año. La alternativa es recorrer el archivo una vez manteniendo solo los 10
mayores: cuando llega una lectura mayor que el menor de esos 10, se reemplaza.

## Necesidad o problema

El equipo necesita:

1. obtener los **K puntos con mayor acumulado** recorriendo el archivo una sola vez;
2. no guardar en memoria más que K puntos a la vez;
3. obtener los **K puntos con menor acumulado** con el mismo código y otro comparador;
4. construir un montículo a partir de una lista ya existente sin insertar uno por uno.

## Datos y campos

**Acumulados del lote de prueba, en milímetros** (el lote es pequeño para poder
revisarlo a mano; el archivo real tiene 6 000 000 de filas):

| Lectura | Punto | Acumulado (mm) |
|---:|---|---:|
| 1 | P01 | 12,4 |
| 2 | P02 | 0,0 |
| 3 | P03 | 35,8 |
| 4 | P04 | 7,2 |
| 5 | P05 | 51,3 |
| 6 | P06 | 4,6 |
| 7 | P07 | 28,9 |
| 8 | P08 | 0,3 |
| 9 | P09 | 65,7 |
| 10 | P10 | 18,1 |
| 11 | P11 | 9,5 |
| 12 | P12 | 42,0 |

El boletín pide los **K = 3** puntos con más lluvia.

## Volumen y crecimiento

El archivo mensual tiene 6 000 000 de filas y el lote de prueba, 12. Con la red actual,
son 240 puntos y 432 lecturas por punto al mes. El plan de expansión lleva la red a 400
puntos y el archivo a 10 000 000 de filas. El costo de la consulta no puede crecer con
el archivo al ritmo de ordenarlo: debe crecer como el número de filas recorridas.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Recorrer el archivo y actualizar los K mayores | una vez por boletín: mensual |
| Consultar el menor de los K mayores | en cada fila: 6 000 000 de veces |
| Construir el montículo desde una lista ya cargada | al cambiar el método de carga |

## Restricciones

- No se puede ordenar el archivo completo: la memoria debe quedarse en K puntos.
- La consulta no puede recorrer el archivo dos veces.
- Recorrer 6 000 000 de filas y hacer una operación de más en cada una se nota: la
  actualización de cada fila debe tocar un solo camino del montículo.

## Pregunta o tarea

Trabaja en parejas y registra tus respuestas en un archivo antes de ejecutar el
programa.

1. Escribe el criterio del montículo para el Top-K: ¿es un montículo de mínimos o de
   máximos? ¿Qué guarda en la raíz y por qué?
2. Reproduce el recorrido del lote con K = 3. Para cada lectura, escribe la lista del
   montículo, los intercambios y, si sale algún punto, cuál.
3. Al terminar, la lista del montículo tiene los 3 mayores, pero **no está ordenada**.
   ¿Por qué? ¿Cómo se obtienen los 3 en orden de mayor a menor?
4. Si en lugar de los 3 mayores se pidieran los 3 **menores**, ¿qué tendría que cambiar:
   el recorrido, el comparador o los dos? Justifica.
5. Construir un montículo de 12 elementos insertando uno por uno cuesta 7
   intercambios; construirlo con el método de Floyd (ajustar hacia abajo desde el
   último nodo interno) cuesta 6. Explica por qué el segundo hace menos trabajo.
6. ¿Cuántas comparaciones hace el recorrido del archivo, en función de n y de K? ¿Por
   qué no es O(n log n)?
7. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la salida.
8. En `MonticuloBinario.cs`, cambia `ConstruirDesde` para que inserte los elementos uno
   por uno en vez de ajustar hacia abajo. Antes de ejecutar, predice si la lista final
   es la misma y si los intercambios cambian. Luego deshaz el cambio.
