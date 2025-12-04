using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFeedback : MonoBehaviour
{
    [Header("Sangre (Overlay)")]
    public CanvasGroup bloodOverlay; // Arrastra el objeto con CanvasGroup
    public float fadeSpeed = 2f;     // Qué tan rápido desaparece la sangre

    [Header("Temblor de Cámara (Shake)")]
    public Transform cameraTransform; // Arrastra la Main Camera (el HIJO)
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f; // Qué tan fuerte tiembla

    // Variables internas
    private Vector3 originalLocalPos;
    private float currentShakeTime;
    private float overlayAlpha;

    void Start()
    {
        // Guardamos la posición local (0,0,0 normalmente) para volver a ella
        if (cameraTransform != null)
            originalLocalPos = cameraTransform.localPosition;
            
        if (bloodOverlay != null)
        {
            bloodOverlay.alpha = 0f;
            overlayAlpha = 0f;
        }
    }

    void Update()
    {
        // --- EFECTO SANGRE (Fade Out) ---
        if (bloodOverlay != null && overlayAlpha > 0)
        {
            // Reducimos el alpha poco a poco
            overlayAlpha -= Time.deltaTime * fadeSpeed;
            bloodOverlay.alpha = overlayAlpha;
        }
    }

    // --- ESTA ES LA FUNCIÓN QUE LLAMARÁS ---
    public void TriggerDamageEffect()
    {
        // 1. Activar Sangre
        overlayAlpha = 1f; // Opacidad máxima instantánea
        if (bloodOverlay != null) bloodOverlay.alpha = 1f;

        // 2. Activar Temblor
        StopAllCoroutines(); // Reinicia el temblor si ya estaba temblando
        StartCoroutine(ShakeCamera());
    }

    IEnumerator ShakeCamera()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Generar posición aleatoria cerca del centro
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // Mover la cámara LOCALMENTE (respetando al padre que sigue al jugador)
            cameraTransform.localPosition = originalLocalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restaurar posición exacta al terminar
        cameraTransform.localPosition = originalLocalPos;
    }
}