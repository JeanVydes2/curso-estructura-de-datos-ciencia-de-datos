# CatalogoEstaciones

## Objetivo

Solución de referencia del caso del catálogo de estaciones de la red
hidrometeorológica del Magdalena. El programa inserta las 12 estaciones del caso en
un árbol binario de búsqueda con el código como clave y lo dibuja en la consola.
Después muestra cada operación sobre el dibujo: la validación de lecturas comparación
por comparación, el menor y el mayor código, el registro de una estación, los tres
casos de eliminación, el reporte por rango de códigos y el árbol degenerado que
produce una carga ordenada.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de las tablas de
`caso.md`. No se lee ningún archivo externo.

## Abrir en Visual Studio

1. Abra `CatalogoEstaciones.sln` con Visual Studio 2022 (17.x) o posterior, con el
   SDK de .NET 10 instalado.
2. Establezca `CatalogoEstaciones` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `CatalogoEstacionesTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore CatalogoEstaciones.sln
dotnet build CatalogoEstaciones.sln
dotnet run --project src/CatalogoEstaciones.csproj
dotnet run --project src/CatalogoEstaciones.csproj -- --demo
dotnet test tests/CatalogoEstacionesTests.csproj
```

Sin argumentos, el programa muestra un menú. La búsqueda (opción 3) espera Enter
entre una comparación y la siguiente. Con `--demo` ejecuta la secuencia completa del
caso, sin pausas.

En el dibujo, `[X]` es un nodo, `(X)` un nodo ya visitado y `<X>` el nodo actual. En
una terminal con colores, lo visitado se ve en cian y el nodo actual en amarillo.

## Estructura

- `src/NodoBinario.cs`: nodo con `Valor`, `Izquierdo` y `Derecho`.
- `src/ArbolBinarioBusqueda.cs`: `Insertar`, `Buscar`, `CaminoDeBusqueda`,
  `Eliminar`, `CasoDeEliminacion`, `SucesorInorden`, `Minimo`, `Maximo`, `EnOrden`,
  `Rango` y `EsValido`.
- `src/ComparadorDeCodigo.cs`: ordena estaciones por código para simular la carga
  ordenada.
- `src/ImpresorArbol.cs`: dibuja el árbol en la consola, de arriba hacia abajo.
- `src/DatosDelCaso.cs`: estaciones en orden de instalación, lecturas y cambios del
  año.
- `src/Modelos/Estacion.cs`: ficha de una estación; la cuenca sale del primer dígito
  del código.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/CatalogoEstacionesTests.cs`: pruebas de altura, inorden, búsquedas,
  inserción, los tres retiros, rango, mínimo y máximo y carga ordenada.
