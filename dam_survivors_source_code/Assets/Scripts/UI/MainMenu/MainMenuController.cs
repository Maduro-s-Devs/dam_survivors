using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup fadeOverlay; // El panel negro

    [Header("Referencias Audio")]
    public AudioSource backgroundMusic; 

    [Header("Configuración")]
    public float fadeDuration = 1.0f; // Tiempo para el Fade In y Fade Out
    public string gameSceneName = "Gameplay";
    public string optionsSceneName = "OptionMenu";

    private void Start()
    {
        // Al empezar, iniciamos la rutina de "Aclarar la pantalla" (Fade In)
        StartCoroutine(FadeInRoutine());
    }

    // --- CORUTINA DE ENTRADA (FADE IN) ---
    IEnumerator FadeInRoutine()
    {
        // 1. Empezamos con la pantalla totalmente negra y bloqueando clicks
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 1; 
            fadeOverlay.blocksRaycasts = true; // Bloquea hasta que termine de aclararse
        }

        // Opcional: Si quieres que la música empiece floja y suba, configúralo aquí
        float targetVolume = (backgroundMusic != null) ? backgroundMusic.volume : 1f;
        if(backgroundMusic != null) backgroundMusic.volume = 0f;

        float timePassed = 0;

        while (timePassed < fadeDuration)
        {
            timePassed += Time.unscaledDeltaTime;
            float t = timePassed / fadeDuration;

            // De 1 (Negro) a 0 (Transparente)
            if (fadeOverlay != null) 
                fadeOverlay.alpha = Mathf.Lerp(1, 0, t);

            // De 0 (Silencio) a Volumen Original
            if (backgroundMusic != null)
                backgroundMusic.volume = Mathf.Lerp(0, targetVolume, t);

            yield return null;
        }

        // 2. Finalizar: Asegurar que se ve el juego y se puede clicar
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0;
            fadeOverlay.blocksRaycasts = false; // ¡IMPORTANTE! Desbloquear el ratón
        }
        
        if (backgroundMusic != null) backgroundMusic.volume = targetVolume;
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void JugarPartida()
    {
        StartCoroutine(FadeAndLoadScene(gameSceneName));
    }

    public void IrAOptions()
    {
        StartCoroutine(FadeAndLoadScene(optionsSceneName));
    }

    public void SalirJuego()
    {
        Debug.Log("Abriendo PopUp de Salida...");
        // (Aquí va tu lógica del PopUp o Application.Quit)
    }

    // --- CORUTINA DE SALIDA (FADE OUT) ---
    IEnumerator FadeAndLoadScene(string sceneToLoad)
    {
        // 1. Bloqueamos clicks para que no le den a nada más mientras nos vamos
        if (fadeOverlay != null) fadeOverlay.blocksRaycasts = true;

        float timePassed = 0;
        float startVolume = (backgroundMusic != null) ? backgroundMusic.volume : 1f;

        while (timePassed < fadeDuration)
        {
            timePassed += Time.unscaledDeltaTime;
            float t = timePassed / fadeDuration;

            // Fade In del Negro (0 -> 1) para tapar la pantalla
            if (fadeOverlay != null) 
                fadeOverlay.alpha = Mathf.Lerp(0, 1, t);

            // Fade Out de la Música (Volumen -> 0)
            if (backgroundMusic != null) 
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0, t);

            yield return null;
        }

        // Aseguramos valores finales
        if (fadeOverlay != null) fadeOverlay.alpha = 1;
        if (backgroundMusic != null) backgroundMusic.volume = 0;

        // 2. CAMBIO DE ESCENA
        SceneManager.LoadScene(sceneToLoad);
    }
}