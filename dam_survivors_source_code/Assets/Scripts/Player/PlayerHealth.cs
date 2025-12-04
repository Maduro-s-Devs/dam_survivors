using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private HealthBarController healthBar; // <--- ARRASTRA AQUÍ TU BARRA (El Slider)
    [SerializeField] private DamageFeedback damageFeedback;

    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    // Propiedad para que la futura UI sepa cuánta vida queda
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Sincronizamos la barra al iniciar
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        // Aplicamos el daño directamente sin preguntar
        currentHealth -= amount;
        
        // Actualizamos la UI visualmente
        UpdateHealthUI();

        Debug.Log($"[PLAYER] ¡Auch! Daño: {amount}. Vida restante: {currentHealth}");

        if (damageFeedback != null)
        {
            damageFeedback.TriggerDamageEffect();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        // Actualizamos la UI visualmente
        UpdateHealthUI();
        
        Debug.Log($"[PLAYER] Curado: +{amount}. Vida actual: {currentHealth}");
    }

    private void Die()
    {
        currentHealth = 0;
        UpdateHealthUI(); // Aseguramos que la barra baje a 0 visualmente
        
        Debug.LogError("--- GAME OVER --- Has muerto.");
        
        // Pausamos el juego
        Time.timeScale = 0f;
    }

    // Esta función la llama Unity automáticamente cuando el CharacterController toca algo
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Buscamos el ID de la capa "Water" (Suele ser la capa 4 por defecto en Unity)
        int waterLayerIndex = LayerMask.NameToLayer("Water");

        // Si el objeto que tocamos tiene esa capa...
        if (hit.gameObject.layer == waterLayerIndex)
        {
            if (currentHealth > 0)
            {
                Debug.Log("¡CAÍSTE A LA LAVA! Muerte instantánea.");
                // Nos quitamos toda la vida que nos quede de golpe
                TakeDamage(currentHealth); 
            }
        }
    }
    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }
}