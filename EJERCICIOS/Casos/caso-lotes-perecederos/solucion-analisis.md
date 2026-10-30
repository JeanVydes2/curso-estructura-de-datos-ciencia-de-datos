# Solución de las preguntas de análisis

1. FEFO solo necesita conocer y retirar el lote más urgente mientras siguen llegando y saliendo lotes. Un montículo binario mínimo deja ese lote en la raíz: consultarlo cuesta **O(1)**, y agregar o retirar cuesta **O(log n)** porque el elemento sube o baja, como máximo, una vez por nivel. Una lista ordenada obligaría a desplazar elementos en cada llegada (**O(n)**) y una lista sin ordenar obligaría a recorrerla completa en cada retiro (**O(n)**). Con 80 o 160 lotes en bodega, el montículo evita reordenar toda la estructura en cada movimiento.

2. Va primero el lote con **menos `DaysToExpire`**; si empatan, el de **menor `ArrivalOrder`**. En el proyecto lo hace `LoteComparer`: compara los días y, solo si son iguales, el orden de llegada. El desempate no puede quedar solo en la estructura porque el montículo únicamente garantiza que ningún elemento vaya antes que su padre; entre elementos iguales no conserva ningún orden y los intercambios los mueven sin mirar cuándo llegaron. Si la regla de auditoría importa, tiene que estar en el comparador. Por eso `L02` (4 días, llegó segunda) va antes que `L05` (4 días, llegó quinta) en el estado inicial.

3. `L09` entra al final de la lista (posición 6) y su padre está en la posición `(6 − 1) / 2 = 2`, que es `L03` con 12 días. Como 2 < 12, se intercambian. Ahora `L09` está en la posición 2 y su nuevo padre es `L02`, en la posición 0, con 4 días. Como 2 < 4, vuelve a intercambiarse y queda en la raíz. Fueron **2 intercambios**; en el movimiento 5 `L09` es el primer lote retirado.

4. La altura de un montículo con `n` elementos es `⌊log₂ n⌋`, y una inserción o un retiro hace como máximo un intercambio por nivel.

   | Lotes en bodega | Altura | Intercambios máximos por inserción o retiro |
   |---:|---:|---:|
   | 80 | 6 | 6 |
   | 160 | 7 | 7 |

   Duplicar la cantidad de lotes agrega un solo nivel, así que el costo crece de forma logarítmica y no lineal.

5. La aplicación debe comprobar `IsEmpty` antes de retirar, informar que no hay lotes disponibles y continuar con el siguiente movimiento. En la solución se imprime `retiro omitido porque el montículo está vacío`. Si se llama `ExtractRoot` sin comprobarlo, el montículo lanza `InvalidOperationException` porque no hay raíz que sacar; lo mismo ocurre con `Peek`. El caso pide que un retiro sin existencias no tumbe la jornada.

6. Sin desempate, `DiasComparer` devuelve 0 cuando dos lotes tienen los mismos días. En el movimiento 5, después de retirar `L09`, `L03` baja desde la raíz y sus hijos son `L05` y `L02`, ambos con 4 días. El método `SiftDown` compara primero al hijo izquierdo: como `L05` no es estrictamente menor que `L02`, el empate no provoca un nuevo intercambio y `L05` sube antes que `L02`. Por eso el orden cambia de `... L09, L02, L05 ...` a `... L09, L05, L02 ...`. El riesgo operativo es que se retira un lote más nuevo mientras otro con la misma fecha de vencimiento sigue esperando: se incumple la regla de auditoría, el lote más antiguo permanece más tiempo en bodega y aumenta la probabilidad de merma o de una queja como la que ya recibió la cadena.
