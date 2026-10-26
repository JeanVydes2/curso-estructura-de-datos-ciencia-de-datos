# Árboles de expresión para la liquidación de tarifas

## Actor y contexto

Ribera Logística opera una flota de camiones que transporta carga entre Santa
Marta, Barranquilla y Valledupar. Por cada despacho, un asesor comercial negocia
una tarifa que combina un precio base por tonelada, un recargo por distancia, un
recargo por tipo de carga y descuentos por cliente frecuente.

Hoy las tarifas se calculan con una fórmula escrita directamente en el código. Cada
convenio nuevo obliga a un programador a cambiar la fórmula, y no hay forma de
revisar una tarifa sin contratar un despacho ni de explicarle al cliente de dónde
sale el valor. La empresa quiere representar cada fórmula como un **árbol de
expresión**: los números y las tarifas base son hojas, y cada operación (suma,
resta, multiplicación, división) es un nodo interno con dos hijos. La fórmula se
reconstruye a partir de la notación y se recorre para obtener las tres escrituras
usuales y el valor de la tarifa.

## Necesidad o problema

El equipo necesita:

1. construir el árbol de una tarifa a partir de su notación, sin conocer de
   antemano la forma de la fórmula;
2. rechazar notaciones mal formadas: tokens de más, tokens de menos u operadores
   sin sus dos operandos;
3. obtener la escritura en prefijo (preorden), en infijo (inorden) y en sufijo
   (postorden);
4. calcular la tarifa recorriendo el árbol en postorden, y saber cuántos niveles
   tiene la fórmula y qué tan ancha es.

## Datos y campos

**Fórmulas de los convenios.** Las notaciones están en **sufijo** (postfijo),
porque es la forma en que un operario las digita con una calculadora de pila.

**Convenio 1 — tarifa de despacho urbano:**

```text
2.5 8 * 0.5 4 * +  schemas 6 -
```

Tokens, en orden: `2.5`, `8`, `*`, `0.5`, `4`, `*`, `+`, `6`, `-`.

**Convenio 2 — tarifa de despacho intermunicipal:**

```text
5 3 + 4 *
```

Tokens, en orden: los cinco anteriores.

**Convenio 3 — tarifa con descuento por cliente frecuente:**

```text
10 2 3 * +
```

**Notaciones mal formadas** (para probar el rechazo):

- `3 +` → un operador sin sus dos operandos;
- `3 4 5 +` → sobran operandos: al final la pila no queda en un solo árbol.

## Volumen y crecimiento

Hay 6 convenios vigentes y se cotizan unos 400 despachos al día. Cada cotización
recorre su árbol y lo muestra al cliente. Con la apertura de dos rutas nuevas, la
empresa espera 20 convenios y 1 000 cotizaciones diarias. El trabajo de recorrer
una fórmula depende de cuántos nodos tenga, no de cuántos convenios haya.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Construir el árbol desde la notación | al cambiar un convenio (unas cuatro veces al año) |
| Calcular la tarifa de un despacho | una vez por despacho: 400 al día |
| Explicar la fórmula al cliente (prefijo, infijo y sufijo) | cada cotización |
| Consultar la altura y el ancho de la fórmula | al revisar un convenio |

## Restricciones

- Una notación mal formada no puede usarse: el proceso debe detenerse y decir qué
  falta.
- En las fórmulas del caso un operador nunca tiene un solo hijo; la suma, la resta,
  la multiplicación y la división siempre reciben dos operandos.
- El cálculo debe hacerse recorriendo los hijos antes que el nodo, porque una
  operación no puede resolverse antes de conocer sus dos valores.

## Pregunta o tarea

Trabaja en parejas y registra tus respuestas en un archivo antes de ejecutar el
programa.

1. Construye el árbol del convenio 1 paso a paso: por cada token, indica qué se
   apila y qué se desapila. Dibuja el árbol final.
2. Escribe el prefijo, el infijo y el sufijo del árbol del convenio 1. ¿Coincide el
   sufijo con la notación de entrada? ¿Por qué?
3. Calcula la tarifa del convenio 1 a mano, mostrando el valor de cada subárbol.
4. Repite los puntos 1, 2 y 3 con el convenio 2. ¿Qué diferencia hay en la forma
   del árbol entre los dos convenios?
5. ¿Cuántos nodos, hojas, operadores y niveles tiene cada fórmula? ¿Cuál es la
   altura y el ancho de cada una?
6. Explica por qué el cálculo de la tarifa es un recorrido en postorden y por qué el
   infijo del árbol no sirve para evaluar la expresión sin agregar paréntesis.
7. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la salida.
   Usa la opción 1 del menú para ver los mismos tokens en las cuatro escrituras.
8. En `DatosDelCaso.cs`, cambia la notación del convenio 2 por `4 *`. Antes de
   ejecutar, predice qué falta y cómo lo detecta el constructor. Luego deshaz el
   cambio.
