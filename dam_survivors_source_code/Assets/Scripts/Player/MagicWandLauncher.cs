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
    [SerializeField] private int maxLevel = 10;           // Niveles para evolucionar
    [SerializeField] private GameObject ultimatePrefab;   // El misil evolucionado 
    [SerializeField] private int ultimateTargets = 20;    // A cuantos dispara cuando evoluciona

    protected override void AttemptToFire() 
    {
        // Cálculo de objetivos según nivel (Base + Nivel)
        int currentTargetsToFind = baseTargets + (level - 1);
        GameObject prefabToUse = projectilePrefab;

        // Comprobación de Evolución
        if (level >= maxLevel)
        {
            currentTargetsToFind = ultimateTargets; // Dispara a todo el mundo
            if (ultimatePrefab != null) 
            {
                prefabToUse = ultimatePrefab; // Cambia la apariencia
            }
        }

        // Búsqueda de enemigos
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, LayerMask.GetMask("Enemy"));
        
        if (hitColliders.Length > 0)
        {
            // Ordenar por cercanía y coger solo los necesarios
            List<Transform> closestEnemies = hitColliders
                .Select(col => new { 
                    Trans = col.transform, 
                    Dist = Vector3.Distance(transform.position, col.transform.position) 
                })
                .OrderBy(x => x.Dist)
                .Take(currentTargetsToFind)
                .Select(x => x.Trans)
                .ToList();

            // Disparar
            StartCoroutine(SpawnMissiles(closestEnemies, prefabToUse));
        }
    }

    private IEnumerator SpawnMissiles(List<Transform> targets, GameObject prefab)
    {
        foreach (Transform targetEnemy in targets)
        {
            if (targetEnemy == null) continue;

            // Instanciamos el proyectil elegido
            GameObject missile = Instantiate(prefab, transform.position, Quaternion.identity);
            
            // Configuramos el script del misil
            HomingProjectile homingScript = missile.GetComponent<HomingProjectile>();
            if (homingScript != null)
            {
                // currentDamage viene multiplicado por nivel gracias a BaseLauncher
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