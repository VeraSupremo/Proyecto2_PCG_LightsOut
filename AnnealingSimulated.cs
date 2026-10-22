using UnityEngine;

public class AnnealingSimulated : MonoBehaviour
{
    [SerializeField] private float temperaturaI = 2f;
    [SerializeField] private float enfriaminento = 0.995f;
    [SerializeField] private float tempMin = 0.05f;
    [SerializeField] private int pasosT = 50;

    // Contadores
    public int Evaluaciones { get; private set; }
    public long PasosTotales { get; private set; }

    // Propiedades públicas para GameUI y para la configuración externa
    public float TemperaturaInicial
    {
        get => temperaturaI;
        set => temperaturaI = value;
    }

    public float FactorEnfriamiento
    {
        get => enfriaminento;
        set => enfriaminento = value;
    }

    public float TemperaturaMinima
    {
        get => tempMin;
        set => tempMin = value;
    }

    public int IntentosPorTemperatura
    {
        get => pasosT;
        set => pasosT = value;
    }

    public bool Procesar(Tablero tablero)
    {
        if (!Analizador.esResoluble(tablero.Luces, tablero.Filas, tablero.Columnas))
        {
            return false;
        }

        int fit = Evaluar(tablero, out bool[,] solucion);
        if (fit < 0) return false;
        tablero.Pulsaciones = solucion;
        tablero.NPulsaciones = fit;

        return true;
    }

    private int Evaluar(Tablero objetivo, out bool[,] solucion)
    {
        Evaluaciones++;
        int filas = objetivo.Filas;
        int columnas = objetivo.Columnas;

        Tablero tActual = new Tablero(filas, columnas);
        tActual.Luces = (bool[,])objetivo.Luces.Clone();
        tActual.NLuces = objetivo.NLuces;

        float energia = tActual.NLuces;

        for (float T = temperaturaI; T > tempMin && energia > 0; T *= enfriaminento)
        {
            for (int a = 0; a < pasosT && energia > 0; a++)
            {
                PasosTotales++;
                int fila = Random.Range(0, filas);
                int columna = Random.Range(0, columnas);
                tActual.Pulsar(fila, columna);

                float energy = tActual.NLuces;
                float deltaE = energy - energia;

                if (deltaE <= 0 || Random.value < Mathf.Exp(-deltaE / T))
                {
                    energia = energy;
                }
                else
                {
                    tActual.Pulsar(fila, columna);
                }
            }
        }

        // Guarda la solución encontrada 
        solucion = tActual.Pulsaciones;
        if (energia == 0)
        {
            return tActual.NPulsaciones;
        }
        else
        {
            return -1;
        }
    }
}
