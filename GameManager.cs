using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero
{
    public readonly int Filas, Columnas;
    public CeldaCs[,] Celdas;
    public bool[,] Luces;        // estado inicial
    public bool[,] Pulsaciones;  // solución (x)
    public int NPulsaciones;     // fitness (más = mejor)
    public int NLuces;

    public Tablero(int filas, int columnas)
    {
        Filas = filas;
        Columnas = columnas;
        Celdas = new CeldaCs[filas, columnas];
        Luces = new bool[filas, columnas];
        Pulsaciones = new bool[filas, columnas];
    }

    public void Pulsar(int filas, int columnas)
    {
        Pulsaciones[filas, columnas] = !Pulsaciones[filas, columnas];
        if (Pulsaciones[filas, columnas])
        {
            NPulsaciones++;
        }
        else
        {
            NPulsaciones--;
        }
        ACruz(filas, columnas);
    }

    public void ACruz(int filas, int columnas)
    {
        Activas(filas, columnas);
        Activas(filas - 1, columnas);
        Activas(filas + 1, columnas);
        Activas(filas, columnas - 1);
        Activas(filas, columnas + 1);
    }

    // Cambia UNA sola luz
    public void AlternarLuz(int filas, int columnas)
    {
        if (Luces[filas, columnas] == true)
        {
            Luces[filas, columnas] = false;
            NLuces = NLuces - 1;
        }
        else
        {
            Luces[filas, columnas] = true;
            NLuces = NLuces + 1;
        }
    }

    private void Activas(int filas, int columnas)
    {
        if (filas < 0 || filas >= Filas || columnas < 0 || columnas >= Columnas)
        {
            return;
        }
        Luces[filas, columnas] = !Luces[filas, columnas];
        if (Luces[filas, columnas])
        {
            NLuces++;
        }
        else
        {
            NLuces--;
        }
    }

    public Tablero Clone()
    {
        Tablero table = new Tablero(Filas, Columnas);
        table.Luces = (bool[,])Luces.Clone();
        table.Pulsaciones = (bool[,])Pulsaciones.Clone();
        table.NPulsaciones = NPulsaciones;
        table.NLuces = NLuces;
        return table;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject CeldaPrefab;
    [SerializeField] public Transform AreaJuego;

    [SerializeField] private RandomStart Rscript;
    [SerializeField] private AnnealingSimulated Annealing;
    [SerializeField] private Evolutivo evolutivo;
    [SerializeField] private bool usarEvolutivo = true;

    [SerializeField] private int mu = 3;       // padres
    [SerializeField] private int lambda = 4;   // hijos por generación

    [Header("Visualización y UI")]
    [Tooltip("Tiempo en segundos configurado desde la interfaz para mostrar el paso a paso")]
    [SerializeField] public float tiempoEntreIntentos = 1.0f;

    public static readonly int Filas = 7;
    public static readonly int Columnas = 7;

    private Tablero tablero = new Tablero(Filas, Columnas);
    private CeldaCs[,] celdasVisuales = new CeldaCs[Filas, Columnas];

    private bool juegoListo = false;
    private Coroutine rutinaGeneracion;

    // Propiedades públicas para GameUI
    public int Mu { get => mu; set => mu = value; }
    public int Lambda { get => lambda; set => lambda = value; }
    public bool UsarEvolutivo { get => usarEvolutivo; set => usarEvolutivo = value; }

    private void Awake()
    {
        if (evolutivo == null)
            evolutivo = GetComponent<Evolutivo>() ?? FindObjectOfType<Evolutivo>();
        if (Annealing == null)
            Annealing = GetComponent<AnnealingSimulated>() ?? FindObjectOfType<AnnealingSimulated>();
        if (Rscript == null)
            Rscript = GetComponent<RandomStart>() ?? FindObjectOfType<RandomStart>();
    }

    public void CambiarEstado(int filas, int columnas)
    {
        // No permitir clics mientras se genera el tablero paso a paso
        if (!juegoListo || tablero == null || celdasVisuales[filas, columnas] == null) return;

        tablero.ACruz(filas, columnas);
        Refresh(filas, columnas);
        Refresh(filas - 1, columnas);
        Refresh(filas + 1, columnas);
        Refresh(filas, columnas - 1);
        Refresh(filas, columnas + 1);

        if (tablero.NLuces == 0) Debug.Log("<color=green>¡Ganaste! Todas las luces están apagadas.</color>");
    }

    private void Refresh(int fila, int columna)
    {
        if (fila < 0 || fila >= Filas || columna < 0 || columna >= Columnas) { return; }
        if (celdasVisuales[fila, columna] != null)
        {
            celdasVisuales[fila, columna].SetEstado(tablero.Luces[fila, columna]);
        }
    }

    private void LimpiarTablero()
    {
        for (int i = AreaJuego.childCount - 1; i >= 0; i--)
        {
            Destroy(AreaJuego.GetChild(i).gameObject);
        }
    }

    public void IniciarGame()
    {
        if (rutinaGeneracion != null)
        {
            StopCoroutine(rutinaGeneracion);
        }

        rutinaGeneracion = StartCoroutine(RutinaGeneracionPasoAPaso());
    }

    private IEnumerator RutinaGeneracionPasoAPaso()
    {
        juegoListo = false;
        LimpiarTablero();

        if (usarEvolutivo && evolutivo == null)
        {
            evolutivo = GetComponent<Evolutivo>() ?? FindObjectOfType<Evolutivo>();
            if (evolutivo == null)
            {
                evolutivo = gameObject.AddComponent<Evolutivo>();
            }
        }

        // 1. Instanciamos la cuadrícula de celdas visuales inmediatamente en pantalla
        tablero = new Tablero(Filas, Columnas);
        for (int Fila = 0; Fila < Filas; Fila++)
        {
            for (int Columna = 0; Columna < Columnas; Columna++)
            {
                Vector3 Posicion = new(Columna - (Columnas / 2) + 3.3f, 0, Fila - (Filas / 2));
                GameObject celdaObj = Instantiate(CeldaPrefab, AreaJuego);
                celdaObj.name = $"Celda_{Fila}_{Columna}";
                celdaObj.transform.localPosition = Posicion;
                celdaObj.transform.localRotation = Quaternion.identity;
                celdaObj.transform.localScale = CeldaPrefab.transform.localScale;

                celdasVisuales[Fila, Columna] = celdaObj.GetComponent<CeldaCs>();
                celdasVisuales[Fila, Columna].init(this, Fila, Columna);
            }
        }
        tablero.Celdas = celdasVisuales;

        if (!usarEvolutivo)
        {
            // Búsqueda simple paso a paso de un tablero resoluble
            bool resoluble = false;
            int intento = 0;
            Debug.Log("--- Iniciando búsqueda paso a paso de tablero resoluble ---");

            while (!resoluble)
            {
                intento++;
                Tablero candidato = Rscript.GenStart(Filas, Columnas);
                candidato.Celdas = celdasVisuales;

                MostrarTableroEnPantalla(candidato);
                Debug.Log($"[Intento {intento}] Mostrando tablero candidato en pantalla...");

                yield return new WaitForSeconds(tiempoEntreIntentos);

                if (Annealing.Procesar(candidato))
                {
                    tablero = candidato;
                    tablero.Celdas = celdasVisuales;
                    resoluble = true;
                    Debug.Log($"<color=green>¡Tablero resoluble encontrado en el intento {intento} con {tablero.NPulsaciones} pulsaciones!</color>");
                }
                else
                {
                    Debug.Log($"<color=orange>[Intento {intento}]</color> Tablero descartado. Probando siguiente...");
                }
            }
        }
        else
        {
            // Búsqueda con Estrategia Evolutiva paso a paso
            Debug.Log("--- Generando población inicial paso a paso ---");
            List<Tablero> poblacion = new List<Tablero>();
            int muValido = Mathf.Max(1, mu);
            int lambdaValido = Mathf.Max(1, lambda);
            int totalIndividuos = muValido + lambdaValido;
            int intentoGlobal = 0;

            for (int i = 0; i < totalIndividuos; i++)
            {
                bool individuoValido = false;
                int intentosLocales = 0;

                while (!individuoValido)
                {
                    intentoGlobal++;
                    intentosLocales++;

                    Tablero candidato = Rscript.GenStart(Filas, Columnas);
                    candidato.Celdas = celdasVisuales;

                    MostrarTableroEnPantalla(candidato);
                    Debug.Log($"[Población {i + 1}/{totalIndividuos}] Probando candidato (Intento #{intentoGlobal})...");

                    yield return new WaitForSeconds(tiempoEntreIntentos);

                    if (Annealing.Procesar(candidato))
                    {
                        poblacion.Add(candidato);
                        individuoValido = true;
                        Debug.Log($"<color=cyan>[Población {i + 1}/{totalIndividuos}] ¡Individuo válido encontrado! (Fitness: {candidato.NPulsaciones} pulsaciones)</color>");
                    }
                    else if (intentosLocales >= 100 && poblacion.Count > 0)
                    {
                        Debug.LogWarning($"[Población] Se alcanzaron 100 intentos para el individuo {i + 1}. Avanzando con {poblacion.Count} individuos encontrados.");
                        break;
                    }
                }
                /*
                if (intentosLocales >= 100 && poblacion.Count > 0)
                {
                    break;
                }
                */
            }

            Debug.Log("--- Iniciando evolución paso a paso ---");
            Tablero mejorEvolutivo = null;

            if (evolutivo != null && poblacion.Count > 0)
            {
                yield return StartCoroutine(evolutivo.EjecutarPasoAPaso(
                    poblacion, muValido, lambdaValido, Filas, Columnas, tiempoEntreIntentos,
                    (tableroActual, mensaje) =>
                    {
                        MostrarTableroEnPantalla(tableroActual);
                        Debug.Log(mensaje);
                    },
                    (tableroFinal) =>
                    {
                        mejorEvolutivo = tableroFinal;
                    }
                ));
            }
            else if (poblacion.Count > 0)
            {
                mejorEvolutivo = MejorDe(poblacion);
            }

            if (mejorEvolutivo != null)
            {
                tablero = mejorEvolutivo;
                tablero.Celdas = celdasVisuales;
                MostrarTableroEnPantalla(tablero);
            }
        }

        // El Tablero esta listo para que el usuario pueda empezar a jugar
        juegoListo = true;
        rutinaGeneracion = null;
        Debug.Log("<color=green>¡Tablero final listo! Ya puedes jugar.</color>");
    }

    private void MostrarTableroEnPantalla(Tablero t)
    {
        if (t == null || celdasVisuales == null) return;
        for (int f = 0; f < Filas; f++)
        {
            for (int c = 0; c < Columnas; c++)
            {
                if (celdasVisuales[f, c] != null)
                {
                    celdasVisuales[f, c].SetEstado(t.Luces[f, c]);
                }
            }
        }
    }

    private Tablero MejorDe(List<Tablero> poblacion)
    {
        if (poblacion == null || poblacion.Count == 0) return null;
        Tablero mejor = poblacion[0];
        foreach (Tablero t in poblacion)
            if (t.NPulsaciones > mejor.NPulsaciones) mejor = t;
        return mejor;
    }
}
