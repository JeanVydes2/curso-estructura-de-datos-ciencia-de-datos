# ClasificacionBecas

## Objetivo

Solución de referencia del caso de clasificación de solicitudes de beca. El
programa construye un árbol binario a partir de la tabla de reglas del caso (Sí →
`Izquierdo`, No → `Derecho`), valida que la tabla describa un árbol y lo dibuja en
la consola. Después permite revisar sus métricas y recorridos, clasificar cada
solicitud del semestre paso a paso y obtener las condiciones que llevan a un apoyo.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de las tablas de
`caso.md`. No se lee ningún archivo externo.

## Abrir en Visual Studio

1. Abra `ClasificacionBecas.sln` con Visual Studio 2022 (17.x) o posterior, con el
   SDK de .NET 10 instalado.
2. Establezca `ClasificacionBecas` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `ClasificacionBecasTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore ClasificacionBecas.sln
dotnet build ClasificacionBecas.sln
dotnet run --project src/ClasificacionBecas.csproj
dotnet run --project src/ClasificacionBecas.csproj -- --demo
dotnet test tests/ClasificacionBecasTests.csproj
```

Sin argumentos, el programa muestra un menú. Al clasificar una solicitud (opción
5), espera Enter entre una pregunta y la siguiente. Con `--demo` ejecuta todas las
opciones seguidas, sin pausas.

En el dibujo, `[X]` es un nodo, `(X)` un nodo ya recorrido y `<X>` el nodo actual.
En una terminal con colores, el recorrido se ve en cian y el nodo actual en
amarillo.

## Estructura

- `src/NodoBinario.cs`: nodo con `Valor`, `Izquierdo` y `Derecho`.
- `src/ArbolBinario.cs`: raíz, conteos, altura, `EsLleno`, los cuatro recorridos y
  `CaminoHacia`.
- `src/ConstructorDeArbol.cs`: construye el árbol desde la tabla, encuentra la raíz
  y rechaza tablas mal formadas.
- `src/ClasificadorBecas.cs`: baja desde la raíz respondiendo una pregunta por
  nivel.
- `src/ImpresorConsola.cs`: dibuja el árbol en la consola, de arriba hacia abajo.
- `src/DatosDelCaso.cs`: tabla de reglas y solicitudes del semestre.
- `src/Modelos/`: `Regla`, `Comprobacion` y `Solicitud`.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/ClasificacionBecasTests.cs`: pruebas de construcción, métricas, recorridos,
  clasificación y tablas inválidas.
