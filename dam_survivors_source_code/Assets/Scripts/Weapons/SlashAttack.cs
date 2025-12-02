// este script lo lleva el prefab del slash
using UnityEngine;
public class SlashAttack : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float lifetime = 0.3f; // Duración 

    private float damage;
    private float lifestealAmount = 0f; 
    private bool canLifesteal = false;

    private void Start()
    {
        // Se autodestruye rapidísimo, es un "zasca"
        Destroy(gameObject, lifetime);
    }

    // El lanzador configura esto al instanciarlo
    public void Initialize(float dmg, bool isEvolved, float healAmount)
    {
        damage = dmg;
        canLifesteal = isEvolved;
        lifestealAmount = healAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            
            if (enemy != null)
            {
                // 1. Aplicar Daño
                enemy.TakeDamage(damage);

                // 2. Lógica de Robo de Vida (Evolución)
                if (canLifesteal)
                {
                    HealPlayer();
                }
            }
        }
    }

    private void HealPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // TODO: Lógica de curación futura
            // player.GetComponent<PlayerHealth>()?.Heal(lifestealAmount);
        }
    }
}