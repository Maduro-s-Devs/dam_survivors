// Este script lo lleva el player
using UnityEngine;

public class MolotovLauncher : BaseLauncher
{
    [Header("Molotov Settings")]
    [SerializeField] private GameObject molotovPrefab; 
    [SerializeField] private float throwRange = 5f;   // Distancia máxima de lanzamiento
    [SerializeField] private float arcHeight = 3f;    // Altura de la curva

    // Sobrescribimos el disparo base
    protected override void AttemptToFire()
    {
        // Calcular un punto de caída aleatorio
        Vector2 randomPoint = Random.insideUnitCircle * throwRange;
        Vector3 targetPos = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

        // Crear la botella
        GameObject molotov = Instantiate(molotovPrefab, transform.position, Quaternion.identity);

        // Configurar la botella con los datos
        MolotovProjectile script = molotov.GetComponent<MolotovProjectile>();
        if (script != null)
        {
            // Es el nivel DE ESTA ARMA (heredado de BaseLauncher)
            script.Initialize(transform.position, targetPos, arcHeight, currentDamage, level);
        }
    }
}