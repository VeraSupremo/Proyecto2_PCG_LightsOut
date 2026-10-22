using UnityEngine;

public class CeldaCs : MonoBehaviour
{
    [Header("Tipo de celda")]
    [SerializeField] private Color OnColor  = Color.yellow;
    [SerializeField] private Color OffColor = Color.white;

    private Renderer rend;
    private bool Activo = false;


    private GameManager gameManager;
    private int fila, columna;

    public void init(GameManager gameManager, int fila, int columna)
    {
        this.gameManager = gameManager;
        this.fila = fila;
        this.columna = columna;
    }
    
    void Awake()
    {
        rend = GetComponent<Renderer>();

        //instanciar el material
        rend.material = new Material(rend.material);
    }

    void Start() => UpdateColor();

    void UpdateColor()
    {
        if (Activo)
        {
            rend.material.color = OnColor;
        }
        else
        {
            rend.material.color = OffColor;
        }
        
    }

    public void CambiarEstado()
    {
        Activo = !Activo;
        UpdateColor();
    }

    public void SetEstado(bool estado)
    {
        Activo = estado;
        UpdateColor();
    }

    public bool estaActivo() => Activo;

    private void OnMouseDown() => gameManager.CambiarEstado(fila, columna);
}