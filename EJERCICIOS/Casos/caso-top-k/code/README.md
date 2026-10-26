# LluviaTopK

## Objetivo

Solución de referencia del caso de los K pluviómetros con más lluvia. El programa
recorre el lote de prueba manteniendo un montículo de mínimos de tamaño K
(`MonticuloBinario<Acumulado>`, guardado en una `List<T>`) y muestra el estado tras
cada lectura, los puntos que salen, el Top-K de mayor a menor, el Bottom-K con el
comparador invertido y la construcción por Floyd contra la inserción uno por uno.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de la tabla de `caso.md`. No
se lee ningún archivo externo; el archivo real del mes tiene 6 000 000 de filas, que
aquí se representa con un lote de 12 lecturas.

## Abrir en Visual Studio

1. Abra `LluviaTopK.sln` con Visual Studio 2022 (17.x) o posterior, con el SDK de .NET
   10 instalado.
2. Establezca `LluviaTopK` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `LluviaTopKTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore LluviaTopK.sln
dotnet build LluviaTopK.sln
dotnet run --project src/LluviaTopK.csproj
dotnet run --project src/LluviaTopK.csproj -- --demo
dotnet test tests/LluviaTopKTests.csproj
```

Sin argumentos, el programa muestra un menú. La opción 1 recorre el lote una lectura a
la vez. Con `--demo` recorre el lote y muestra los tres resultados, sin pausas.

En el dibujo del montículo, `[X]` es un elemento y `<X>` el resaltado. Las etiquetas se
escriben `Id:milímetros`.

## Estructura

- `src/MonticuloBinario.cs`: `Insertar`, `ExtraerRaiz`, `VerRaiz`, `Copia`, `Altura`,
  `UltimosIntercambios`, `EsValido` y `ConstruirDesde` (método de Floyd).
- `src/ComparadorPorMilimetros.cs`: ordena por milímetros y desempata por id.
- `src/SeleccionTopK.cs`: `TopK` y `BottomK`, con el mismo recorrido y el comparador
  invertido.
- `src/ImpresorMonticulo.cs`: dibuja la lista como árbol (hijos en 2i + 1 y 2i + 2) y
  como lista con índices.
- `src/Modelos/Acumulado.cs`: identificador y milímetros de un pluviómetro.
- `src/DatosDelCaso.cs`: lote de prueba y valor de K.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/LluviaTopKTests.cs`: pruebas del Top-K, el Bottom-K, los casos límite y la
  construcción por Floyd.
