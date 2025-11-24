// Script lo porta el player
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MagicWandLauncher : LanzadorBase 
{
    [Header("Ajustes de Varita")]
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private int targetsToFind = 2; 
    [SerializeField] private float searchRadius = 15f; 
    [SerializeField] private float spawnDelayBetweenMissiles = 0.2f; 

    protected override void AttemptToFire() 
    {
        [cite_start]// Encontrar todos los enemigos cercanos 
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, LayerMask.GetMask("Enemy"));
        
        if (hitColliders.Length > 0)
        {
            [cite_start]// Encontrar los X enemigos más cercanos
            // Seleccionamos directamente el 'Transform'
            List<Transform> closestEnemies = hitColliders
                .Select(col => new { 
                    Trans = col.transform, 
                    Dist = Vector3.Distance(transform.position, col.transform.position) 
                })
                .OrderBy(x => x.Dist)
                .Take(targetsToFind)
                .Select(x => x.Trans) // Nos quedamos solo con el Transform final
                .ToList();

            [cite_start]// Spawn con retardo
            StartCoroutine(SpawnMissiles(closestEnemies));
        }
    }

    private IEnumerator SpawnMissiles(List<Transform> targets)
    {
        foreach (Transform targetEnemy in targets)
        {
            // Si el enemigo murió durante la espera anterior, saltamos al siguiente
            if (targetEnemy == null) continue;

            // Crear el proyectil
            GameObject missile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            [cite_start]// Dirección = Calcular vector (destino - origen)
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            
            HomingProjectile homingScript = missile.GetComponent<HomingProjectile>();
            if (homingScript != null)
            {
                // currentDamage heredado de LanzadorBase
                homingScript.SetDirectionAndDamage(direction, currentDamage); 
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