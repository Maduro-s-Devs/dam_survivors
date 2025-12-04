using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject pausePanel;       
    public CanvasGroup backgroundGroup; 
    public RectTransform leftContent;   
    public RectTransform rightContent;  

    [Header("Transición de Salida (Fade Out)")]
    public CanvasGroup transitionOverlay; // <--- ARRASTRA AQUÍ TU NUEVO PANEL NEGRO
    public float fadeOutDuration = 1.0f;  // Tiempo para irse a negro

    [Header("Animación Entrada")]
    public float animationSpeed = 0.5f; 
    public AnimationCurve entryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

    // Variable estática
    public static bool IsGamePaused = false;

    private Controls controls;
    private float screenWidth;

    private void Awake()
    {
        controls = new Controls();
        controls.UI.Pause.performed += context => TogglePause();
        screenWidth = Screen.width; 
    }

    private void Start()
    {
        if(pausePanel != null) pausePanel.SetActive(false);
        
        if(backgroundGroup != null)
        {
            backgroundGroup.alpha = 0f;
            backgroundGroup.blocksRaycasts = false;
        }
        
        if(transitionOverlay != null)
        {
            transitionOverlay.alpha = 0f;
            transitionOverlay.blocksRaycasts = false;
        }
    }

    private void OnEnable() { controls.Enable(); }
    private void OnDisable() { controls.Disable(); }

    // --- LÓGICA PRINCIPAL ---

    public void TogglePause()
    {
        if (IsGamePaused) DeactivatePause();
        else ActivatePause();
    }

    void ActivatePause()
    {
        IsGamePaused = true;
        Time.timeScale = 0f; 
        AudioListener.pause = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            
            // --- NUEVO: Activamos el bloqueo de ratón ---
            if (backgroundGroup != null) backgroundGroup.blocksRaycasts = true; // <--- AHORA SÍ BLOQUEA

            StopAllCoroutines();
            StartCoroutine(AnimateMenu(true));
        }
    }

    public void DeactivatePause()
    {
        if (pausePanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateMenu(false));
        }
    }

    // --- ANIMACIÓN MENÚ (Entrada/Salida visual) ---
    IEnumerator AnimateMenu(bool isEntering)
    {
        float timer = 0f;
        
        // Configuración de posiciones (Igual que antes)
        Vector2 leftStart = isEntering ? new Vector2(-screenWidth, 0) : Vector2.zero;
        Vector2 leftEnd   = isEntering ? Vector2.zero : new Vector2(-screenWidth, 0);
        Vector2 rightStart = isEntering ? new Vector2(screenWidth, 0) : Vector2.zero;
        Vector2 rightEnd   = isEntering ? Vector2.zero : new Vector2(screenWidth, 0);
        float alphaStart = isEntering ? 0f : 1f;
        float alphaEnd   = isEntering ? 1f : 0f;

        if (isEntering)
        {
            if(backgroundGroup) backgroundGroup.alpha = 0f;
            if(leftContent) leftContent.anchoredPosition = leftStart;
            if(rightContent) rightContent.anchoredPosition = rightStart;
        }

        while (timer < animationSpeed)
        {
            timer += Time.unscaledDeltaTime; 
            float percentage = Mathf.Clamp01(timer / animationSpeed);
            float curveValue = entryCurve.Evaluate(percentage);

            if(backgroundGroup) backgroundGroup.alpha = Mathf.Lerp(alphaStart, alphaEnd, percentage);
            if(leftContent) leftContent.anchoredPosition = Vector2.LerpUnclamped(leftStart, leftEnd, curveValue);
            if(rightContent) rightContent.anchoredPosition = Vector2.LerpUnclamped(rightStart, rightEnd, curveValue);

            yield return null;
        }

        // Finalizar reanudación
        if (!isEntering) // Si estamos SALIENDO (Cerrando pausa)
        {
            if (backgroundGroup != null) backgroundGroup.blocksRaycasts = false; // <--- YA NO MOLESTA

            pausePanel.SetActive(false); 
            
            Time.timeScale = 1f; 
            IsGamePaused = false;
            AudioListener.pause = false;
        }
        
        else
        {
            // Asegurar posiciones finales
            if(leftContent) leftContent.anchoredPosition = Vector2.zero;
            if(rightContent) rightContent.anchoredPosition = Vector2.zero;
        }
    }

    // --- NUEVO: SALIR AL MENÚ CON FADE OUT ---
    public void GoToMainMenu()
    {
        StartCoroutine(FadeAndExitRoutine());
    }

    IEnumerator FadeAndExitRoutine()
    {
        // 1. Activar el telón negro
        if (transitionOverlay != null)
        {
            transitionOverlay.gameObject.SetActive(true);
            transitionOverlay.blocksRaycasts = true; // Bloquea clicks extra
        }

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime; // Usamos unscaled porque el juego sigue pausado
            float t = Mathf.Clamp01(timer / fadeOutDuration);

            // Fade In del negro
            if (transitionOverlay != null) transitionOverlay.alpha = t;

            yield return null;
        }

        // 2. ¡CRUCIAL! Devolver el tiempo a la normalidad ANTES de cambiar de escena
        // Si no lo haces, el menú principal cargará congelado.
        Time.timeScale = 1f; 
        IsGamePaused = false;
        AudioListener.pause = false;

        // 3. Cargar escena
        SceneManager.LoadScene("MainMenu");
    }
}