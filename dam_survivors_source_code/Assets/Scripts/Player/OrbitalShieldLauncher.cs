// Este script lo utiliza el player
using UnityEngine;
using System.Collections.Generic;

public class OrbitalShieldLauncher : BaseLauncher
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject orbPrefab;         // Prefab normal
    [SerializeField] private float rotationSpeed = 100f;   // Velocidad de giro
    [SerializeField] private float orbitRadius = 3f;       // Distancia al jugador
    private float  defaultRotationSpeed = 100f;
    private float  defaultOrbitRadius= 3f;

    [Header("Evolution")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateOrbPrefab; // Prefab evolucionado
    
    [Header("Ajustes de Zona (Evolución)")]
    [SerializeField] private float areaDamageInterval = 0.7f; // Cada cuánto hace daño el área (ticks)
    [Range(0.1f, 1f)]
    [SerializeField] private float areaDamageMultiplier = 0.5f; // % del daño base que aplica la zona (0.5 = 50%)

    // Lista para guardar las bolas que tenemos activas
    private List<GameObject> activeOrbs = new List<GameObject>();
    
    // Ángulo actual del "contenedor"
    private float currentRotation = 0f;

    // Temporizador para el daño de área (Evolución)
    private float areaTimer = 0f;

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

        // LÓGICA DE EVOLUCIÓN: DAÑO EN ÁREA (CIRCUNFERENCIA COMPLETA)
        if (level >= maxLevel)
        {
            ApplyAreaDamage();
        }
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
        int orbCount = 3 + (level / 2);

        // EVOLUCIÓN: Si es nivel máximo, ponemos MUCHOS más
        GameObject prefabToUse = orbPrefab;
        
        // Reseteamos valores base antes de aplicar evolución si bajamos de nivel
       if (level < maxLevel) 
        {
             // Restauramos los valores a su estado original (por si bajamos de nivel con el Debug)
             rotationSpeed = defaultRotationSpeed;
             orbitRadius = defaultOrbitRadius;
        }

        if (level >= maxLevel)
        {
            orbCount += 5; // Bonus de evolución
            if (ultimateOrbPrefab != null) prefabToUse = ultimateOrbPrefab;
            
            rotationSpeed = 200f; // Actualizamos la variable global para que el Update gire rápido
            orbitRadius = 6f;     // Actualizamos la variable global para que el área sea grande
        }

        // Spawneamos los orbes
        if (orbCount > 0)
        {
            float angleStep = 360f / orbCount;
            for (int i = 0; i < orbCount; i++)
            {
                GameObject newOrb = Instantiate(prefabToUse, transform.position, Quaternion.identity);
                
                // Configuramos daño de la bola (Este sigue siendo el 100% del daño)
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

    // Nueva función para gestionar el daño de zona al evolucionar
    private void ApplyAreaDamage()
    {
        areaTimer -= Time.deltaTime;

        if (areaTimer <= 0f)
        {
            // Detectamos todo lo que esté dentro del radio del escudo
            Collider[] enemiesInside = Physics.OverlapSphere(transform.position, orbitRadius, LayerMask.GetMask("Enemy"));

            foreach (var hit in enemiesInside)
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // Aplicamos solo un porcentaje del daño total indicado en areaDamageMultiplier, así el área debilita, pero las bolas siguen siendo importantes para rematar.
                    enemy.TakeDamage(currentDamage * areaDamageMultiplier);
                }
            }

            // Reiniciamos el timer
            areaTimer = areaDamageInterval;
        }
    }

    // Dibujamos el área de efecto en el editor para verla
    private void OnDrawGizmosSelected()
    {
        if (level >= maxLevel)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f); // Rojo transparente suave
            Gizmos.DrawSphere(transform.position, orbitRadius);
        }
    }
}