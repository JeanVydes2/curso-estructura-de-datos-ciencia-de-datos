# Consultas de orden sobre el índice de precios de la lonja

## Actor y contexto

La Lonja de Santa Marta publica cada mañana el precio de referencia por kilogramo de
los productos del mercado. Un equipo de ingeniería de datos mantiene un servicio que
responde preguntas de compradores y de la secretaría de agricultura a partir de esos
precios:

- un restaurante quiere el producto **más caro que no supere** el presupuesto del día;
- una cooperativa quiere el **más barato que alcance** un precio mínimo de venta;
- la secretaría quiere el **k-ésimo producto más barato** para seguir la canasta;
- quiere saber **cuántos productos cuestan menos** que un valor de referencia.

Los precios se registran a medida que llegan los lotes, así que el orden de llegada no
sigue el orden de los precios. El servicio necesita responder esas cuatro consultas
sin ordenar la lista completa en cada llamada.

## Necesidad o problema

El equipo necesita un índice de precios en memoria que permita:

1. registrar un precio nuevo sin reconstruir el índice;
2. obtener el **piso** de un valor: el mayor precio que no lo supera;
3. obtener el **techo** de un valor: el menor precio que lo alcanza o lo supera;
4. obtener el **k-ésimo precio más barato**;
5. contar **cuántos precios hay por debajo** de un valor dado.

Un compañero propone guardar los precios en un arreglo y, en cada consulta, ordenarlo
y recorrerlo. Funciona para 12 productos, pero con el catálogo completo del mercado
(cientos de productos y miles de consultas al día) ordenar cada vez es desperdiciar
trabajo: la respuesta se puede obtener recorriendo un solo camino del árbol, si cada
nodo conoce **cuántos nodos hay en su subárbol**.

## Datos y campos

**Precios registrados esta semana, en el orden en que llegaron los lotes:**

| Orden | Producto | Precio (COP/kg) |
|---:|---|---:|
| 1 | Café pergamino | 35 000 |
| 2 | Plátano | 12 000 |
| 3 | Cacao | 48 000 |
| 4 | Yuca | 8 000 |
| 5 | Ají | 27 000 |
| 6 | Aguacate Hass | 55 000 |
| 7 | Mango | 19 000 |
| 8 | Camarón | 62 000 |
| 9 | Ñame | 3 000 |
| 10 | Panela | 41 000 |
| 11 | Ahuyama | 23 000 |
| 12 | Queso costeño | 50 000 |

**Consultas del día:**

1. Piso y techo de 30 000 (presupuesto del restaurante).
2. Piso de 3 000 y techo de 62 000 (los extremos del catálogo).
3. Piso de 1 000 y techo de 70 000 (fuera del catálogo).
4. El 1.º, el 6.º, el 7.º y el 12.º precio más barato.
5. Cuántos productos cuestan menos de 41 000.

## Volumen y crecimiento

Hoy hay 12 productos y se registran unos 30 precios nuevos al día. El servicio atiende
unos 2 000 consultas diarias. La secretaría planea cubrir todo el mercado del
Magdalena: unos 400 productos y 50 000 consultas diarias. El costo de una consulta no
puede crecer al ritmo del catálogo: debe recorrer un solo camino.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Registrar un precio | decenas por día |
| Piso y techo de un valor | cada consulta de comprador: miles por día |
| K-ésimo más barato | para el reporte de la canasta: diario |
| Cuántos precios hay por debajo de un valor | al publicar el índice: diario |

## Restricciones

- Registrar un precio y consultar el índice no pueden reordenar todo el catálogo:
  deben costar como la altura del árbol.
- Los precios son la clave y no se repiten: un producto con un precio ya registrado se
  rechaza, porque dos productos no comparten precio de referencia.
- Cada nodo debe conocer cuántos nodos tiene en su subárbol, para que el k-ésimo y el
  conteo por debajo no recorran todo el árbol.

## Pregunta o tarea

Trabaja en parejas y registra tus respuestas en un archivo antes de ejecutar el
programa.

1. Inserta los 12 precios en el orden de llegada y dibuja el árbol resultante, con el
   precio como clave. ¿Cuál es la raíz y cuál es la altura?
2. Anota, debajo de cada nodo, cuántos nodos tiene su subárbol (incluyéndolo). ¿Coincide
   ese número con el tamaño del subárbol, o con la altura?
3. Responde las consultas del día: piso y techo de 30 000, piso de 3 000, techo de
   62 000, piso de 1 000 y techo de 70 000. Cuando una consulta no tiene respuesta,
   explica por qué.
4. Encuentra el 1.º, el 6.º, el 7.º y el 12.º precio más barato. Describe, para el
   7.º, cómo deciden los tamaños de los subárboles si bajar a la izquierda o a la
   derecha.
5. ¿Cuántos productos cuestan menos de 41 000? ¿Es lo mismo que el rango de una
   consulta anterior?
6. Explica por qué, sin el tamaño en cada nodo, el k-ésimo más barato costaría O(n) y
   el conteo por debajo también, y con el tamaño cuestan O(h).
7. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la salida.
8. En `ArbolDePrecios.cs`, haz que `Insertar` **no** actualice el tamaño del nodo. Antes
   de ejecutar, predice qué consultas dan un resultado equivocado y por qué el árbol
   sigue siendo un árbol de búsqueda válido pero el k-ésimo falla. Luego deshaz el
   cambio.
