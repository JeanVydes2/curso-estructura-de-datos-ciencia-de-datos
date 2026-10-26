# IndiceDePrecios

## Objetivo

Solución de referencia del caso de consultas de orden sobre el índice de precios de
la lonja. El programa inserta los 12 precios del caso en un árbol binario de búsqueda
que guarda en cada nodo el tamaño de su subárbol y lo dibuja en la consola con la
etiqueta `[precio:tamaño]`. Después responde las consultas del día: piso y techo de un
valor, el k-ésimo precio más barato, el conteo por debajo de un valor y un rango.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de `caso.md`. No se lee
ningún archivo externo.

## Abrir en Visual Studio

1. Abra `IndiceDePrecios.sln` con Visual Studio 2022 (17.x) o posterior, con el SDK de
   .NET 10 instalado.
2. Establezca `IndiceDePrecios` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `IndiceDePreciosTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore IndiceDePrecios.sln
dotnet build IndiceDePrecios.sln
dotnet run --project src/IndiceDePrecios.csproj
dotnet run --project src/IndiceDePrecios.csproj -- --demo
dotnet test tests/IndiceDePreciosTests.csproj
```

Sin argumentos, el programa muestra un menú. Con `--demo` responde las consultas del
día, sin pausas.

En el dibujo, cada nodo se etiqueta `[precio:tamaño]`; el nodo que se resalta se ve
entre `< >` y en amarillo. La etiqueta incluye el tamaño para poder revisar el
invariante del punto 2 del caso.

## Estructura

- `src/NodoPrecio.cs`: nodo con `Precio`, `Producto`, `Izquierdo`, `Derecho` y
  `Tamano`.
- `src/ArbolDePrecios.cs`: `Insertar`, `Buscar`, `Piso`, `Techo`, `KEsimo`,
  `ConteoPorDebajo`, `EnOrden`, `Rango`, `Contar`, `Altura` y `EsValido`.
- `src/ImpresorPrecios.cs`: dibuja el árbol en la consola con `[precio:tamaño]`.
- `src/Producto.cs`: nombre y precio de un producto.
- `src/DatosDelCaso.cs`: precios, consultas de piso y techo, k-ésimos y valor del
  conteo.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/IndiceDePreciosTests.cs`: pruebas de construcción, tamaños, inorden, piso,
  techo, k-ésimo, conteo por debajo y duplicados.
