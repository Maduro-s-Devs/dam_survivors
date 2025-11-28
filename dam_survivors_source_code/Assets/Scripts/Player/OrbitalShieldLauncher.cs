// Este script lo utiliza el player
using UnityEngine;
using System.Collections.Generic;

public class OrbitalShieldLauncher : BaseLauncher
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject orbPrefab;         // Prefab normal
    [SerializeField] private float rotationSpeed = 100f;   // Velocidad de giro
    [SerializeField] private float orbitRadius = 3f;       // Distancia al jugador
    
    // Variables para restaurar
    private float defaultRotationSpeed = 100f;
    private float defaultOrbitRadius = 3f;

    [Header("Evolution")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateOrbPrefab; // Prefab evolucionado
    
    [Header("Zone settings (Evolution)")]
    [SerializeField] private float areaDamageInterval = 0.7f; 
    [Range(0.1f, 1f)]
    [SerializeField] private float areaDamageMultiplier = 0.5f; 

    private List<GameObject> activeOrbs = new List<GameObject>();
    private float currentRotation = 0f;
    private float areaTimer = 0f;

    protected override void Start()
    {
        base.Start(); 
        
        defaultRotationSpeed = rotationSpeed;
        defaultOrbitRadius = orbitRadius;

        if (isUnlocked)
        {
            SpawnOrbs();
        }
    }

    // Usamos LateUpdate para que las bolas sigan al jugador SUAVEMENTE
    // sin importar lo rápido que te muevas.
    private void LateUpdate()
    {
        if (!isUnlocked) return;

        // Movimiento Circular
        currentRotation += rotationSpeed * Time.deltaTime; 

        // Actualizamos posiciones
        UpdateOrbPositions();

        // Lógica de Evolución
        if (level >= maxLevel)
        {
            ApplyAreaDamage();
        }
    }

    // Anulamos el Update normal para que no haga nada (usamos LateUpdate)
    protected override void Update() { }

    public new void ActivateWeapon() 
    {
        base.ActivateWeapon();
        SpawnOrbs();
    }
    
    private void OnValidate()
    {
        CalculateStats();
    }

    public new void CalculateStats()
    {
        base.CalculateStats(); 
        
        if (Application.isPlaying)
        {
            if (isUnlocked)
            {
                SpawnOrbs();
            }
            else
            {
                ClearActiveOrbs();
            }
        }
    }

    private void ClearActiveOrbs()
    {
        foreach (var orb in activeOrbs)
        {
            if (orb != null) Destroy(orb);
        }
        activeOrbs.Clear();
    }

    private void SpawnOrbs()
    {
        ClearActiveOrbs();

        int orbCount = 3 + (level / 2);
        GameObject prefabToUse = orbPrefab;
        
        if (level < maxLevel) 
        {
             rotationSpeed = defaultRotationSpeed;
             orbitRadius = defaultOrbitRadius;
        }

        if (level >= maxLevel)
        {
            orbCount += 5; 
            if (ultimateOrbPrefab != null) prefabToUse = ultimateOrbPrefab;
            
            rotationSpeed = 200f; 
            orbitRadius = 6f;     
        }

        if (orbCount > 0)
        {
            for (int i = 0; i < orbCount; i++)
            {
                // Instanciamos como HIJO (para mantener orden) pero moveremos en global
                GameObject newOrb = Instantiate(prefabToUse, transform); 
                
                OrbitalProjectile script = newOrb.GetComponent<OrbitalProjectile>();
                if (script != null) script.SetDamage(currentDamage);

                activeOrbs.Add(newOrb);
            }
        }
        
        // Posicionamos inmediatamente para que no haya un frame "raro"
        UpdateOrbPositions();
    }

    private void UpdateOrbPositions()
    {
        if (activeOrbs.Count == 0) return;

        float angleStep = 360f / activeOrbs.Count;

        for (int i = 0; i < activeOrbs.Count; i++)
        {
            if (activeOrbs[i] == null) continue;

            float currentOrbAngle = currentRotation + (i * angleStep);
            float angleRad = currentOrbAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleRad) * orbitRadius;
            float z = Mathf.Sin(angleRad) * orbitRadius;

            // Usamos transform.position (GLOBAL)
            // Dónde está el jugador en el mundo + el offset del círculo.
            // Al sobrescribir la .position cada frame, ignoramos la rotación del padre.
            activeOrbs[i].transform.position = transform.position + new Vector3(x, 0, z);
        }
    }

    private void ApplyAreaDamage()
    {
        areaTimer -= Time.deltaTime;

        if (areaTimer <= 0f)
        {
            Collider[] enemiesInside = Physics.OverlapSphere(transform.position, orbitRadius, LayerMask.GetMask("Enemy"));

            foreach (var hit in enemiesInside)
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(currentDamage * areaDamageMultiplier);
                }
            }
            areaTimer = areaDamageInterval;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (level >= maxLevel)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f); 
            Gizmos.DrawSphere(transform.position, orbitRadius);
        }
    }
}