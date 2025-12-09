using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreScreenIntro : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform topText;
    public RectTransform bottomText;
    public CanvasGroup backgroundCanvasGroup;

    [Header("---- TEXTO DE ARRIBA (Configuración) ----")]
    [Tooltip("Segundo exacto en que comienza a entrar")]
    public float topEnterTime = 1.0f;
    [Tooltip("Segundo exacto en que comienza a irse")]
    public float topExitTime = 5.0f;

    [Header("---- TEXTO DE ABAJO (Configuración) ----")]
    [Tooltip("Segundo exacto en que comienza a entrar")]
    public float bottomEnterTime = 2.0f;
    [Tooltip("Segundo exacto en que comienza a irse")]
    public float bottomExitTime = 6.0f;

    [Header("Animación General")]
    public float moveDuration = 1.0f; // Cuánto tardan en desplazarse
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Flotación (Idle)")]
    public float idleAmplitude = 10f; 
    public float idleSpeed = 2f;

    // Variables internas
    private float timer = 0f;
    private float screenWidth;

    // Estados independientes
    private bool topEntered = false;
    private bool topExited = false;
    private bool bottomEntered = false;
    private bool bottomExited = false;

    // Posiciones
    private Vector2 topFinalPos, topStartPos, topExitPos;
    private Vector2 bottomFinalPos, bottomStartPos, bottomExitPos;

    void Start()
    {
        screenWidth = Screen.width;

        // 1. Configurar Posiciones Objetivo (Centro)
        topFinalPos = topText.anchoredPosition;
        bottomFinalPos = bottomText.anchoredPosition;

        // 2. Configurar Posiciones Iniciales (Fuera pantalla)
        topStartPos = new Vector2(screenWidth + 400, topFinalPos.y);        // Derecha
        bottomStartPos = new Vector2(-screenWidth - 400, bottomFinalPos.y); // Izquierda

        // 3. Configurar Posiciones de Salida (Lado opuesto)
        topExitPos = new Vector2(-screenWidth - 400, topFinalPos.y);        // Izquierda
        bottomExitPos = new Vector2(screenWidth + 400, bottomFinalPos.y);   // Derecha

        // 4. Colocar textos fuera inmediatamente
        topText.anchoredPosition = topStartPos;
        bottomText.anchoredPosition = bottomStartPos;

        if (backgroundCanvasGroup != null) backgroundCanvasGroup.alpha = 1f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // --- LÓGICA DEL TEXTO DE ARRIBA ---
        
        // Entrar
        if (!topEntered && timer >= topEnterTime)
        {
            StartCoroutine(AnimateMove(topText, topStartPos, topFinalPos));
            topEntered = true;
        }

        // Salir
        if (!topExited && timer >= topExitTime)
        {
            StartCoroutine(AnimateMove(topText, topText.anchoredPosition, topExitPos));
            topExited = true;
        }

        // Flotar (Solo si ya entró y no ha salido)
        if (topEntered && !topExited && !IsAnimating(topText))
        {
            ApplyIdle(topText, topFinalPos, 1f);
        }


        // --- LÓGICA DEL TEXTO DE ABAJO ---

        // Entrar
        if (!bottomEntered && timer >= bottomEnterTime)
        {
            StartCoroutine(AnimateMove(bottomText, bottomStartPos, bottomFinalPos));
            bottomEntered = true;
        }

        // Salir
        if (!bottomExited && timer >= bottomExitTime)
        {
            StartCoroutine(AnimateMove(bottomText, bottomText.anchoredPosition, bottomExitPos));
            bottomExited = true;
        }

        // Flotar (Solo si ya entró y no ha salido)
        if (bottomEntered && !bottomExited && !IsAnimating(bottomText))
        {
            ApplyIdle(bottomText, bottomFinalPos, -1f); // -1 para movimiento inverso
        }

        // --- LIMPIEZA FINAL ---
        // Si ambos ya salieron y la animación terminó, destruir
        if (topExited && bottomExited && timer > (Mathf.Max(topExitTime, bottomExitTime) + moveDuration))
        {
            // Opcional: Fade out del fondo antes de destruir
            if (backgroundCanvasGroup != null) backgroundCanvasGroup.alpha -= Time.deltaTime;
            
            if (backgroundCanvasGroup == null || backgroundCanvasGroup.alpha <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // Corrutina genérica de movimiento
    IEnumerator AnimateMove(RectTransform target, Vector2 from, Vector2 to)
    {
        // Marcamos el objeto como "animando" usando un tag temporal o variable simple?
        // Usaremos la posición para saberlo, o simplemente confiamos en el timer.
        
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / moveDuration;
            float curve = movementCurve.Evaluate(percent);

            target.anchoredPosition = Vector2.Lerp(from, to, curve);
            yield return null;
        }
        target.anchoredPosition = to;
    }

    // Función pequeña para aplicar flotación
    void ApplyIdle(RectTransform target, Vector2 basePos, float direction)
    {
        float yOffset = Mathf.Sin(Time.time * idleSpeed) * idleAmplitude * direction;
        target.anchoredPosition = new Vector2(basePos.x, basePos.y + yOffset);
    }

    // Helper simple para evitar que el Idle pelee con la Corrutina
    bool IsAnimating(RectTransform target)
    {
        // Una forma sencilla es chequear si estamos cerca de los tiempos de transición
        // Si el tiempo actual está dentro del rango de "moverse", devolvemos true.
        
        // Rango entrada Top
        if (target == topText)
        {
            bool animatingIn = (timer >= topEnterTime && timer < topEnterTime + moveDuration);
            bool animatingOut = (timer >= topExitTime && timer < topExitTime + moveDuration);
            return animatingIn || animatingOut;
        }
        // Rango entrada Bottom
        else if (target == bottomText)
        {
            bool animatingIn = (timer >= bottomEnterTime && timer < bottomEnterTime + moveDuration);
            bool animatingOut = (timer >= bottomExitTime && timer < bottomExitTime + moveDuration);
            return animatingIn || animatingOut;
        }
        return false;
    }
}