// este script, lo llevan todos los enemigos menos el Boss
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")] // Corregido typo "Enemie" -> "Enemy"
    [SerializeField] private float movementSpeed = 3f; 
    [SerializeField] private float acceleration = 2f;  
    [SerializeField] private float maxHealth = 100f;    
    [SerializeField] private float damage = 10f;       

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.0f; // Tiempo de espera entre golpes (1s)

    private Transform playerTransform; 
    private float currentHealth;       
    private float currentSpeed = 0f;   
    private Rigidbody rb;              

    // Variable interna para controlar el tiempo
    private float damageTimer = 0f; 

    private void Start()
    {
        // Si la vida no ha sido modificada por dificultad, usamos la base
        if (currentHealth == 0) currentHealth = maxHealth;
        
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("¡No se encuentra al Player! Revisa el Tag 'Player'.");
        }
    }

    private void Update()
    {
        // El temporizador debe contar siempre
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, acceleration * Time.fixedDeltaTime);
        Vector3 targetVelocity = direction * currentSpeed;

        // Mantenemos linearVelocity para Unity 6
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        Vector3 lookPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPos);
    }

    // --- FUNCIÓN RECUPERADA (Necesaria para el WaveDirector) ---
    public void ApplyDifficultyScaling(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth; // Rellenamos la vida
        damage *= multiplier;

        // Hacemos que crezcan un poco para que den más miedo
        float scaleMod = 1f + ((multiplier - 1f) * 0.2f); 
        transform.localScale *= scaleMod;
    }
    // -----------------------------------------------------------

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // INTEGRACIÓN CON VIC942TOR (Números Flotantes)
        // Usamos ?. por si acaso el Manager no está en la escena aún
        DamageNumberManager.Instance?.ShowDamage(amount, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        LootDropper looter = GetComponent<LootDropper>();
        if (looter != null)
        {
            looter.DropLoot();
        }
        Destroy(gameObject);
    }

    // Usamos OnCollisionStay para detectar contacto continuo
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Solo atacamos si el temporizador ha llegado a 0
            if (damageTimer <= 0)
            {
                PlayerHealth playerHP = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (playerHP != null)
                {
                    playerHP.TakeDamage(damage);
                    
                    // Reiniciamos el temporizador al valor del cooldown (1s)
                    damageTimer = attackCooldown;
                }
            }
        }  
    }
}