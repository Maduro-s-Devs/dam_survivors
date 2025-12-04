//este script lo lleva el prefab de la vida
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float healAmount = 25f; // Cantidad de vida a curar

    private void OnTriggerEnter(Collider other)
    {
        // Solo el jugador puede recogerlo
        if (other.CompareTag("Player"))
        {
            // Buscamos el script de salud del jugador
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Si el jugador no tiene la vida al máximo, lo curamos
                if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log($"[ITEM] Recogida Poción. Salud +{healAmount}");
                    
                    // Destruimos el objeto tras usarlo
                    Destroy(gameObject);
                }
                else
                {
                    // Si ya tiene la vida a tope, no lo recogemos
                    // Si prefieres que se gaste igual, borra este 'else'.
                    Debug.Log("[ITEM] Vida llena, no se consume la poción.");
                }
            }
        }
    }
}