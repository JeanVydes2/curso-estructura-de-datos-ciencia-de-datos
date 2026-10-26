# Casos de aplicación: árboles y montículos

Seis casos de aplicación, dos por cada estructura no lineal de la unidad
([Árboles](../../03_NO_LINEALES/01_ARBOLES.md) y
[Heaps](../../03_NO_LINEALES/02_HEAPS.md)): árbol binario, árbol binario de
búsqueda y montículo binario.

Los ejercicios de [04_EJERCICIOS_ARBOLES.md](../04_EJERCICIOS_ARBOLES.md) y
[05_EJERCICIOS_HEAPS.md](../05_EJERCICIOS_HEAPS.md) plantean el problema y una
plantilla en un solo archivo. Estos casos dan el siguiente paso: el mismo tipo de
problema, con datos y preguntas, y un proyecto de C# que construye la estructura,
la dibuja en la consola y la muestra después de cada operación, con pruebas
automáticas.

| Carpeta | Estructura | Situación |
|---|---|---|
| `caso-decision-becas/` | árbol binario | reglas de dos salidas que clasifican solicitudes de beca |
| `caso-expresiones/` | árbol binario | expresiones aritméticas: construir, recorrer y evaluar |
| `caso-catalogo-estaciones/` | árbol binario de búsqueda | catálogo de estaciones de la red hidrometeorológica del Magdalena |
| `caso-consultas-rango/` | árbol binario de búsqueda | consultas por rango de caudal: piso, techo y k-ésimo menor |
| `caso-triage-urgencias/` | montículo binario | sala de urgencias: prioridad con desempate por llegada |
| `caso-top-k/` | montículo binario | los K pluviómetros con más lluvia de un archivo de un millón de filas |

Cada carpeta contiene:

- `caso.md`: el enunciado, con los datos y las preguntas;
- `analisis-solucion.md`: las respuestas, para revisar después de intentar el caso;
- `code/`: la solución en .NET 10, con pruebas xUnit. Su `README.md` explica cómo
  compilarla, ejecutarla y probarla.

Antes de ejecutar un proyecto, responde las preguntas del caso y dibuja la
estructura. Después ejecuta el programa y compara su salida con tus respuestas.

## Cómo ejecutar un caso

Cada `code/` es una solución independiente:

```bash
cd caso-decision-becas/code
dotnet run --project src/DecisionBecas.csproj -- --demo
dotnet test tests/DecisionBecasTests.csproj
```

Sin `--demo`, el programa muestra un menú interactivo para avanzar paso a paso.
