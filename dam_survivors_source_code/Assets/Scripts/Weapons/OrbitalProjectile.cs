// Este script lo utiliza el orbe de orbital shield
using UnityEngine;

public class OrbitalProjectile : MonoBehaviour
{
    private float damage; // El daño se lo pasa el lanzador

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Buscamos el script del enemigo y le hacemos daño
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}