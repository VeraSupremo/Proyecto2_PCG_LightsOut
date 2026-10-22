using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStart : MonoBehaviour
{

    public Tablero GenStart(int filas,int columnas)
    {
        Tablero tablero = new Tablero(filas, columnas);
            for (int f = 0; f < filas; f++)
            {
                for (int c = 0; c < columnas; c++)
                {
                    // Random state: true or false

                    if (Random.value > 0.5f)
                    { 
                        tablero.AlternarLuz(f, c); 
                    }
                }
            }
        return tablero;
    }
}
