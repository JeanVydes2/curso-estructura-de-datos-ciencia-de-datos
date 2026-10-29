# Solución de las preguntas de análisis

1. `RecordNumber` identifica una lectura de forma única y permite organizarla sin depender del producto o del centro donde se tomó.
2. Una lectura duplicada puede alterar la trazabilidad y hacer difícil determinar cuál temperatura corresponde realmente al lote.
3. Una temperatura fuera de los límites permitidos representa una desviación que puede comprometer el producto y requiere atención crítica.
4. `Review` identifica una temperatura cercana a los límites de control; `Critical` representa una temperatura fuera del rango permitido.
5. El recorrido por niveles permite observar la distribución de los registros desde la raíz y cómo se organizan por profundidad.
6. Las nuevas inserciones pueden aumentar la cantidad de nodos, la altura y modificar los recorridos; el duplicado no debe cambiar el árbol.
