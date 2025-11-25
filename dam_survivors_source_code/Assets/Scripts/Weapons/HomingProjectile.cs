// Script lo porta el prefab del misil de la magicwand
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 10f;
    
    private Transform targetEnemy; // Guardamos "quién" es el enemigo
    private float damage;

    private void Start()
    {
        // Ordena a Unity que destruya este objeto pasados 10 segundos por si no le da a ningñun enemigo o no se destruye cuando debe
        Destroy(gameObject, 10f);
    }

    // AHORA recibimos el Transform del enemigo, no un Vector3
    public void SetTargetAndDamage(Transform target, float dmg)
    {
        targetEnemy = target;
        damage = dmg;
    }

    private void Update()
    {
        // Verificamos si el enemigo sigue vivo
        if (targetEnemy != null)
        {
            // Calculamos la dirección nueva en CADA frame
            Vector3 direction = (targetEnemy.position - transform.position).normalized;

            // Mueve el proyectil hacia la posición ACTUAL del enemigo
            transform.position += direction * projectileSpeed * Time.deltaTime;

            // Rota el proyectil para que mire al enemigo
            transform.LookAt(targetEnemy);
        }
        else
        {
            // Si el enemigo muere mientras la bala vuela, destruimos la bala
            Destroy(gameObject);
        }
    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Buscamos el componente EnemyController
            EnemyController enemy = other.GetComponent<EnemyController>();
            
            if (enemy != null)
            {
                // Aplicamos daño
                enemy.TakeDamage(damage);
            }

            Debug.Log("Impacto Teledirigido con: " + other.name);
            Destroy(gameObject);
        }
    }
}