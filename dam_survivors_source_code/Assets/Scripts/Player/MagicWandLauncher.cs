// este script lo lleva el player
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Linq;

public class MagicWandLauncher : BaseLauncher 
{
    [Header("Magic Wand Settings")]
    [SerializeField] private GameObject projectilePrefab; // El misil normal
    [SerializeField] private int baseTargets = 2;         // Enemigos base a nivel 1
    [SerializeField] private float searchRadius = 15f; 
    [SerializeField] private float spawnDelayBetweenMissiles = 0.2f; 

    [Header("Evolución Final (Nivel Máximo)")]
    [SerializeField] private int maxLevel = 10;           // Nivel para evolucionar
    [SerializeField] private GameObject ultimatePrefab;   // El misil evolucionado
    [SerializeField] private int ultimateTargets = 20;    // A cuantos dispara cuando evoluciona

    // Sobrescribimos la función de disparo
    protected override void AttemptToFire() 
    {
        // Cálculo de objetivos (Cuántas balas tenemos)
        int bulletsToFire = baseTargets + (level - 1);
        GameObject prefabToUse = projectilePrefab;

        // Comprobación de Evolución
        if (level >= maxLevel)
        {
            bulletsToFire = ultimateTargets; 
            if (ultimatePrefab != null) 
            {
                prefabToUse = ultimatePrefab; 
            }
        }

        // Búsqueda de enemigos
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, LayerMask.GetMask("Enemy"));
        
        if (hitColliders.Length > 0)
        {
            // Ordenamos los enemigos disponibles por distancia (pero NO los recortamos aún)
            List<Transform> availableEnemies = hitColliders
                .Select(col => new { 
                    Trans = col.transform, 
                    Dist = Vector3.Distance(transform.position, col.transform.position) 
                })
                .OrderBy(x => x.Dist)
                .Select(x => x.Trans)
                .ToList();

            // Creamos la lista final de objetivos (Round Robin)
            // Si tengo 5 balas y 2 enemigos, llenamos la lista así: [E1, E2, E1, E2, E1]
            List<Transform> finalTargets = new List<Transform>();

            for (int i = 0; i < bulletsToFire; i++)
            {
                // Usamos el operador resto (%) para volver al principio de la lista si se acaban los enemigos
                int enemyIndex = i % availableEnemies.Count;
                finalTargets.Add(availableEnemies[enemyIndex]);
            }

            // Disparar la lista completa
            StartCoroutine(SpawnMissiles(finalTargets, prefabToUse));
        }
    }

    private IEnumerator SpawnMissiles(List<Transform> targets, GameObject prefab)
    {
        foreach (Transform targetEnemy in targets)
        {
            // Seguridad: Si el enemigo muere antes de que le toque su segunda bala,
            // Unity devolverá null. Saltamos ese disparo para no dar error.
            if (targetEnemy == null) continue;

            // Instanciamos el proyectil
            GameObject missile = Instantiate(prefab, transform.position, Quaternion.identity);
            
            // Configuramos el misil 
            HomingProjectile homingScript = missile.GetComponent<HomingProjectile>();
            if (homingScript != null)
            {
                homingScript.SetTargetAndDamage(targetEnemy, currentDamage);
            }
            
            yield return new WaitForSeconds(spawnDelayBetweenMissiles);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}