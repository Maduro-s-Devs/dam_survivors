// Este script lo lleva el Player
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private XPBarController xpBar; // Arrastra aquí el objeto de la barra de XP
    [SerializeField] private LevelUpManager levelManager; // Arrastra aquí el GameManager

    [Header("PLayer stats")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExperience = 0f;
    [SerializeField] private float maxExperience = 100f;  

    [Header("Level up config")]
    [SerializeField] private float xpMultiplier = 1.2f;   // Cada nivel cuesta un 20% más
    
    [Header("Recolection Area")]
    public float pickupRadius = 5f; 

    // Propiedad pública por si otros scripts (como UI) necesitan leer el nivel
    public int CurrentLevel => currentLevel;

    private void Start()
    {
        // Inicializamos la UI visualmente al empezar
        UpdateUI();
        if(xpBar != null) xpBar.UpdateLevelText(currentLevel);
    }

    public void AddExperience(float amount)
    {
        currentExperience += amount;
        
        // Debug visual para la consola
        // Debug.Log($"XP: {currentExperience} / {maxExperience}");

        // Comprobar si subimos de nivel Bucle por si se gana mucha XP de golpe
        while (currentExperience >= maxExperience)
        {
            LevelUp();
        }

        // Actualizamos la barra visualmente después de los cálculos
        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= maxExperience;
        maxExperience *= xpMultiplier;

        Debug.Log($"<color=yellow>¡NIVEL UP! Ahora eres Nivel {currentLevel}</color>");

        // Actualizar visuales de la barra y el número de nivel
        if (xpBar != null)
        {
            xpBar.OnLevelUp(); // Resetea la barra para que no baje visualmente
            xpBar.UpdateLevelText(currentLevel); // Cambia el número "lvl number"
        }

        // Aquí más adelante llamaremos a la Pausa y al Menú de Mejoras
        // (IMPLEMENTADO: Llamada al gestor de cartas)
        if (levelManager != null)
        {
            levelManager.ShowLevelUpOptions();
        }
    }

    // Función auxiliar para mantener la barra sincronizada
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