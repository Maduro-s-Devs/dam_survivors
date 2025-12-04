using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con Sliders

public class HealthBarController : MonoBehaviour
{
    [Header("Referencias")]
    public Slider healthSlider; // Arrastra el componente Slider aquí
    public Image fillImage;     // Arrastra la imagen "Fill" (la que tiene color)

    [Header("Configuración Visual")]
    public float smoothSpeed = 5f; // Velocidad de la animación
    public Gradient colorGradient; // Configura: Izq=Rojo, Der=Verde

    // Variable interna para la animación
    private float targetValue = 1f; 

    void Start()
    {
        // Al empezar, aseguramos que la barra esté llena y verde
        targetValue = 1f;
        healthSlider.value = 1f;
        
        if(fillImage != null)
            fillImage.color = colorGradient.Evaluate(1f);
    }

    void Update()
    {
        // Animación suave (Lerp) para que la barra no baje a saltos
        if (healthSlider.value != targetValue)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, Time.deltaTime * smoothSpeed);

            // Cambiar color según la vida restante
            if(fillImage != null)
                fillImage.color = colorGradient.Evaluate(healthSlider.value);
        }
    }

    // Esta es la función que llama tu PlayerHealth
    public void SetHealth(float currentHealth, float maxHealth)
    {
        // Convertimos la vida (ej: 80/100) a porcentaje (0.8)
        targetValue = Mathf.Clamp01(currentHealth / maxHealth);
    }
}