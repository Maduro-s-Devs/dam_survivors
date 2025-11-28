// Este script lo lleva el player
using UnityEngine;

public class MolotovLauncher : BaseLauncher
{
    [Header("Molotov Settings")]
    [SerializeField] private GameObject molotovPrefab; // Botella Normal (Nivel 1-9)
    [SerializeField] private float throwRange = 8f;   
    [SerializeField] private float arcHeight = 5f;    

    [Header("Evolution")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateMolotovPrefab; // Botella Evolucionada (Nivel 10)

    protected override void AttemptToFire()
    {
        // Calcular punto de caída
        Vector2 randomPoint = Random.insideUnitCircle * throwRange;
        Vector3 targetPos = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

        // ¿Qué botella lanzamos?
        GameObject prefabToUse = molotovPrefab; // Por defecto la normal
        
        // Si somos nivel máximo y tenemos asignado el prefab especial...
        if (level >= maxLevel && ultimateMolotovPrefab != null)
        {
            prefabToUse = ultimateMolotovPrefab; // Cambiamos al prefab evolucionado
        }

        // Instanciamos la botella elegida
        GameObject molotov = Instantiate(prefabToUse, transform.position, Quaternion.identity);

        // Inicializamos sus datos (Daño, Nivel, etc.)
        MolotovProjectile script = molotov.GetComponent<MolotovProjectile>();
        if (script != null)
        {
            // Le pasamos el 'level' para que el proyectil sepa si tiene que hacer Napalm
            script.Initialize(transform.position, targetPos, arcHeight, currentDamage, level);
        }
    }
}