# Triaje de pacientes de la sala de urgencias

## Actor y contexto

El Hospital Universitario de Santa Marta atiende en su sala de urgencias a cientos de
pacientes al día. El sistema de triaje asigna a cada paciente un nivel de prioridad
según su estado clínico en el momento de la admisión:

| Nivel | Nombre | Ejemplo |
|---:|---|---|
| 1 | Emergencia vital | infarto, trauma severo |
| 2 | Urgencia alta | fractura expuesta, apendicitis |
| 3 | Urgencia media | fiebre alta, dolor abdominal |
| 4 | Urgencia baja | esguince |
| 5 | Atención general | gripa, control de rutina |

El jefe de urgencias debe despachar siempre al paciente con el **menor nivel de
prioridad** (el 1 primero), sin importar la hora a la que llegó. Una regla adicional,
que exige auditoría de la Secretaría de Salud, desempata: si dos pacientes tienen el
mismo nivel, se atiende primero al que **llegó antes** a la sala. Ya hubo un reclamo
porque un paciente con fractura se atendió antes que otro que llevaba más tiempo
esperando con la misma prioridad.

## Necesidad o problema

La sala de espera necesita una cola que permita:

1. admitir a un paciente en cuanto llega;
2. consultar y llamar, en cualquier momento, al paciente de menor nivel de prioridad;
3. desempatar por orden de llegada.

No se necesita la lista completa ordenada: solo saber cuál es el próximo paciente. Una
lista sin ordenar obliga a revisarla entera en cada llamada, y mantenerla ordenada
obliga a desplazar elementos en cada llegada; con 60 pacientes en espera al mismo
tiempo, eso se nota en la madrugada, cuando llegan más casos.

## Datos y campos

**Eventos de la mañana del 21 de octubre de 2026:**

| N.º | Hora | Evento | Paciente | Motivo | Prioridad |
|---:|---|---|---|---|---:|
| 1 | 08:00 | llega | P01 | apendicitis | 2 |
| 2 | 08:02 | llega | P02 | infarto | 1 |
| 3 | 08:05 | llega | P03 | consulta_general | 5 |
| 4 | 08:07 | llega | P04 | fractura_expuesta | 2 |
| 5 | 08:09 | médico libre | | | |
| 6 | 08:11 | médico libre | | | |
| 7 | 08:13 | llega | P05 | fiebre_alta | 3 |
| 8 | 08:15 | llega | P06 | trauma_severo | 1 |
| 9 | 08:18 | médico libre | | | |
| 10 | 08:20 | médico libre | | | |
| 11 | 08:25 | médico libre | | | |

## Volumen y crecimiento

Una mañana pasan por la sala unas 180 admisiones y nunca hay más de 60 pacientes en
espera al mismo tiempo. El hospital planea abrir un servicio anexo y estima que las
esperas simultáneas pueden llegar a 120.

## Operaciones y frecuencia

| Operación | Frecuencia |
|---|---|
| Admitir un paciente | cada llegada: cientos por mañana |
| Llamar al paciente de menor prioridad | cada vez que un médico queda libre: cientos por mañana |
| Consultar el próximo sin llamarlo | en el tablero de la sala, cada minuto |

## Restricciones

- Admitir y llamar deben costar poco aunque haya 120 pacientes en espera: no se puede
  reordenar toda la sala en cada evento.
- El desempate por orden de llegada es obligatorio.
- Si la sala está vacía cuando un médico queda libre, el sistema debe saberlo sin
  fallar.

## Pregunta o tarea

La estructura de este caso es un montículo binario mínimo guardado en una lista, como
la clase `MonticuloBinario<T>` del proyecto. Trabaja en parejas y registra tus
respuestas en un archivo antes de ejecutar el programa.

1. Escribe el criterio de comparación en una frase, incluido el desempate.
2. Para los eventos 1 a 4 y 7 y 8, escribe la lista después de cada admisión, el árbol
   que representa y el número de intercambios.
3. En el evento 4, el P04 tiene la misma prioridad que el P01, que está en la sala
   desde el evento 1. ¿Se intercambian al admitir al P04? ¿Por qué?
4. Para los eventos 5, 6 y 9 a 11, escribe qué paciente se llama en cada uno, la lista
   resultante y los intercambios. En el evento 6, ¿por qué se llama al P01 y no al P04,
   si ambos tienen prioridad 2?
5. ¿En qué orden se llaman los seis pacientes si al final se vacía la sala?
6. Con 60 y con 120 pacientes en espera, ¿cuántos intercambios puede requerir, como
   máximo, una admisión o una llamada?
7. Ejecuta el proyecto de la carpeta `code/` y compara cada respuesta con la salida.
   Con la opción 2 del menú se avanza un evento a la vez.
8. En `ComparadorDeTriaje.cs`, quita el desempate para que solo compare la prioridad.
   Antes de ejecutar, predice si cambia el orden de atención. Ejecuta, compara y deja
   el comparador como estaba.
