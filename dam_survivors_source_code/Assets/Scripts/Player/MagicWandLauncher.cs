// este script lo lleva el player
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Linq;

public class MagicWandLauncher : BaseLauncher 
{
    [Header("Magic Wand Settings")]
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private int targetsToFind = 2; 
    [SerializeField] private float searchRadius = 15f; 
    [SerializeField] private float spawnDelayBetweenMissiles = 0.2f; 

    // Sobrescribimos la función de disparo del padre
    protected override void AttemptToFire() 
    {
        // Encontrar todos los enemigos cercanos en la capa "Enemy"
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, LayerMask.GetMask("Enemy"));
        
        if (hitColliders.Length > 0)
        {
            // Encontrar los X enemigos más cercanos usando LINQ
            List<Transform> closestEnemies = hitColliders
                .Select(col => new { 
                    Trans = col.transform, 
                    Dist = Vector3.Distance(transform.position, col.transform.position) 
                })
                .OrderBy(x => x.Dist)   // Ordenar por distancia (menor a mayor)
                .Take(targetsToFind)    // Coger solo los necesarios (ej: 2)
                .Select(x => x.Trans)   // Quedarnos solo con el Transform
                .ToList();

            // Disparar la corrutina de spawn
            StartCoroutine(SpawnMissiles(closestEnemies));
        }
    }

    private IEnumerator SpawnMissiles(List<Transform> targets)
    {
        foreach (Transform targetEnemy in targets)
        {
            // Si el enemigo muere antes de disparar, pasamos al siguiente
            if (targetEnemy == null) continue;

            // Crear el misil
            GameObject missile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // Calcular dirección hacia el enemigo
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            
            // Configurar el misil
            HomingProjectile homingScript = missile.GetComponent<HomingProjectile>();
            if (homingScript != null)
            {
                // currentDamage viene heredado de BaseLauncher
                homingScript.SetTargetAndDamage(targetEnemy, currentDamage);
            }
            
            // Esperar antes del siguiente misil
            yield return new WaitForSeconds(spawnDelayBetweenMissiles);
        }
    }

    // Dibujar el rango en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}