// Este script lo lleva el Player
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
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
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= maxExperience;
        maxExperience *= xpMultiplier;

        Debug.Log($"<color=yellow>¡NIVEL UP! Ahora eres Nivel {currentLevel}</color>");

        // Aquí más adelante llamaremos a la Pausa y al Menú de Mejoras
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}