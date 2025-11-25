// Este script lo utiliza el player
using UnityEngine;
using System.Collections.Generic;

public class OrbitalShieldLauncher : BaseLauncher
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject orbPrefab;         // Prefab normal
    [SerializeField] private float rotationSpeed = 100f;   // Velocidad de giro
    [SerializeField] private float orbitRadius = 3f;       // Distancia al jugador

    [Header("Evolution")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateOrbPrefab; // Prefab evolucionado

    // Lista para guardar las bolas que tenemos activas
    private List<GameObject> activeOrbs = new List<GameObject>();
    
    // Ángulo actual del "contenedor"
    private float currentRotation = 0f;

    protected override void Start()
    {
        base.Start(); // Llama al Start de BaseLauncher
        if (isUnlocked)
        {
            SpawnOrbs();
        }
    }

    protected override void Update()
    {
        // Si no está desbloqueada, no hacemos nada
        if (!isUnlocked) return;

        // Movimiento Circular
        currentRotation += rotationSpeed * Time.deltaTime; 

        // Giramos el transform del lanzador
        // Calculamos la posición de cada bola manualmente.
        UpdateOrbPositions();
    }

    // Esta función se llama cuando desbloqueamos el arma o subimos de nivel externamente
    public new void ActivateWeapon() 
    {
        base.ActivateWeapon();
        SpawnOrbs();
    }
    
    // Para ver los cambios en tiempo real en el Editor al cambiar 'Level'
    private void OnValidate()
    {
        // Llamamos a CalculateStats para actualizar daño y orbes
        CalculateStats();
    }

    // Sobrescribimos CalculateStats para añadir la lógica de "Más orbes por nivel"
    public new void CalculateStats()
    {
        base.CalculateStats(); // Calcula el daño base
        // Añadimos el chequeo de Application.isPlaying para no spawnear cosas en modo edición y ensuciar la escena
        if (Application.isPlaying && isUnlocked)
        {
            SpawnOrbs();
        }
    }

    private void SpawnOrbs()
    {
        // Limpiar orbes antiguos
        foreach (var orb in activeOrbs)
        {
            if (orb != null) Destroy(orb);
        }
        activeOrbs.Clear();

        // Calcular cuántos orbes tocan
        // 3 Base + (Nivel / 2)
        // Nivel 1 = 3 + 0 = 3
        // Nivel 2 = 3 + 1 = 4
        // Nivel 10 = 3 + 5 = 8
        int orbCount = 3 + (level / 2);

        // EVOLUCIÓN: Si es nivel máximo, ponemos MUCHOS más
        GameObject prefabToUse = orbPrefab;
        float currentSpeed = rotationSpeed; // Variable local para velocidad

        if (level >= maxLevel)
        {
            orbCount += 10; // Bonus de evolución
            if (ultimateOrbPrefab != null) prefabToUse = ultimateOrbPrefab;
            currentSpeed = 400f; // Gira muchísimo más rápido al evolucionar
            orbitRadius = 6f;  
        }

        // Spawneamos los orbes
        if (orbCount > 0)
        {
            float angleStep = 360f / orbCount;
            for (int i = 0; i < orbCount; i++)
            {
                GameObject newOrb = Instantiate(prefabToUse, transform.position, Quaternion.identity);
                
                // Configuramos daño
                OrbitalProjectile script = newOrb.GetComponent<OrbitalProjectile>();
                if (script != null) script.SetDamage(currentDamage);

                activeOrbs.Add(newOrb);
            }
        }
        
        // Colocamos la primera vez
        UpdateOrbPositions();
    }

    private void UpdateOrbPositions()
    {
        if (activeOrbs.Count == 0) return;

        float angleStep = 360f / activeOrbs.Count;

        for (int i = 0; i < activeOrbs.Count; i++)
        {
            if (activeOrbs[i] == null) continue;

            // Ángulo de esta bola específica = Rotación Global + (Su índice * Separación)
            float currentOrbAngle = currentRotation + (i * angleStep);
            
            // Convertir a Radianes
            float angleRad = currentOrbAngle * Mathf.Deg2Rad;

            // Posición X y Z (Offset desde el jugador)
            float x = Mathf.Cos(angleRad) * orbitRadius;
            float z = Mathf.Sin(angleRad) * orbitRadius;

            // Centro del Jugador + Offset
            activeOrbs[i].transform.position = transform.position + new Vector3(x, 0, z);
        }
    }
}