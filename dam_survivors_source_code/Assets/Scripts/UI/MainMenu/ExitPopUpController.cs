using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitPopUpController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject popupPanel;      // El objeto padre "ExitPopUp"
    public RectTransform windowRect;   // La ventana "PopUpWindow"
    public CanvasGroup fadeOverlay;    // El telón negro

    [Header("Referencias Audio")]
    public AudioSource backgroundMusic; // <--- ARRASTRA TU MÚSICA AQUÍ

    [Header("Tiempos")]
    public float animationDuration = 0.5f; 
    public float quitFadeDuration = 1.5f; // Lo he subido a 1.5s para disfrutar más el final dramático

    [Header("Curvas de Movimiento")]
    public AnimationCurve showCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.7f, 1.1f), new Keyframe(1, 1));
    public AnimationCurve hideCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, -0.1f), new Keyframe(1, 1));

    // Variables internas
    private Image backgroundImage;
    private float maxAlpha; 

    void Start()
    {
        if(popupPanel != null)
            backgroundImage = popupPanel.GetComponent<Image>();

        if (backgroundImage != null)
            maxAlpha = backgroundImage.color.a;

        if(fadeOverlay != null)
        {
            fadeOverlay.alpha = 0;
            fadeOverlay.blocksRaycasts = false;
        }

        popupPanel.SetActive(false);
    }

    // --- BOTONES ---

    public void OpenPopUp()
    {
        StopAllCoroutines();
        popupPanel.SetActive(true); 
        StartCoroutine(AnimateWindow(true)); 
    }

    public void ClosePopUp()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateWindow(false)); 
    }

    public void ConfirmExitGame()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAndQuit());
    }

    // --- ANIMACIÓN VENTANA ---
    IEnumerator AnimateWindow(bool opening)
    {
        float timePassed = 0;
        Vector2 startPos = opening ? new Vector2(0, -Screen.height) : Vector2.zero;
        Vector2 endPos = opening ? Vector2.zero : new Vector2(0, -Screen.height);
        float startAlpha = opening ? 0f : maxAlpha;
        float endAlpha = opening ? maxAlpha : 0f;

        if(backgroundImage != null) SetBackgroundAlpha(startAlpha);

        while (timePassed < animationDuration)
        {
            timePassed += Time.unscaledDeltaTime;
            float percentage = timePassed / animationDuration;

            // Curva Movimiento
            float curveValue = opening ? showCurve.Evaluate(percentage) : hideCurve.Evaluate(percentage);
            windowRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, curveValue);

            // Fade Fondo (Lineal)
            if(backgroundImage != null)
            {
                float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, percentage);
                SetBackgroundAlpha(currentAlpha);
            }
            
            yield return null;
        }
        
        windowRect.anchoredPosition = endPos;
        if(backgroundImage != null) SetBackgroundAlpha(endAlpha);

        if (!opening) popupPanel.SetActive(false);
    }

    void SetBackgroundAlpha(float alphaVal)
    {
        Color tempColor = backgroundImage.color;
        tempColor.a = alphaVal;
        backgroundImage.color = tempColor;
    }

    // --- FUNDIDO FINAL (AUDIO + VIDEO) ---
    IEnumerator FadeAndQuit()
    {
        float timePassed = 0;
        fadeOverlay.blocksRaycasts = true; // Bloquea clicks

        // Guardamos el volumen actual para bajarlo suavemente desde ahí
        float startVolume = 0.5f; 
        if(backgroundMusic != null) startVolume = backgroundMusic.volume;

        while (timePassed < quitFadeDuration)
        {
            timePassed += Time.unscaledDeltaTime;
            float t = timePassed / quitFadeDuration;

            // 1. Pantalla a Negro (0 -> 1)
            fadeOverlay.alpha = Mathf.Lerp(0, 1, t);

            // 2. Música a Silencio (Volumen Inicial -> 0)
            if(backgroundMusic != null)
            {
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0, t);
            }
            
            yield return null;
        }

        // Aseguramos final limpio
        fadeOverlay.alpha = 1;
        if(backgroundMusic != null) backgroundMusic.volume = 0;
        Application.Quit();
    }
}