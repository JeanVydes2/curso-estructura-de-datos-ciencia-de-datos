# Clasificación de solicitudes de beca

## Actor y contexto

La Universidad del Magdalena recibe cada semestre unas 3 400 solicitudes de beca
de estudiantes de pregrado. Una persona del área de Bienestar Universitario revisa
cada solicitud y decide qué apoyo se asigna: beca completa, media beca, apoyo de
transporte o rechazo.

Hoy la decisión está escrita como un bloque de `if` anidados dentro de una hoja de
cálculo. Cada vez que el comité cambia un requisito hay que leer el bloque
completo, y dos analistas que revisan la misma solicitud no siempre llegan al
mismo apoyo. El comité exportó las reglas a una tabla de configuración: cada fila
es una pregunta con dos salidas (sí o no) o un apoyo final. Con esa tabla hay que
reconstruir la estructura de decisión, comprobar que esté bien formada y usarla
para clasificar las solicitudes del semestre.

Una decisión equivocada tiene consecuencias concretas: si una solicitud con
promedio bajo recibe beca completa, se agota un cupo que necesitaba otro
estudiante; si una solicitud que cumplía todos los requisitos se rechaza, el
estudiante pierde el semestre.

## Necesidad o problema

El equipo de Bienestar necesita:

1. construir la estructura de decisión a partir de la tabla, encontrando la
   pregunta inicial sin que nadie la indique;
2. detectar tablas mal formadas: salidas que apuntan a reglas inexistentes, reglas
   con dos entradas o más de una pregunta inicial;
3. clasificar cada solicitud siguiendo las respuestas a las preguntas;
4. documentar las reglas para la auditoría del comité y saber qué condiciones
   llevan a cada apoyo.

## Datos y campos

**Tabla de reglas.** Las filas están en el orden en que salieron de la herramienta
de configuración, no en el orden en que se evalúan. Convención del comité: la
salida **Sí** es el hijo izquierdo y la salida **No**, el derecho.

| Id | Tipo | Texto | Sí → | No → |
|---|---|---|---|---|
| R2 | Pregunta | ¿El promedio acumulado es al menos 3,8? | R4 | R1 |
| D6 | Destino | Rechazar: cupos agotados para el segundo periodo | | |
| R1 | Pregunta | ¿La solicitud llegó antes del cierre de convocatoria? | R3 | D6 |
| D1 | Destino | Asignar beca completa | | |
| R4 | Pregunta | ¿El estrato del hogar es 1 o 2? | D2 | R5 |
| R3 | Pregunta | ¿Aprobó al menos el 80 % de los créditos? | D3 | D5 |
| D2 | Destino | Asignar beca completa y apoyo de transporte | | |
| R5 | Pregunta | ¿La familia está registrada en el Sisbén? | D1 | D4 |
| D3 | Destino | Asignar media beca | | |
| D4 | Destino | Asignar apoyo de transporte | | |
| D5 | Destino | Asignar media beca condicionada a matrícula de honor | | |

**Solicitudes del semestre 2026-2** (datos del sistema académico):

| Orden de llegada | Solicitud | Promedio | Créditos aprobados | Estrato | Sisbén | Llegó antes del cierre |
|---:|---|---:|---:|---:|---|---|
| 1 | S-1042 | 3,4 | 76 % | 3 | sí | no |
| 2 | S-1043 | 4,2 | 92 % | 1 | sí | sí |
| 3 | S-1044 | 4,0 | 85 % | 3 | sí | sí |
| 4 | S-1045 | 3,9 | 70 % | 2 | sí | sí |
| 5 | S-1046 | 3,2 | 90 % | 2 | sí | sí |
| 6 | S-1047 | 3,6 | 65 % | 2 | no | sí |
| 7 | S-1048 | 4,5 | 88 % | 3 | no | sí |

## Volumen y crecimiento

Hoy hay 11 reglas y llegan 3 400 solicitudes por semestre. Con dos programas
nuevos, el comité espera unas 20 reglas y 5 000 solicitudes en dos años. La
cantidad de solicitudes crece más rápido que la de reglas: lo que pesa es cuántas
preguntas responde cada solicitud, no cuántas reglas haya en la tabla.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Construir y validar la estructura desde la tabla | cada vez que cambia la tabla (unas dos veces al año) |
| Clasificar una solicitud | una vez por solicitud: 3 400 por semestre |
| Listar las reglas para auditoría, de la primera hacia abajo | una vez por semestre |
| Contar las reglas por nivel de profundidad | al revisar un cambio de reglas |
| Obtener las condiciones que llevan a un apoyo | cuando un estudiante apela un rechazo |

## Restricciones

- Una tabla mal formada no puede usarse: el proceso debe detenerse y decir qué
  fila falla.
- La convención Sí → izquierda, No → derecha es fija, porque las condiciones de un
  apoyo se leen siguiendo esas posiciones.
- Cada regla es salida de una sola pregunta, y la estructura debe describir un
  árbol, no un grafo.

## Pregunta o tarea

Trabaja en parejas y registra tus respuestas en un archivo antes de ejecutar el
programa.

1. ¿Cuál es la regla inicial? Explica cómo la identificas usando solo la tabla.
2. Dibuja el árbol con la convención Sí a la izquierda y No a la derecha.
3. Calcula nodos, aristas, hojas, nodos internos y altura. ¿Es un árbol lleno? ¿Se
   cumple que hojas = internos + 1?
4. Escribe el preorden, el inorden, el postorden y el recorrido por niveles. ¿Cuál
   sirve para listar las reglas de la primera hacia abajo?
5. Para cada solicitud del semestre, indica el apoyo y cuántas preguntas responde.
   ¿Cuál es el máximo posible de preguntas para cualquier solicitud y de qué
   depende?
6. Un estudiante apela el apoyo D5. ¿Qué respuestas llevan a ese apoyo?
7. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la
   salida. Usa la opción 5 del menú para seguir una solicitud paso a paso.
8. En `DatosDelCaso.cs`, cambia la salida Sí de R1 por `R5` en vez de `R3`. Antes
   de ejecutar, predice qué error detecta el constructor y por qué esa tabla ya no
   describe un árbol. Luego deshaz el cambio.
