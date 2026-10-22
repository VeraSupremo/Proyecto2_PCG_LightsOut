using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GameUI : MonoBehaviour
{
    [Header("Referencias a Scripts")]
    public GameManager gameManager;
    public AnnealingSimulated annealing;
    public Evolutivo evolutivo;

    [Header("Campos de la Interfaz (Input Fields)")]
    public InputField inputTempInicial;
    public InputField inputEnfriamiento;
    public InputField inputTempMinima;
    public InputField inputIntentos;
    public InputField inputtiempoEntreIntentos;
    public InputField inputGeneraciones;

    [Header("Botón")]
    public Button botonIniciar;

    private void OnEnable()
    {
        InicializarUI();
    }

    private void OnValidate()
    {
        AutoconectarReferencias();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                ConfigurarEtiquetasPanel();
                CargarValoresEnUI();
            }
        };
#endif
    }

    private void Start()
    {
        InicializarUI();
    }

    public void InicializarUI()
    {
        AutoconectarReferencias();

        // Poner los nombres de las variables 
        ConfigurarEtiquetasPanel();

        CargarValoresEnUI();

        if (Application.isPlaying)
        {
            ConfigurarEventosUI();
        }
    }

    private void AutoconectarReferencias()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (annealing == null)
            annealing = FindObjectOfType<AnnealingSimulated>();

        if (evolutivo == null)
        {
            evolutivo = FindObjectOfType<Evolutivo>();
            if (evolutivo == null && gameManager != null)
            {
                evolutivo = gameManager.GetComponent<Evolutivo>();
            }
        }

        InputField[] campos = GetComponentsInChildren<InputField>(true);
        if (campos.Length == 0 && transform.root != null)
            campos = transform.root.GetComponentsInChildren<InputField>(true);

        foreach (InputField campo in campos)
        {
            string nombre = campo.gameObject.name.ToLowerInvariant();
            if (nombre.Contains("inicial") && inputTempInicial == null)
                inputTempInicial = campo;
            else if (nombre.Contains("enfri") && inputEnfriamiento == null)
                inputEnfriamiento = campo;
            else if ((nombre.Contains("minima") || nombre.Contains("mínima")) && inputTempMinima == null)
                inputTempMinima = campo;
            else if (nombre.Contains("tiempo") && inputtiempoEntreIntentos == null)
                inputtiempoEntreIntentos = campo;
            else if (nombre.Contains("intento") && !nombre.Contains("tiempo") && inputIntentos == null)
                inputIntentos = campo;
            else if (nombre.Contains("generacion") && inputGeneraciones == null)
                inputGeneraciones = campo;
        }

        if (botonIniciar == null)
        {
            botonIniciar = GetComponentInChildren<Button>(true);
            if (botonIniciar == null && transform.root != null)
                botonIniciar = transform.root.GetComponentInChildren<Button>(true);
        }
    }

    private void ConfigurarEtiquetasPanel()
    {
        CrearOActualizarEtiqueta(inputtiempoEntreIntentos, "Tiempo entre Intentos", "tiempoEntreIntentos");
        CrearOActualizarEtiqueta(inputTempInicial, "Temperatura Inicial", "temperaturaInicial");
        CrearOActualizarEtiqueta(inputEnfriamiento, "Factor Enfriamiento", "factorEnfriamiento");
        CrearOActualizarEtiqueta(inputTempMinima, "Temperatura Mínima", "temperaturaMinima");
        CrearOActualizarEtiqueta(inputIntentos, "Intentos por Temperatura", "intentosPorTemperatura");
        CrearOActualizarEtiqueta(inputGeneraciones, "Generaciones Evolutivas", "generaciones");
    }

    private void CrearOActualizarEtiqueta(InputField campo, string textoEtiqueta, string textoPlaceholder)
    {
        if (campo == null) return;

        // Ajustar el InputField hacia la derecha del Panel para dejar espacio al nombre de la variable a la izquierda
        RectTransform rectCampo = campo.GetComponent<RectTransform>();
        if (rectCampo != null)
        {
            rectCampo.anchoredPosition = new Vector2(82f, rectCampo.anchoredPosition.y);
            rectCampo.sizeDelta = new Vector2(185f, 38f);
        }

        // Actualizar el Placeholder con el nombre de la variable
        if (campo.placeholder is Text placeholderText)
        {
            placeholderText.text = textoPlaceholder;
        }

        // Buscar o crear el objeto Text con el nombre de la variable a la izquierda del InputField
        Transform labelTransform = campo.transform.Find("Label_Variable");
        GameObject labelObj;
        if (labelTransform == null)
        {
            labelObj = new GameObject("Label_Variable", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelObj.transform.SetParent(campo.transform, false);
        }
        else
        {
            labelObj = labelTransform.gameObject;
        }

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = new Vector2(-180f, 0f);
        labelRect.sizeDelta = new Vector2(165f, 42f);
        labelRect.localScale = Vector3.one;

        Text labelText = labelObj.GetComponent<Text>();
        if (campo.textComponent != null && campo.textComponent.font != null)
        {
            labelText.font = campo.textComponent.font;
        }
        else
        {
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null)
                defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            labelText.font = defaultFont;
        }

        labelText.text = textoEtiqueta;
        labelText.fontSize = 13;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        labelText.alignment = TextAnchor.MiddleLeft;
        labelText.horizontalOverflow = HorizontalWrapMode.Wrap;
        labelText.verticalOverflow = VerticalWrapMode.Overflow;
        labelText.raycastTarget = false;
    }

    private void CargarValoresEnUI()
    {
        if (annealing != null)
        {
            if (inputTempInicial != null)
                inputTempInicial.SetTextWithoutNotify(annealing.TemperaturaInicial.ToString(CultureInfo.InvariantCulture));
            if (inputEnfriamiento != null)
                inputEnfriamiento.SetTextWithoutNotify(annealing.FactorEnfriamiento.ToString(CultureInfo.InvariantCulture));
            if (inputTempMinima != null)
                inputTempMinima.SetTextWithoutNotify(annealing.TemperaturaMinima.ToString(CultureInfo.InvariantCulture));
            if (inputIntentos != null)
                inputIntentos.SetTextWithoutNotify(annealing.IntentosPorTemperatura.ToString(CultureInfo.InvariantCulture));
        }

        if (evolutivo != null && inputGeneraciones != null)
        {
            inputGeneraciones.SetTextWithoutNotify(evolutivo.Generaciones.ToString(CultureInfo.InvariantCulture));
        }

        if (gameManager != null && inputtiempoEntreIntentos != null)
        {
            inputtiempoEntreIntentos.SetTextWithoutNotify(gameManager.tiempoEntreIntentos.ToString(CultureInfo.InvariantCulture));
        }
    }

    private void ConfigurarEventosUI()
    {
        if (botonIniciar != null)
        {
            botonIniciar.onClick.RemoveListener(AlPresionarIniciar);
            botonIniciar.onClick.AddListener(AlPresionarIniciar);
        }

        SuscribirInput(inputTempInicial);
        SuscribirInput(inputEnfriamiento);
        SuscribirInput(inputTempMinima);
        SuscribirInput(inputIntentos);
        SuscribirInput(inputtiempoEntreIntentos);
        SuscribirInput(inputGeneraciones);
    }

    private void SuscribirInput(InputField campo)
    {
        if (campo == null) return;
        campo.onEndEdit.RemoveListener(AlEditarCampo);
        campo.onEndEdit.AddListener(AlEditarCampo);
        campo.onValueChanged.RemoveListener(AlEditarCampo);
        campo.onValueChanged.AddListener(AlEditarCampo);
    }

    private void AlEditarCampo(string _)
    {
        AplicarValoresDesdeUI();
    }

    public void AplicarValoresDesdeUI()
    {
        AutoconectarReferencias();

        if (annealing != null)
        {
            if (inputTempInicial != null && TryParseFloat(inputTempInicial.text, out float tempI))
                annealing.TemperaturaInicial = tempI;

            if (inputEnfriamiento != null && TryParseFloat(inputEnfriamiento.text, out float enfri))
                annealing.FactorEnfriamiento = enfri;

            if (inputTempMinima != null && TryParseFloat(inputTempMinima.text, out float tempMin))
                annealing.TemperaturaMinima = tempMin;

            if (inputIntentos != null && int.TryParse(inputIntentos.text.Trim(), out int intentos))
                annealing.IntentosPorTemperatura = intentos;
        }

        if (evolutivo != null && inputGeneraciones != null && int.TryParse(inputGeneraciones.text.Trim(), out int gen))
        {
            evolutivo.Generaciones = Mathf.Max(1, gen);
        }

        if (gameManager != null && inputtiempoEntreIntentos != null && TryParseFloat(inputtiempoEntreIntentos.text, out float tiempo))
        {
            gameManager.tiempoEntreIntentos = Mathf.Max(0.01f, tiempo);
        }
    }

    public void AlPresionarIniciar()
    {
        // 1. Leer los valores de los inputs
        AplicarValoresDesdeUI();

        // 2. Iniciar el juego
        if (gameManager != null)
        {
            Debug.Log($"[GameUI] Iniciando con: TempInicial={annealing?.TemperaturaInicial}, Enfriamiento={annealing?.FactorEnfriamiento}, TempMin={annealing?.TemperaturaMinima}, Intentos={annealing?.IntentosPorTemperatura}, Generaciones={evolutivo?.Generaciones}, TiempoEntreIntentos={gameManager.tiempoEntreIntentos}");
            gameManager.IniciarGame();
        }
    }

    private static bool TryParseFloat(string texto, out float valor)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            valor = 0f;
            return false;
        }

        string normalizado = texto.Trim().Replace(',', '.');
        return float.TryParse(normalizado, NumberStyles.Float, CultureInfo.InvariantCulture, out valor);
    }
}
