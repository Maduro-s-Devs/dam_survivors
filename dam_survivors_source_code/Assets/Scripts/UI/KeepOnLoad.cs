using UnityEngine;
using UnityEngine.SceneManagement; 

public class KeepCanvasOnLoad : MonoBehaviour
{
    private static KeepCanvasOnLoad instance;

    private void Awake()
    {
        // Singleton para UI: Asegura que no se dupliquen las barras de vida ni el menú
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si cargamos una escena que ya trae un Canvas por defecto,
            // destruimos el nuevo para quedarnos con el nuestro (que tiene los datos actuales)
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Se ejecuta al entrar en la nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BossArena") 
        {
            Debug.Log("DAM SURVIVORS UI: Canvas transferido a la Boss Arena correctamente.");

            // --- OPCIONAL: RECONECTAR CÁMARA ---
            // Si tu Canvas está en modo "Screen Space - Camera", a veces pierde la referencia
            // al cambiar de escena. Descomenta esto si tus barras de vida desaparecen:
            
            
            Canvas canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            {
                canvas.worldCamera = Camera.main;
            }
            
        }
    }
}