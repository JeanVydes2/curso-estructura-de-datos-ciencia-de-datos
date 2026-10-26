# Análisis y solución: triaje de pacientes de la sala de urgencias

## Por qué un montículo

La sala solo necesita conocer y llamar al paciente más urgente mientras siguen
llegando casos. Una lista ordenada obliga a desplazar elementos en cada llegada
(O(n)), y una lista sin ordenar obliga a revisarla completa en cada llamada (O(n)). Un
montículo binario mínimo deja al paciente más urgente en la raíz: consultarlo cuesta
O(1), y admitir o llamar cuesta O(log n), porque cada intercambio sube o baja un
nivel.

## 1. Criterio de comparación

Va primero el paciente con **menor nivel de prioridad**; si empatan, el que tiene
**menor orden de llegada**. En el proyecto es `ComparadorDeTriaje`: compara `Prioridad`
y, solo si son iguales, `OrdenDeLlegada`.

## 2. Eventos 1 a 4, 7 y 8 (admisiones)

Cada paciente entra al final de la lista y sube mientras vaya antes que su padre, que
está en la posición `(i − 1) / 2`.

| N.º | Paciente | Lista después (`Id:prioridad`) | Intercambios |
|---:|---|---|---:|
| 1 | P01 | [P01:2] | 0 |
| 2 | P02 | [P02:1, P01:2] | 1 |
| 3 | P03 | [P02:1, P01:2, P03:5] | 0 |
| 4 | P04 | [P02:1, P01:2, P03:5, P04:2] | 0 |
| 7 | P05 | [P04:2, P03:5, P05:3] | 0 |
| 8 | P06 | [P06:1, P04:2, P05:3, P03:5] | 2 |

Entre el evento 4 y el 7 hay dos llamadas (eventos 5 y 6), que se resuelven en el punto
4. Después del evento 8, la lista queda [P06:1, P04:2, P05:3, P03:5] y el árbol es:

```text
                    P06:1
           ┌──────────┴──────────┐
        P04:2                P05:3
       ┌──┘
    P03:5
```

La lista no está ordenada (P03:5 aparece después de P05:3), pero cada padre va antes
que sus hijos.

## 3. El empate del evento 4

El P04 entra en la posición 3; su padre está en la posición (3 − 1) / 2 = 1 y es el
P01, con prioridad 2. El P04 también tiene prioridad 2, así que el comparador mira el
orden de llegada: el P01 llegó primero (orden 1) y el P04 después (orden 4). El P04
**no** va antes que su padre, así que no se intercambian y el evento 4 tiene 0
intercambios. Sin el desempate, esta comparación daría 0, se trataría como "no va
antes" igual, y el resultado coincidiría por casualidad; donde el desempate cambia el
resultado es en la llamada del evento 6, porque la subida y la bajada mueven a los
pacientes.

## 4. Eventos 5, 6 y 9 a 11 (llamadas)

En cada llamada sale la raíz, el último paciente de la lista pasa a la raíz y baja por
el hijo que va primero.

| N.º | Llama | Baja | Lista después | Intercambios |
|---:|---|---|---|---:|
| 5 | P02 | P04 | [P01:2, P04:2, P03:5] | 1 |
| 6 | P01 | P03 | [P04:2, P03:5] | 1 |
| 9 | P06 | P03 | [P04:2, P03:5, P05:3] | 1 |
| 10 | P04 | P05 | [P05:3, P03:5] | 0 |
| 11 | P05 | P03 | [P03:5] | 0 |

El evento 6 muestra el desempate: al llamar al P02, la última de la lista (el P04)
pasa a la raíz y baja. Sus hijos son el P01:2 y el P03:5, así que baja por el P01. En
la comparación entre el P04 y el P01, ambos tienen prioridad 2, y el desempate decide:
el P01 llegó antes (orden 1) que el P04 (orden 4), así que el P01 queda en la raíz y se
llama en el evento 6. Sin el desempate, el P04 se quedaría en la raíz y se llamaría a
él primero, justo el reclamo de la Secretaría.

## 5. Orden completo de atención

P02, P01, P06, P04, P05, P03.

Es el orden por prioridad (1, 2, 1, 2, 3, 5), salvo que el P06 llegó después de que se
atendiera al P02 y al P01. El empate de prioridad 2 entre el P01 y el P04 se resolvió
por llegada: el P01 antes que el P04.

## 6. Costo con 60 y 120 pacientes

La forma completa fija la altura: ⌊log₂ n⌋.

| En espera | Altura | Intercambios máximos por admisión o llamada |
|---:|---:|---:|
| 60 | 5 | 5 |
| 120 | 6 | 6 |

Duplicar los pacientes en espera agrega un solo nivel. El crecimiento planeado no
cambia el costo de forma apreciable.

## 7. Contraste con el programa

`dotnet run --project src/TriajeUrgencias.csproj -- --demo` imprime, para cada evento,
el árbol, la lista con índices, el paciente resaltado y los intercambios. Las tablas
anteriores coinciden con esa salida, y las pruebas de `tests/` comprueban la lista
después del evento 8, el empate, las cinco llamadas de la mañana y el orden completo.

## 8. Sin desempate

Si el comparador solo mira la prioridad, el orden de atención cambia a
P02, **P04, P06, P01**, P05, P03: el P04, que llegó después con la misma prioridad 2,
se atiende antes que el P01. Es justo el reclamo que recibiría la Secretaría. El
montículo no conserva el orden de llegada entre iguales, porque los intercambios mueven
a los pacientes sin mirar cuándo llegaron. Si el negocio exige un desempate, tiene que
estar en el comparador.

## Errores frecuentes

- Comparar el paciente nuevo con su hermano en lugar de con su padre.
- Bajar por cualquier hijo en lugar del que va primero según el comparador.
- Suponer que la lista queda ordenada después de cada operación.
- Intercambiar cuando el comparador devuelve 0: los intercambios solo ocurren si el
  hijo va estrictamente antes.
- Llamar a un paciente de una sala vacía sin controlarlo: `ExtraerRaiz` lanza
  `InvalidOperationException`.
