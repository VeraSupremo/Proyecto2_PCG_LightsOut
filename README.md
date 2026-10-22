# Proyecto2_PCG_LightsOut
Lights Out es un juego de mesa donde debes lograr apagar todas las luces del tablero, Esta version de lights out dispone de distintos metodos de generacion procedural tales como: evolutionary strategy, anealing simulate y random start



## Descripción

Proyecto de laboratorio basado en el puzzle **Lights Out**, cuyo objetivo es apagar todas las luces del tablero. Al pulsar una celda, cambian de estado ella misma y sus celdas adyacentes (sin considerar diagonales).

La dificultad del puzzle está en que un movimiento puede apagar luces, pero también encender otras, por lo que se requiere una estrategia de búsqueda para evitar aumentar la cantidad de luces encendidas innecesariamente.

## Estrategia de estados iniciales: Random Start State

Se selecciona **Random Start State** como sistema de generación del estado inicial del puzzle porque permite generar diferentes configuraciones de tablero y, de esta manera, evolucionar distintas variantes hasta conseguir un resultado resoluble.

El uso de este enfoque busca priorizar la variabilidad de los tableros iniciales y comprobar la capacidad de **Evolutionary Strategy** para evolucionar hacia una versión soluble. 


## Algoritmo evolutivo: Evolutionary Strategy

Se eligió **Evolutionary Strategy** principalmente para crear escenarios que evolucionen variando las luces apagadas y encendidas. De esta manera se busca asegurar que se generen los mejores individuos y que los no seleccionados muten, alcanzando variantes más diversas pero visibles dentro del proceso evolutivo.

## Técnica de búsqueda local: Simulated Annealing

Se seleccionó **Simulated Annealing** porque el tablero de Lights Out es cambiante: al presionar una celda se pueden encender o apagar luces a su alrededor. El primer caso es el más probable, lo que puede llevar a que la energía medida por el algoritmo de búsqueda local decida no realizar el paso hacia el vecino.

Sin embargo, un movimiento que parece peor en ese momento —porque aumenta la cantidad de luces encendidas— puede conducir con alta probabilidad a una solución final mucho más favorable, con menos luces encendidas o con el puzzle resuelto.

## Estado y vecindario

- **Estado:** conjunto de celdas pulsadas.
- **Vecindario:** tableros resultantes al pulsar una de las celdas del tablero aleatoriamente.
- **Evaluación:** se compara el valor de energía/entropía del tablero actual con el del vecino. A menor cantidad de luces encendidas, menor entropía.
- **Movimiento:** si el valor de entropía del vecino es menor, se pulsa la celda seleccionada.
- **Métrica:** se cuenta la cantidad de pasos realizados hasta llegar al estado resuelto. Estos pasos sirven como fitness para **Evolutionary Strategy**.
- **Validación:** el análisis completo se realiza solo si el puzzle es resoluble, verificándolo mediante **eliminación gaussiana**.

## Interfaz de usuario

La interfaz gráfica utiliza un **canvas** con un panel para modificar valores de la generacion del mapa, por ejemplo:
- **Enfriamiento**
- **Temperatura inicial**
- **Intentos por temperatura** 
- **Tiempo entre intentos**
- **Factor de Enfriamiento**
- **Generaciones Evolutivas**
- **Usar evolutivo(si/no)**
para que el usuario pueda hacer clic sobre las celdas, activando y desactivando las luces se implemento una matriz de cubos los cuales se generan al presionar start.

La lógica del juego se maneja con una matriz, en la que el estado de cada luz se representa con una variable booleana:

- `true`: luz encendida.
- `false`: luz apagada.
