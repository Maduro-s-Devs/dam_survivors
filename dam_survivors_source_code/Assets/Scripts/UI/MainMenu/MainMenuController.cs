using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para la Imagen Negra
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup fadeOverlay; // El panel negro (Canvas Group)
    
    [Header("Referencias Audio")]
    public AudioSource backgroundMusic; // Para bajar el volumen al irnos

    [Header("Configuración")]
    public float fadeDuration = 1.0f; // Cuánto tarda en irse a negro
    public string gameSceneName = "Gameplay";
    public string optionsSceneName = "OptionMenu";

    private void Start()
    {
        // Al empezar, nos aseguramos de que el negro sea transparente y no bloquee clicks
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0;
            fadeOverlay.blocksRaycasts = false;
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void JugarPartida()
    {
        // Inicia la rutina para ir al Juego
        StartCoroutine(FadeAndLoadScene(gameSceneName));
    }

    public void IrAOptions()
    {
        // Inicia la rutina para ir a Opciones
        StartCoroutine(FadeAndLoadScene(optionsSceneName));
    }

    public void SalirJuego()
    {
        // Este lo dejamos simple porque ya tienes el ExitPopUp que hace lo suyo.
        // Si no usaras el PopUp, aquí pondrías el Application.Quit();
        Debug.Log("Abriendo PopUp de Salida...");
    }

    // --- LA CORUTINA DE TRANSICIÓN ---
    IEnumerator FadeAndLoadScene(string sceneToLoad)
    {
        // 1. Bloqueamos clicks para que no le den a nada más
        if (fadeOverlay != null) fadeOverlay.blocksRaycasts = true;

        float timePassed = 0;
        float startVolume = (backgroundMusic != null) ? backgroundMusic.volume : 1f;

        while (timePassed < fadeDuration)
        {
            timePassed += Time.unscaledDeltaTime;
            float t = timePassed / fadeDuration;

            // Fade In del Negro (0 -> 1)
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

        // 2. ¡EL SALTO HIPERESPACIAL!
        SceneManager.LoadScene(sceneToLoad);
    }
}