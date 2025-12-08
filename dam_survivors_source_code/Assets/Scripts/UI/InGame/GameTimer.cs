using UnityEngine;
using TMPro; 
using UnityEngine.Events; 
using UnityEngine.SceneManagement; // Necesario para cambiar de escena ---

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Configuration")]
    [SerializeField] private float maxTimeInSeconds = 600f; // 10 Minutos (600s)
    [SerializeField] private float panicTime = 540f;        // 9 Minutos (Empieza a parpadear)

    [Header("Boss Transition")]
    [SerializeField] private string bossSceneName = "BossArena"; // Nombre de la escena a cargar
    public UnityEvent OnTimerEnd; 

    [Header("Debug ")]
    [Tooltip("Pon aquí 590 para empezar casi al final y no esperar 10 min.")]
    [SerializeField] private float debugStartTime = 0f; 

    // Propiedad pública para que el WaveManager lea el tiempo
    public float CurrentTime { get; private set; }

    private bool isRunning = true;
    private bool isPanicMode = false;

    void Start()
    {
        // CAMBIO: Ahora inicia en el tiempo que tú le digas (0 por defecto)
        CurrentTime = debugStartTime; 
        
        if(timerText != null) timerText.color = Color.white;
    }

    void Update()
    {
        if (!isRunning) return;

        CurrentTime += Time.deltaTime;

        UpdateTimerDisplay();

        if (CurrentTime >= panicTime && !isPanicMode)
        {
            isPanicMode = true;
        }

        if (isPanicMode)
        {
            FlashTimerEffect();
        }

        if (CurrentTime >= maxTimeInSeconds)
        {
            FinishTimer();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(CurrentTime / 60); 
        int seconds = Mathf.FloorToInt(CurrentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void FlashTimerEffect()
    {
        if (timerText == null) return;

        float t = Mathf.PingPong(Time.time * 5f, 1f);
        
        timerText.color = Color.Lerp(Color.white, Color.red, t);
    }

    void FinishTimer()
    {
        isRunning = false;
        CurrentTime = maxTimeInSeconds;
        
        if (timerText != null) 
        {
            timerText.color = Color.red;
            timerText.text = "10:00";
        }

        Debug.Log("¡TIEMPO AGOTADO! VIAJANDO A LA ARENA DEL BOSS...");

        // Ejecutamos eventos previos (por si quieres guardar partida o lanzar sonido)
        OnTimerEnd.Invoke();

        // CAMBIO DE ESCENA 
        // Asegúrate de que la escena "BossArena" esté en File -> Build Settings
        SceneManager.LoadScene(bossSceneName);
    }
    
    // Haz click derecho en el script (en el inspector) y dale a "Saltar al Final"
    [ContextMenu("Start END")]
    public void DebugSkipToEnd()
    {
        CurrentTime = maxTimeInSeconds - 5f;
    }
}