using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; 
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("ANIMACIÓN HOVER (Ratón Encima)")]
    public float scaleAmount = 1.1f;
    public float rotateAmount = -3f;
    public float animationSpeed = 15f;

    [Header("ANIMACIÓN CLICK (Brillo)")]
    public int flashCount = 3; 
    public float flashDuration = 0.08f; // Rápido para que parezca luz

    [Header("Configuración de Colores")]
    public Color normalTextColor = Color.white;
    public Color flashTextColor = Color.yellow; // El texto brillará en este color

    [Range(0f, 1f)]
    public float flashAlpha = 0.5f; // 0.5 = El botón se vuelve 50% transparente (Grisáceo)

    [Header("Sonidos")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Variables internas
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 targetScale;
    private Quaternion targetRotation;
    private AudioSource audioSource;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private Button btnComp;

    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
        targetScale = originalScale;
        targetRotation = originalRotation;

        audioSource = GetComponent<AudioSource>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        btnComp = GetComponent<Button>();

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Animación suave continua
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btnComp != null && !btnComp.interactable) return;

        targetScale = originalScale * scaleAmount;
        targetRotation = Quaternion.Euler(0, 0, rotateAmount);
        
        if (hoverSound != null) audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetRotation = originalRotation;
        
        // Al salir, nos aseguramos de que el color vuelva a estar bien por si acaso
        if(buttonImage != null) 
        {
            Color c = buttonImage.color;
            c.a = 1f; // Opacidad total
            buttonImage.color = c;
        }
        if(buttonText != null) buttonText.color = normalTextColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btnComp != null && !btnComp.interactable) return;

        if (clickSound != null) audioSource.PlayOneShot(clickSound);
        
        StartCoroutine(FlashBrightnessRoutine());
    }

    IEnumerator FlashBrightnessRoutine()
    {
        // Bucle de parpadeo
        for (int i = 0; i < flashCount; i++)
        {
            // --- ESTADO BRILLANTE (ON) ---
            // 1. Texto a color brillante
            if(buttonText != null) buttonText.color = flashTextColor;

            // 2. Imagen semitransparente (para que parezca gris claro/iluminada)
            if(buttonImage != null)
            {
                Color c = buttonImage.color;
                c.a = flashAlpha; // Bajamos alfa
                buttonImage.color = c;
            }
            
            yield return new WaitForSecondsRealtime(flashDuration);

            // --- ESTADO NORMAL (OFF) ---
            // 1. Texto normal
            if(buttonText != null) buttonText.color = normalTextColor;

            // 2. Imagen opaca (Negro sólido)
            if(buttonImage != null)
            {
                Color c = buttonImage.color;
                c.a = 1f; // Restauramos alfa
                buttonImage.color = c;
            }

            yield return new WaitForSecondsRealtime(flashDuration);
        }
    }
}