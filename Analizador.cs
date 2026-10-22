using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este script analiza si un tablero es resoluble o no, y si lo es, devuelve la matriz de pulsaciones que lo resuelve
//La matriz de pulsaciones es una matriz de booleanos que indica qué celdas deben ser pulsadas para resolver el tablero
public static class Analizador
{
    public static int[,] ConstruirMA(int filas, int columnas)
    {
        int total = filas * columnas;
        int[,] A = new int[total, total];
        for(int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                int i = fila * columnas + columna;
                A[i, i] = 1;
                if (columna > 0)
                {
                    A[i, i - 1] = 1;
                }
                if (columna + 1 < columnas)
                {
                    A[i, i + 1] = 1;
                }
                if (fila > 0)
                {
                    A[i, i - columnas] = 1;
                }
                if (fila + 1<filas)
                {
                    A[i, i + columnas] = 1;
                }
            }
        }
        return A;
    }
    public static bool esResoluble(bool[,] luces, int filas, int columnas)
    {

        int[,] A = ConstruirMA(filas, columnas);
        int[] b = new int[filas * columnas];

        for (int fila = 0; fila < filas;fila++)
        {
            for (int columna=0;columna<columnas;columna++)
            {
                if(luces[fila, columna])
                {
                    b[fila * columnas + columna] = 1;
                }
                else
                {
                    b[fila * columnas + columna] = 0;
                }
            }
        }
        return esResoluble(A, b);

    }
    public static bool esResoluble(int[,] A, int[]b)
    {
        int filas = A.GetLength(0);
        int columnas = A.GetLength(1);

        int[,] matriz = new int[filas, columnas + 1];
        for(int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                matriz[fila, columna] = A[fila, columna];
            }
            matriz[fila, columnas] = b[fila];
        }
        int filaPivote = 0;
        for (int columna = 0; columna < columnas && filaPivote < filas; columna++)
        {
            int filaCPivote = -1;
            for (int fila = filaPivote; fila < filas; fila++)
            {
                if (matriz[fila, columna] == 1)
                {
                    filaCPivote = fila;break;
                }
            }
            if (filaCPivote == -1) continue;
            for (int col = 0; col <= columnas; col++)
            {
                int temporal = matriz[filaPivote, col];
                matriz[filaPivote, col] = matriz[filaCPivote, col];
                matriz[filaCPivote, col] = temporal;
            }
            for (int fila = 0; fila < filas; fila++)
            {
                if (fila != filaPivote && matriz[fila, columna] == 1)
                {
                    for (int col = columna; col <= columnas; col ++) {
                        if (matriz[fila, col] == matriz[filaPivote, col])
                        {
                            matriz[fila, col] = 0;
                        }
                        else
                        {
                            matriz[fila, col] = 1;
                        }
                    }
                }
            }
            filaPivote++;
        }

        for(int fila = 0; fila < filas; fila++)
        {
            bool ZERO = true;
            for(int columna = 0; columna < columnas; columna++)
            {
                if (matriz[fila, columna] == 1 ) {  ZERO = false; break; }
            }
            if (ZERO && matriz[fila, columnas] == 1)
            {
                return false;
            }
        }
        return true;
    }
}
