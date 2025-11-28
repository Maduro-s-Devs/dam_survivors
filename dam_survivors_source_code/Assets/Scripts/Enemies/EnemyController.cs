// este script, lo llevan todos los enemigos menos el Boss
using UnityEngine;

// Añadimos esto para asegurarnos de que el enemigo tenga físicas
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemie Stats")]
    [SerializeField] private float movementSpeed = 3f; // Velocidad de movimiento (Máxima)
    [SerializeField] private float acceleration = 2f;  // Cuánto tarda en alcanzar la velocidad máxima
    [SerializeField] private float maxHealth = 100f;    // Vida Máxima
    [SerializeField] private float damage = 10f;       // Daño al tocar al jugador

    private Transform playerTransform; // Referencia para saber dónde está el jugador
    private float currentHealth;       // Vida actual
    private float currentSpeed = 0f;   // Velocidad actual (empieza en 0 y sube)
    
    private Rigidbody rb;              // Referencia al componente físico

    private void Start()
    {
        // Inicializar vida
        currentHealth = maxHealth;

        // Obtener el Rigidbody para las físicas
        rb = GetComponent<Rigidbody>();

        // Buscar al jugador automáticamente por su Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("¡No se encuentra al Player! el Tag 'Player'.");
        }
    }

    // Usamos FixedUpdate en lugar de Update para cálculos de físicas constantes
    private void FixedUpdate()
    {
        // Si el jugador existe, moverse hacia él
        if (playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        //  Calcular dirección (Destino - Origen)
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // Calculamos la aceleración
        currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, acceleration * Time.fixedDeltaTime);
        Vector3 targetVelocity = direction * currentSpeed;

        // Mantenemos la Y original (rb.linearVelocity.y) para respetar la gravedad
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Mirar al jugador (para que el modelo rote)
        Vector3 lookPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPos);
    }

    // Esta función será llamada por las armas
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Soltar experiencia
        Destroy(gameObject);
    }

    // Dañar al jugador al chocar
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica de daño al jugador
        }
    }
}