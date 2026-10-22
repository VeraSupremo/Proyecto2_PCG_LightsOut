using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolutivo : MonoBehaviour
{
    [SerializeField] private AnnealingSimulated annealing;
    [SerializeField] private int generaciones = 50;

    public int Generaciones
    {
        get => generaciones;
        set => generaciones = value;
    }

    private void Awake()
    {
        if (annealing == null)
            annealing = GetComponent<AnnealingSimulated>() ?? FindObjectOfType<AnnealingSimulated>();
    }

    
    public Tablero Ejecutar(List<Tablero> poblacion, int mu, int lambda, int filas, int columnas)
    {
        if (annealing == null)
            annealing = GetComponent<AnnealingSimulated>() ?? FindObjectOfType<AnnealingSimulated>();

        if (poblacion == null || poblacion.Count == 0)
        {
            Debug.LogError("<color=red>[Evolutivo] Error: La población está vacía.</color>");
            return null;
        }

        mu = Mathf.Clamp(mu, 1, poblacion.Count);
        lambda = Mathf.Max(1, lambda);

        int descartados = 0;
        Ordenar(poblacion, mu);

        for (int g = 0; g < generaciones; g++)
        {
            if (poblacion.Count == 0) break;

            List<Tablero> hijos = new List<Tablero>();

            for (int i = 0; i < lambda; i++)
            {
                Tablero padre = poblacion[Random.Range(0, poblacion.Count)];
                Tablero hijo = padre.Clone();
                Mutar(hijo, filas, columnas);

                // Analizador (Gauss) + annealing dentro de Procesar
                if (annealing.Procesar(hijo)) hijos.Add(hijo);
                else descartados++;
            }

            poblacion.AddRange(hijos);
            Ordenar(poblacion, mu);
        }

        Tablero mejor = poblacion.Count > 0 ? poblacion[0] : null;
        if (mejor != null)
        {
            Debug.Log($"Evolutivo ({mu}+{lambda}): pulsaciones={mejor.NPulsaciones}, descartados={descartados}, " +
                      $"evaluaciones={annealing.Evaluaciones}, pasos={annealing.PasosTotales}");
        }
        return mejor;
    }

    // Con corrutina para ver el paso a paso en pantalla sin congelar Unity
    public IEnumerator EjecutarPasoAPaso(
        List<Tablero> poblacion, int mu, int lambda, int filas, int columnas,
        float tiempoPausa,
        System.Action<Tablero, string> alMostrarTablero,
        System.Action<Tablero> alFinalizar)
    {
        if (annealing == null)
            annealing = GetComponent<AnnealingSimulated>() ?? FindObjectOfType<AnnealingSimulated>();

        // Validacion de seguridad para evitar ArgumentOutOfRangeException
        if (poblacion == null || poblacion.Count == 0)
        {
            Debug.LogError("<color=red>[Evolutivo] Error: La población recibida está vacía. No se puede ejecutar la evolución.</color>");
            alFinalizar?.Invoke(null);
            yield break;
        }

        mu = Mathf.Clamp(mu, 1, poblacion.Count);
        lambda = Mathf.Max(1, lambda);

        int descartados = 0;
        Ordenar(poblacion, mu);

        for (int g = 0; g < generaciones; g++)
        {
            if (poblacion.Count == 0) break;

            List<Tablero> hijos = new List<Tablero>();

            for (int i = 0; i < lambda; i++)
            {
                Tablero padre = poblacion[Random.Range(0, poblacion.Count)];
                Tablero hijo = padre.Clone();
                Mutar(hijo, filas, columnas);

                if (annealing.Procesar(hijo))
                {
                    hijos.Add(hijo);
                }
                else
                {
                    descartados++;
                }
            }

            poblacion.AddRange(hijos);
            Ordenar(poblacion, mu);

            if (poblacion.Count > 0)
            {
                Tablero mejorGen = poblacion[0];
                alMostrarTablero?.Invoke(mejorGen, $"[Generación {g + 1}/{generaciones}] Mejor tablero actual: {mejorGen.NPulsaciones} pulsaciones (Descartados: {descartados})");
            }

            yield return new WaitForSeconds(tiempoPausa);
        }

        Tablero mejorFinal = poblacion.Count > 0 ? poblacion[0] : null;
        if (mejorFinal != null)
        {
            Debug.Log($"<color=green>Evolutivo ({mu}+{lambda}) finalizado con éxito: pulsaciones={mejorFinal.NPulsaciones}, descartados={descartados}, evaluaciones={annealing.Evaluaciones}, pasos={annealing.PasosTotales}</color>");
        }
        alFinalizar?.Invoke(mejorFinal);
    }

    private void Ordenar(List<Tablero> poblacion, int mu)
    {
        if (poblacion == null || poblacion.Count == 0) return;
        mu = Mathf.Max(1, mu);
        poblacion.Sort((a, b) => b.NPulsaciones.CompareTo(a.NPulsaciones));
        if (poblacion.Count > mu) poblacion.RemoveRange(mu, poblacion.Count - mu);
    }

    // cada luz cambia con prob. 1/n (en promedio una), mínimo una
    private void Mutar(Tablero t, int filas, int columnas)
    {
        float prob = 1f / (filas * columnas);
        bool alguna = false;
        for (int f = 0; f < filas; f++)
        {
            for (int c = 0; c < columnas; c++)
            {
                if (Random.value < prob)
                {
                    t.AlternarLuz(f, c);
                    alguna = true;
                }
            }
        }
        if (!alguna) t.AlternarLuz(Random.Range(0, filas), Random.Range(0, columnas));
    }
}
