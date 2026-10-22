# Proyecto2_PCG_LightsOut

Lights Out es un juego de mesa donde debes lograr apagar todas las luces del tablero. Esta versión de Lights Out dispone de distintos métodos de generación procedural tales como: **Evolutionary Strategy**, **Simulated Annealing** y **Random Start**.

---

## Descripción

Proyecto de laboratorio basado en el puzzle **Lights Out**, cuyo objetivo es apagar todas las luces del tablero. Al pulsar una celda, cambian de estado ella misma y sus celdas adyacentes (sin considerar diagonales).

La dificultad del puzzle está en que un movimiento puede apagar luces, pero también encender otras, por lo que se requiere una estrategia de búsqueda para evitar aumentar la cantidad de luces encendidas innecesariamente.

---

## Estrategia de estados iniciales: Random Start State

Se selecciona **Random Start State** como sistema de generación del estado inicial del puzzle porque permite generar diferentes configuraciones de tablero y, de esta manera, evolucionar distintas variantes hasta conseguir un resultado resoluble.

El uso de este enfoque busca priorizar la variabilidad de los tableros iniciales y comprobar la capacidad de **Evolutionary Strategy** para evolucionar hacia una versión soluble.

---

## Algoritmo evolutivo: Evolutionary Strategy

Se eligió **Evolutionary Strategy** principalmente para crear escenarios que evolucionen variando las luces apagadas y encendidas. De esta manera se busca asegurar que se generen los mejores individuos y que los no seleccionados muten, alcanzando variantes más diversas pero visibles dentro del proceso evolutivo.

---

## Técnica de búsqueda local: Simulated Annealing

Se seleccionó **Simulated Annealing** porque el tablero de Lights Out es cambiante: al presionar una celda se pueden encender o apagar luces a su alrededor. El primer caso es el más probable, lo que puede llevar a que la energía medida por el algoritmo de búsqueda local decida no realizar el paso hacia el vecino.


---

## Estado y vecindario

- **Estado:** conjunto de celdas pulsadas.
- **Vecindario:** tableros resultantes al pulsar una de las celdas del tablero aleatoriamente.
- **Evaluación:** se compara el valor de energía/entropía del tablero actual con el del vecino. A menor cantidad de luces encendidas, menor entropía.
- **Movimiento:** si el valor de entropía del vecino es menor, se pulsa la celda seleccionada.
- **Métrica:** se cuenta la cantidad de pasos realizados hasta llegar al estado resuelto. Estos pasos sirven como fitness para **Evolutionary Strategy**.
- **Validación:** el análisis completo se realiza solo si el puzzle es resoluble, verificándolo mediante **eliminación gaussiana**.

---

## Interfaz de usuario

La interfaz gráfica utiliza un **canvas** con un panel para modificar los valores de generación del mapa. Para que el usuario pueda hacer clic sobre las celdas, activando y desactivando las luces, se implementó una matriz de cubos que se generan al presionar **Start**.

La lógica del juego se maneja con una matriz, en la que el estado de cada luz se representa con una variable booleana:

- `true`: luz encendida.
- `false`: luz apagada.

---

## Parámetros de la Interfaz

Estas opciones controlan cómo buscan la solución los algoritmos. No cambian el puzzle, solo el comportamiento de la búsqueda.

### Simulated Annealing (búsqueda local)

- **Temperatura inicial:** valor con el que arranca el algoritmo. Alta = explora más (acepta movimientos que empeoran al principio para no atascarse). Baja = converge rápido pero se estanca fácil. *Default: 2.0*
- **Factor de Enfriamiento:** qué tan rápido baja la temperatura en cada iteración (`T = T × factor`). Cerca de 1 (ej. 0.999) = búsqueda lenta y exhaustiva. Más bajo (ej. 0.9) = converge rápido con riesgo de mínimo local. *Default: 0.995*
- **Temperatura mínima:** temperatura a la que el algoritmo se detiene. *Default: 0.05*
- **Intentos por temperatura:** cuántos movimientos se prueban antes de enfriar. Más intentos = mejor calidad pero más tiempo. *Default: 50*

### Evolutionary Strategy

- **Generaciones Evolutivas:** cuántas iteraciones del ciclo evolutivo se ejecutan (mutar, evaluar con Annealing, seleccionar mejores). Más generaciones = mejores soluciones pero más costo. *Default: 50*
- **Usar evolutivo (sí/no):** activa o desactiva la Evolutionary Strategy. Si está en **no**, solo se busca un tablero resoluble con Random Start + Annealing (más rápido, menos variado).

### Visualización

- **Tiempo entre intentos:** pausa en segundos entre pasos mostrados en pantalla. **No afecta la calidad de la solución**, solo cuánto tarda en verse la animación. se recomienda bajarlo para pruebas rápidas. *Default: 1.0 s*

---

## Regla rápida de ajuste

| Si el problema es... | Ajusta... |
|---|---|
| Tarda demasiado | Disminuir Generaciones, disminuir Intentos por temperatura, aumentar Factor de Enfriamiento |
| Se atasca en la misma solución | Aumentar Temperatura inicial, disminuir Factor de Enfriamiento |
| Descarta muchos tableros | Aumentar Generaciones, aumentar Intentos por temperatura |
| No quieres ver la animación | Disminuir Tiempo entre intentos |

[Ver Proyecto en Itchio]([docs/instalacion.md](https://pinamc.itch.io/procedurallightsout20-moreproceduralthanever))

