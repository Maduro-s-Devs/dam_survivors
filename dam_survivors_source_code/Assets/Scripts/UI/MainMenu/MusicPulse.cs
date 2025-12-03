using UnityEngine;
using System.Collections.Generic;

public class MusicPulse : MonoBehaviour
{
    [Header("Ritmo (BPM)")]
    public float bpm = 120f;

    [Header("Elementos que hacen 'Bop'")]
    public List<RectTransform> uiElements;

    [Header("Configuración del Golpe")]
    [Tooltip("Cuánto crece de golpe (1.1 = 10% más grande)")]
    public float pumpScale = 1.1f; 
    
    [Tooltip("Qué tan rápido vuelve a su tamaño normal. Más alto = Más seco.")]
    public float returnSpeed = 10f; 

    // Variables internas
    private float beatInterval;
    private float timer;
    private List<Vector3> originalScales = new List<Vector3>();

    void Start()
    {
        // 1. Calculamos cada cuánto tiempo ocurre un beat (en segundos)
        beatInterval = 60f / bpm;

        // 2. Guardamos los tamaños originales
        foreach (var element in uiElements)
        {
            if (element != null)
                originalScales.Add(element.localScale);
        }
    }

    void Update()
    {
        // --- DETECCIÓN DEL BEAT ---
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer -= beatInterval; // Reseteamos el temporizador manteniendo la precisión
            ApplyBeat(); // ¡GOLPE!
        }

        // --- RECUPERACIÓN SUAVE (El regreso) ---
        // En cada frame, hacemos que los objetos vuelvan a su tamaño original
        for (int i = 0; i < uiElements.Count; i++)
        {
            if (uiElements[i] != null)
            {
                // Lerp: Mueve el tamaño actual hacia el original rápidamente
                uiElements[i].localScale = Vector3.Lerp(
                    uiElements[i].localScale, 
                    originalScales[i], 
                    Time.deltaTime * returnSpeed
                );
            }
        }
    }

    // Esta función se ejecuta UNA sola vez justo en el golpe del beat
    void ApplyBeat()
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            if (uiElements[i] != null)
            {
                // Pone el tamaño al MÁXIMO instantáneamente (sin transición)
                // Esto crea el efecto de impacto seco
                uiElements[i].localScale = originalScales[i] * pumpScale;
            }
        }
    }
}