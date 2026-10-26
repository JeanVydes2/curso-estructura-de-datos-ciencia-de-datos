# TriajeUrgencias

## Objetivo

Solución de referencia del caso del triaje de la sala de urgencias. El programa
procesa los eventos de la mañana del caso sobre un montículo binario mínimo
(`MonticuloBinario<Paciente>`) guardado en una `List<T>`. El orden lo decide
`ComparadorDeTriaje`: menor prioridad y, si empatan, orden de llegada. Después de cada
evento dibuja la cola como árbol y como lista con índices, resalta al paciente que se
movió e informa los intercambios.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de la tabla de eventos de
`caso.md`. No se lee ningún archivo externo.

## Abrir en Visual Studio

1. Abra `TriajeUrgencias.sln` con Visual Studio 2022 (17.x) o posterior, con el SDK de
   .NET 10 instalado.
2. Establezca `TriajeUrgencias` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `TriajeUrgenciasTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore TriajeUrgencias.sln
dotnet build TriajeUrgencias.sln
dotnet run --project src/TriajeUrgencias.csproj
dotnet run --project src/TriajeUrgencias.csproj -- --demo
dotnet test tests/TriajeUrgenciasTests.csproj
```

Sin argumentos, el programa muestra un menú. La opción 2 procesa un evento a la vez.
Las opciones 4 y 5 permiten admitir o llamar fuera del guion. Con `--demo` procesa los
11 eventos y vacía la sala, sin pausas.

En el dibujo, `<X>` marca al paciente que acaba de admitirse o al que bajó desde la
raíz en una llamada. En una terminal con colores se ve en amarillo.

## Estructura

- `src/MonticuloBinario.cs`: `Insertar`, `ExtraerRaiz`, `VerRaiz`, `Copia`, `Altura`,
  `UltimosIntercambios` y `EsValido`.
- `src/ComparadorDeTriaje.cs`: criterio de prioridad con desempate por llegada.
- `src/ImpresorMonticulo.cs`: dibuja la lista como árbol (hijos en 2i + 1 y 2i + 2) y
  como lista con índices.
- `src/DatosDelCaso.cs`: eventos de la mañana.
- `src/Modelos/`: `Paciente` y `EventoDeSala`.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/TriajeUrgenciasTests.cs`: pruebas de la lista tras las admisiones, el empate,
  las llamadas, el orden completo, `VerRaiz` y la sala vacía.
