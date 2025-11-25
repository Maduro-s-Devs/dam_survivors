// este script, lo llevan todos los enemigos menos el Boss
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Estadísticas del Enemigo")]
    [SerializeField] private float movementSpeed = 3f; // Velocidad de movimiento (Máxima)
    [SerializeField] private float acceleration = 2f;  // Cuánto tarda en alcanzar la velocidad máxima
    [SerializeField] private float maxHealth = 100f;    // Vida Máxima
    [SerializeField] private float damage = 10f;       // Daño al tocar al jugador

    private Transform playerTransform; // Referencia para saber dónde está el jugador
    private float currentHealth;       // Vida actual
    private float currentSpeed = 0f;   // Velocidad actual (empieza en 0 y sube)

    private void Start()
    {
        // Inicializar vida
        currentHealth = maxHealth;

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

    private void Update()
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

        // Calculamos la aceleración: Vamos aumentando currentSpeed hacia movementSpeed poco a poco
        currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, acceleration * Time.deltaTime);

        // Usamos Space.World para movernos en coordenadas globales (usando la velocidad actual suavizada)
        transform.position += direction * currentSpeed * Time.deltaTime;

        // Mirar al jugador (para que el modelo rote)
        // Bloqueamos la rotación en X y Z para que no se incline
        Vector3 lookPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPos);
    }

    // Esta función será llamada por tus armas
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
        // Lógica de soltar experiencia
        // Ejemplo: Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // Lógica para dañar al jugador al chocar
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Aquí llamaríamos al script del jugador para hacerle daño
            // Ejemplo: collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            // Debug.Log("Atacando al jugador!");
        }
    }
}