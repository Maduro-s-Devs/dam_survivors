using UnityEngine;
using TMPro; // Necesario para el texto
using UnityEngine.Events; // Para conectar con Oleadas/Boss

public class GameTimer : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Configuración")]
    [SerializeField] private float maxTimeInSeconds = 600f; // 10 Minutos (600s)
    [SerializeField] private float panicTime = 540f;        // 9 Minutos (Empieza a parpadear)

    [Header("Eventos (Para Oleadas y Boss)")]
    public UnityEvent OnTimerEnd; 

    // Propiedad pública para que el WaveManager lea el tiempo
    public float CurrentTime { get; private set; }

    private bool isRunning = true;
    private bool isPanicMode = false;

    void Start()
    {
        CurrentTime = 0f;
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

        Debug.Log("¡TIEMPO AGOTADO! LLEGADA DEL JEFE.");

        OnTimerEnd.Invoke();
    }
}