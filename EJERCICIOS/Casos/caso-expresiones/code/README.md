# TarifasExpresion

## Objetivo

Solución de referencia del caso de árboles de expresión para la liquidación de
tarifas. El programa construye el árbol de cada fórmula a partir de su notación
sufija, la dibuja en la consola y muestra las cuatro escrituras (prefijo, infijo
con paréntesis, sufijo y valor), sus métricas de forma y el rechazo de las
notaciones mal formadas.

Los datos están en `src/DatosDelCaso.cs` y son los mismos de `caso.md`. No se lee
ningún archivo externo.

## Abrir en Visual Studio

1. Abra `TarifasExpresion.sln` con Visual Studio 2022 (17.x) o posterior, con el
   SDK de .NET 10 instalado.
2. Establezca `TarifasExpresion` como proyecto de inicio y ejecútelo.
3. Use el Explorador de pruebas para ejecutar `TarifasExpresionTests`.

## Restaurar, compilar, ejecutar y probar

Desde esta carpeta:

```bash
dotnet restore TarifasExpresion.sln
dotnet build TarifasExpresion.sln
dotnet run --project src/TarifasExpresion.csproj
dotnet run --project src/TarifasExpresion.csproj -- --demo
dotnet test tests/TarifasExpresionTests.csproj
```

Sin argumentos, el programa muestra un menú. Con `--demo` recorre los tres convenios
y las notaciones inválidas, sin pausas.

En el dibujo, `[X]` es un nodo. En la vista paso a paso, `<X>` marca el nodo de la
fórmula que se está explicando.

## Estructura

- `src/NodoExpresion.cs`: nodo hoja con un número o nodo operador con dos hijos.
- `src/ConstructorDeExpresion.cs`: construye el árbol desde la notación sufija con
  una pila y rechaza notaciones mal formadas.
- `src/ArbolDeExpresion.cs`: valor en postorden, prefijo, infijo con paréntesis,
  sufijo y métricas (`ContarNodos`, `ContarHojas`, `ContarOperadores`, `Altura`,
  `Ancho`, `Niveles`).
- `src/ImpresorExpresion.cs`: dibuja el árbol en la consola, de arriba hacia abajo.
- `src/Convenio.cs`: nombre, notación sufija y tarifa esperada de un convenio.
- `src/DatosDelCaso.cs`: convenios y notaciones inválidas.
- `src/Program.cs`: menú y modo `--demo`.
- `tests/TarifasExpresionTests.cs`: pruebas de construcción, recorridos, evaluación,
  métricas y notaciones inválidas.
