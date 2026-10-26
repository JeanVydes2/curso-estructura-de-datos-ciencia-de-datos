# Análisis y solución: árboles de expresión para la liquidación de tarifas

## Por qué es un árbol binario

Una operación siempre recibe dos operandos: el izquierdo y el derecho. Las hojas
son los números de la fórmula y los nodos internos son las operaciones. La forma
del árbol es la que fija el orden de cálculo: `2.5 8 *` y `0.5 4 *` se resuelven
antes de sumarlos, porque cuelgan de la suma.

## 1. Construcción del convenio 1

Notación en sufijo: `2.5 8 * 0.5 4 * + 6 -`.

| Token | Acción | Pila (abajo → arriba) |
|---|---|---|
| `2.5` | apila la hoja 2.5 | 2.5 |
| `8` | apila la hoja 8 | 2.5, 8 |
| `*` | desapila 8 y 2.5, apila `(2.5 * 8)` | (2.5 * 8) |
| `0.5` | apila la hoja 0.5 | (2.5 * 8), 0.5 |
| `4` | apila la hoja 4 | (2.5 * 8), 0.5, 4 |
| `*` | desapila 4 y 0.5, apila `(0.5 * 4)` | (2.5 * 8), (0.5 * 4) |
| `+` | desapila los dos y apila la suma | ((2.5 * 8) + (0.5 * 4)) |
| `6` | apila la hoja 6 | ((2.5 * 8) + (0.5 * 4)), 6 |
| `-` | desapila 6 y la suma, apila la resta | (((2.5 * 8) + (0.5 * 4)) − 6) |

Árbol final:

```text
                          (−)
                 ┌─────────┴─────────┐
                (+)                  6
          ┌──────┴──────┐
        (*)            (*)
      ┌──┴──┐        ┌──┴──┐
     2.5    8      0.5    4
```

## 2. Las tres escrituras del convenio 1

| Recorrido | Escritura | Cuándo aparece |
|---|---|---|
| Preorden | `− + * 2.5 8 * 0.5 4 6` | prefijo; el operador va antes de sus operandos |
| Inorden | `2.5 * 8 + 0.5 * 4 − 6` | infijo; es la fórmula "natural", pero sin paréntesis cambia el resultado |
| Postorden | `2.5 8 * 0.5 4 * + 6 −` | sufijo; coincide con la notación de entrada |

El sufijo coincide con la entrada porque la construcción apilando operandos y
resolviendo operadores deja los subárboles ya cerrados cuando el recorrido en
postorden los vuelve a encontrar. Es la razón por la que la notación sufija se
presta para una calculadora de pila.

## 3. Cálculo de la tarifa del convenio 1

Se recorre en postorden y cada operador combina los valores ya calculados de sus
dos hijos:

| Subárbol | Valor |
|---|---|
| `2.5 * 8` | 20 |
| `0.5 * 4` | 2 |
| `20 + 2` | 22 |
| `22 − 6` | 16 |

La tarifa es **16**.

## 4. Convenio 2

Notación: `5 3 + 4 *`.

| Token | Acción | Pila |
|---|---|---|
| `5` | apila | 5 |
| `3` | apila | 5, 3 |
| `+` | desapila 3 y 5, apila `(5 + 3)` | (5 + 3) |
| `4` | apila | (5 + 3), 4 |
| `*` | desapila 4 y la suma, apila el producto | ((5 + 3) * 4) |

```text
                    (*)
              ┌──────┴──────┐
             (+)             4
           ┌──┴──┐
          5      3
```

| Recorrido | Escritura |
|---|---|
| Preorden | `* + 5 3 4` |
| Inorden | `5 + 3 * 4` |
| Postorden | `5 3 + 4 *` |

`5 + 3` vale 8 y `8 * 4` vale 32: la tarifa es **32**.

La diferencia entre los dos convenios es la profundidad: el convenio 1 suma dos
productos antes de restar y tiene altura 3; el convenio 2 concentra la operación
en la raíz y tiene altura 2. El convenio 3, `10 2 3 * +`, es `10 + (2 * 3)`.

## 5. Métricas

| Medida | Convenio 1 | Convenio 2 | Convenio 3 |
|---|---:|---:|---:|
| Nodos | 9 | 5 | 5 |
| Hojas (números) | 5 | 3 | 3 |
| Operadores (internos) | 4 | 2 | 2 |
| Altura | 3 | 2 | 2 |
| Ancho (nodos en el nivel más poblado) | 4 | 2 | 2 |

En los tres casos hojas = internos + 1 (5 = 4 + 1, 3 = 2 + 1), porque las fórmulas
del caso son árboles llenos: ningún operador tiene un solo operando. La altura
crece con el encadenamiento de las operaciones; el ancho, con las que se pueden
calcular en paralelo.

## 6. Por qué el cálculo es postorden

Un operador no puede resolverse antes de conocer el valor de sus dos hijos, así que
sus hijos se visitan primero: izquierdo, derecho y luego el nodo. El infijo de un
árbol de expresión, sin paréntesis, no conserva la forma: en el convenio 1, el
infijo es `2.5 * 8 + 0.5 * 4 − 6`, y un lector que aplique la multiplicación antes
que la suma obtiene 22 − 6 = 16, que aquí coincide; pero en el convenio 2 el infijo
`5 + 3 * 4` valdría 17 y no 32. Para que el infijo sea fiel hay que escribir cada
subárbol entre paréntesis, que es lo que hace la opción de salida del programa.

## 7. Contraste con el programa

La opción `--demo` (o el menú) imprime, para cada convenio, las cuatro escrituras,
las métricas y el valor. Las pruebas de `tests/` comprueban cada uno de estos
resultados y el rechazo de las notaciones mal formadas.

## 8. Una notación mal formada

Con la entrada `4 *`, el primer token apila 4; el `*` necesita dos operandos y solo
hay uno, así que el constructor se detiene con el mensaje *"El operador * no tiene
dos operandos."*. Es el token que falta, no un error de la fórmula.

## Errores frecuentes

- Leer el prefijo de derecha a izquierda: el prefijo se lee de izquierda a derecha,
  y el sufijo también.
- Desapilar el operando izquierdo antes que el derecho. En el sufijo, el primer
  operando que sale de la pila es el **derecho**; invertirlos cambia el resultado en
  la resta y en la división.
- Suponer que el infijo es evaluable tal cual. Solo lo es si se agregan los
  paréntesis de cada subárbol.
- Contar la altura en nodos en lugar de aristas.
