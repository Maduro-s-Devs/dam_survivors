using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    [Header("Animación de Escala")]
    public float maxScale = 1.5f;        // Tamaño máximo del "POP"
    public float flashDuration = 0.1f;   // Fase 1: Crecer (Blanco)
    public float settleDuration = 0.1f;  // Fase 2: Volver a tamaño normal (Color)
    
    [Header("Vida")]
    public float lifeTime = 0.8f;        // Tiempo total en pantalla
    public float fadeDuration = 0.3f;    // Cuánto tarda en desvanecerse al final

    [Header("Movimiento")]
    public float floatSpeed = 1.5f;      // Velocidad de flotación hacia arriba

    // Variables internas
    private Color targetColor;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(string digitChar, Color color)
    {
        textMesh.text = digitChar;
        targetColor = color;
        
        // ESTADO INICIAL
        timer = 0f;
        transform.localScale = Vector3.zero; // Nace invisible
        textMesh.color = Color.white;        // Nace Blanco
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 1. MOVIMIENTO CONSTANTE (Hacia arriba)
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // 2. LÓGICA DE FASES
        
        // --- FASE 1: POP UP (Crecer + Blanco) ---
        if (timer < flashDuration)
        {
            float t = timer / flashDuration;
            // De 0 a 1.5 (Grande)
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, t);
            textMesh.color = Color.white;
        }
        // --- FASE 2: SETTLE (Volver a Normal + Colorear) ---
        else if (timer < flashDuration + settleDuration)
        {
            float t = (timer - flashDuration) / settleDuration;
            // De 1.5 a 1.0 (Tamaño Normal)
            transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one, t);
            // De Blanco a Amarillo
            textMesh.color = Color.Lerp(Color.white, targetColor, t);
        }
        // --- FASE 3: SUSTAIN & FADE (Mantenerse y Desvanecer) ---
        else
        {
            // Aseguramos que la escala es 1 (Normal)
            transform.localScale = Vector3.one;

            // ¿Toca desvanecerse?
            if (timer > lifeTime - fadeDuration)
            {
                // Calculamos el alpha (1 -> 0)
                float fadeTimeStart = lifeTime - fadeDuration;
                float t = (timer - fadeTimeStart) / fadeDuration;
                
                Color c = targetColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                textMesh.color = c;
            }
            else
            {
                // Si aún no toca desvanecerse, color sólido
                textMesh.color = targetColor;
            }
        }

        // 3. MUERTE
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if(Camera.main != null)
            transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}