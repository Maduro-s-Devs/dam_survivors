using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuEntryAnimation : MonoBehaviour
{
    [Header("Configuración de Elementos")]
    [Tooltip("Elementos que entrarán volando desde la IZQUIERDA")]
    public List<RectTransform> enterFromLeft;

    [Tooltip("Elementos que entrarán volando desde la DERECHA")]
    public List<RectTransform> enterFromRight;

    [Header("Configuración de Animación")]
    public float duration = 1.0f; // Tiempo que tarda en llegar al sitio
    public float startDelay = 0.2f; // Pequeña pausa antes de empezar para que no sea brusco

    [Tooltip("Dibuja aquí cómo se mueven. Recomendado: Que suba rápido y se pase un poco (Rebote)")]
    public AnimationCurve motionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.7f, 1.1f), new Keyframe(1, 1));

    // Diccionario para recordar dónde tenía que estar cada cosa
    private Dictionary<RectTransform, Vector2> finalPositions = new Dictionary<RectTransform, Vector2>();

    void Awake()
    {
        // 1. ANTES DE NADA: Guardar posiciones finales y mandar todo fuera de la pantalla
        // Lo hacemos en Awake para que el jugador no vea los botones en su sitio ni un milisegundo

        // Procesar lista Izquierda
        foreach (var item in enterFromLeft)
        {
            if (item != null)
            {
                finalPositions[item] = item.anchoredPosition; // Guardamos destino
                // Lo movemos lejos a la izquierda (fuera de pantalla)
                item.anchoredPosition = new Vector2(-Screen.width, item.anchoredPosition.y);
            }
        }

        // Procesar lista Derecha
        foreach (var item in enterFromRight)
        {
            if (item != null)
            {
                finalPositions[item] = item.anchoredPosition; // Guardamos destino
                // Lo movemos lejos a la derecha
                item.anchoredPosition = new Vector2(Screen.width, item.anchoredPosition.y);
            }
        }
    }

    IEnumerator Start()
    {
        // Esperamos un poquito (opcional) para dar tiempo a que cargue la escena
        yield return new WaitForSeconds(startDelay);

        float timePassed = 0;

        while (timePassed < duration)
        {
            timePassed += Time.unscaledDeltaTime; // Unscaled para que funcione aunque el juego esté en pausa
            float percentage = timePassed / duration;

            // Evaluamos la curva (0 a 1)
            float curveValue = motionCurve.Evaluate(percentage);

            // ANIMAR IZQUIERDA
            foreach (var item in enterFromLeft)
            {
                if (item != null)
                {
                    // Posición inicial: Fuera a la izq (-Screen.width)
                    // Posición final: Su sitio original guardado en el diccionario
                    Vector2 startPos = new Vector2(-Screen.width, finalPositions[item].y);
                    
                    // LerpUnclamped permite que la curva se pase de 1 (Rebote)
                    item.anchoredPosition = Vector2.LerpUnclamped(startPos, finalPositions[item], curveValue);
                }
            }

            // ANIMAR DERECHA
            foreach (var item in enterFromRight)
            {
                if (item != null)
                {
                    // Posición inicial: Fuera a la der (+Screen.width)
                    Vector2 startPos = new Vector2(Screen.width, finalPositions[item].y);
                    
                    item.anchoredPosition = Vector2.LerpUnclamped(startPos, finalPositions[item], curveValue);
                }
            }

            yield return null;
        }

        // ASEGURAR POSICIONES FINALES EXACTAS
        // Esto evita errores de flotantes o que queden un pixel movidos
        foreach (var kvp in finalPositions)
        {
            if (kvp.Key != null)
                kvp.Key.anchoredPosition = kvp.Value;
        }
    }
}