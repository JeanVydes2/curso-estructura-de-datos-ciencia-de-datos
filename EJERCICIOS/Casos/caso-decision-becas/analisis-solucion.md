# Análisis y solución: clasificación de solicitudes de beca

## Por qué es un árbol binario

Cada pregunta tiene exactamente dos salidas con nombre, Sí y No, y cada regla es
salida de una sola pregunta. Eso es un árbol binario: la pregunta es un nodo
interno, la salida Sí es el hijo izquierdo, la salida No es el derecho y los
apoyos son hojas. La posición importa: si se intercambian los hijos de R4, una
solicitud de estrato 3 iría a beca completa y transporte.

## 1. La regla inicial

La raíz es la única regla que no aparece en las columnas Sí → y No →. Si se
revisan esas dos columnas, aparecen todas las reglas menos **R2**. El programa hace
lo mismo: crea un nodo por fila, conecta las salidas y anota cuáles reglas tienen
padre. La que queda sin padre es la raíz.

## 2. El árbol

```text
                                  R2
                     ┌────────────┴────────────┐
                    R4                         R1
             ┌───────┴───────┐          ┌──────┴──────┐
            D2              R5         R3             D6
                        ┌───┴───┐   ┌──┴──┐
                       D1      D4  D3    D5
```

## 3. Métricas

| Medida | Valor | Razón |
|---|---|---|
| Nodos | 11 | una fila por nodo |
| Aristas | 10 | n − 1: cada nodo, salvo la raíz, tiene una arista hacia su padre |
| Hojas (apoyos) | 6 | D1 a D6 |
| Nodos internos (preguntas) | 5 | R1 a R5 |
| Altura | 3 | camino R2 → R4 → R5 → D1, medido en aristas |
| ¿Lleno? | sí | cada pregunta tiene 0 o 2 hijos; ninguna tiene uno solo |

Como el árbol es lleno, se cumple hojas = internos + 1: 6 = 5 + 1.

## 4. Recorridos

| Recorrido | Orden | Uso en el caso |
|---|---|---|
| Preorden | R2, R4, D2, R5, D1, D4, R1, R3, D3, D5, D6 | listar las reglas para auditoría: cada pregunta aparece antes de sus salidas |
| Inorden | D2, R4, D1, R5, D4, R2, D3, R3, D5, R1, D6 | es el orden de izquierda a derecha del dibujo; en este caso no responde una pregunta del negocio |
| Postorden | D2, D1, D4, R5, R4, D3, D5, R3, D6, R1, R2 | cada pregunta aparece después de sus dos salidas; sirve para cálculos que dependen de los hijos, como la altura |
| Por niveles | R2 / R4, R1 / D2, R5, R3, D6 / D1, D4, D3, D5 | cuántas reglas hay a cada profundidad: 1, 2, 4 y 4 |

Para documentar las reglas de la primera hacia abajo se usa el **preorden**.

## 5. Clasificación de las solicitudes

| Solicitud | Respuestas | Apoyo | Preguntas |
|---|---|---|---|
| S-1042 | R2 no (3,4), R1 no | D6 | 2 |
| S-1043 | R2 sí (4,2), R4 sí (estrato 1) | D2 | 2 |
| S-1044 | R2 sí (4,0), R4 no, R5 sí | D1 | 3 |
| S-1045 | R2 sí (3,9), R4 sí (estrato 2) | D2 | 2 |
| S-1046 | R2 no, R1 sí, R3 sí (90 %) | D3 | 3 |
| S-1047 | R2 no, R1 sí, R3 no (65 %) | D5 | 3 |
| S-1048 | R2 sí (4,5), R4 no, R5 no | D4 | 3 |

Cada solicitud baja un nivel por pregunta, así que el máximo de preguntas es la
**altura** del árbol: 3. No depende de cuántas reglas tenga la tabla, sino de qué
tan largo es el camino más profundo. Si las reglas crecen a 20 y se agregan en
cadena, la altura crece con ellas; si se reparten en ramas, crece mucho menos.

## 6. Condiciones para llegar a D5

El camino desde la raíz es R2 → R1 → R3: el promedio **no** alcanza 3,8 **y** la
solicitud llegó antes del cierre **y** aprobó menos del 80 % de los créditos. Para
encontrarlo se recorre el árbol en profundidad guardando el camino: se agrega cada
nodo al bajar y se quita si su rama no contiene D5.

## 7. Contraste con el programa

La opción `--demo` (o el menú) produce exactamente las respuestas anteriores. Las
pruebas de `tests/` comprueban la raíz, las métricas, los cuatro recorridos, el
apoyo y la cantidad de nodos visitados de cada solicitud, y el camino hacia D5.

## 8. Una tabla mal formada

Si la salida Sí de R1 apunta a R5, entonces R5 es salida de R4 y también de R1:
tendría dos padres. El constructor se detiene con el mensaje *"La regla R5 tiene
dos padres."*. Además, R3, D3 y D5 quedarían sin camino desde la raíz. Esa tabla
describe un grafo, no un árbol.

## Errores frecuentes

- Buscar la raíz en la primera fila de la tabla. La tabla no está ordenada; la
  raíz se reconoce porque no es salida de nadie.
- Intercambiar Sí y No al conectar. El árbol sigue siendo válido, pero manda las
  solicitudes al apoyo contrario.
- Contar la altura en nodos (4) en lugar de aristas (3).
- Pensar que el costo de clasificar una solicitud depende del número total de
  reglas. Depende del camino que recorre: a lo sumo, altura + 1 nodos.
