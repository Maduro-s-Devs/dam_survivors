using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private XPBarController xpBar; 

    [Header("Player stats")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExperience = 0f;
    [SerializeField] private float maxExperience = 100f;  

    [Header("Level up config")]
    [SerializeField] private float xpMultiplier = 1.2f;   
    
    [Header("Recolection Area")]
    public float pickupRadius = 5f; 

    public int CurrentLevel => currentLevel;

    void Start()
    {
        UpdateUI();
        
        // <--- NUEVO: Al empezar, nos aseguramos de que ponga "1" (o el nivel guardado)
        if(xpBar != null) xpBar.UpdateLevelText(currentLevel);
    }

    public void AddExperience(float amount)
    {
        currentExperience += amount;
        
        while (currentExperience >= maxExperience)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= maxExperience;
        maxExperience *= xpMultiplier;

        Debug.Log($"<color=yellow>¡NIVEL UP! Ahora eres Nivel {currentLevel}</color>");
        
        // <--- NUEVO: Avisamos a la barra y ACTUALIZAMOS EL TEXTO
        if(xpBar != null)
        {
            xpBar.OnLevelUp();
            xpBar.UpdateLevelText(currentLevel); // Actualiza el número en pantalla
        }
    }

    private void UpdateUI()
    {
        if (xpBar != null)
        {
            xpBar.UpdateXP(currentExperience, maxExperience);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}