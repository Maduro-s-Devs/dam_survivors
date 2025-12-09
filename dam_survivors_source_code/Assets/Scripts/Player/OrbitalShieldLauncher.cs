using UnityEngine;
using System.Collections.Generic;

public class OrbitalShieldLauncher : BaseLauncher
{
    [Header("Configuración Escudo")]
    [SerializeField] private GameObject orbPrefab;         // Prefab normal (bola azul/blanca)
    [SerializeField] private float rotationSpeed = 100f;   // Velocidad de giro
    [SerializeField] private float orbitRadius = 3f;       // Distancia al jugador
    
    // Variables para restaurar valores si reiniciamos
    private float defaultRotationSpeed = 100f;
    private float defaultOrbitRadius = 3f;

    [Header("Evolución (Nivel 10)")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateOrbPrefab; // Prefab evolucionado 
    
    [Header("Daño en Área (Solo Evolucionado)")]
    [SerializeField] private float areaDamageInterval = 0.5f; 
    [Range(0.1f, 1f)]
    [SerializeField] private float areaDamageMultiplier = 0.5f; // Hace la mitad de daño que un golpe directo

    // Estado interno
    private List<GameObject> activeOrbs = new List<GameObject>();
    private float currentRotation = 0f;
    private float areaTimer = 0f;

    protected override void Start()
    {
        base.Start(); 
        
        // Guardamos la configuración inicial
        defaultRotationSpeed = rotationSpeed;
        defaultOrbitRadius = orbitRadius;

        // Si venía desbloqueado de serie, lo activamos
        if (isUnlocked)
        {
            SpawnOrbs();
        }
    }

    // Usamos LateUpdate para que las bolas sigan al jugador SUAVEMENTE
    private void LateUpdate()
    {
        if (!isUnlocked) return;

        // Calcular giro
        currentRotation += rotationSpeed * Time.deltaTime; 

        //over bolas
        UpdateOrbPositions();

        // 3. Lógica de Evolución (Daño en Área constante)
        if (level >= maxLevel)
        {
            ApplyAreaDamage();
        }
    }

    // Anulamos el Update normal del padre porque el escudo funciona distinto (siempre está activo)
    protected override void Update() { }

    // --- MÉTODOS SOBRESCRITOS  ---

    public override void ActivateWeapon() 
    {
        // Llama a la lógica base para poner isUnlocked = true y level = 1
        base.ActivateWeapon();
        // Genera las bolas visualmente
        SpawnOrbs();
    }

    public override void CalculateStats()
    {
        base.CalculateStats(); 
        
        // Si estamos jugando, regeneramos las bolas cada vez que cambien las stats (nivel)
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

    // --- LÓGICA INTERNA ---

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

        // Fórmula: Empieza con 3, y gana 1 bola cada 2 niveles aprox.
        int orbCount = 3 + (level / 2); 
        GameObject prefabToUse = orbPrefab;
        
        // Si NO es evolución, usamos valores normales
        if (level < maxLevel) 
        {
            rotationSpeed = defaultRotationSpeed;
            orbitRadius = defaultOrbitRadius;
        }

        // SI ES EVOLUCIÓN (Nivel 10+)
        if (level >= maxLevel)
        {
            orbCount += 4; // Muchas más bolas
            if (ultimateOrbPrefab != null) prefabToUse = ultimateOrbPrefab;
            
            rotationSpeed = 250f; // Gira mucho más rápido
            orbitRadius = 5f;     // Se aleja más
        }

        if (orbCount > 0)
        {
            for (int i = 0; i < orbCount; i++)
            {
                // Instanciamos
                GameObject newOrb = Instantiate(prefabToUse, transform); 
                
                // Configuramos daño
                OrbitalProjectile script = newOrb.GetComponent<OrbitalProjectile>();
                if (script != null) script.SetDamage(currentDamage);

                activeOrbs.Add(newOrb);
            }
        }
        
        // Colocamos inmediatamente
        UpdateOrbPositions();
    }

    private void UpdateOrbPositions()
    {
        if (activeOrbs.Count == 0) return;

        float angleStep = 360f / activeOrbs.Count;

        for (int i = 0; i < activeOrbs.Count; i++)
        {
            if (activeOrbs[i] == null) continue;
            
            // Matemáticas circulares (Seno y Coseno)
            float currentOrbAngle = currentRotation + (i * angleStep);
            float angleRad = currentOrbAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleRad) * orbitRadius;
            float z = Mathf.Sin(angleRad) * orbitRadius;

            // Mover la bola. Usamos transform.position del jugador + el offset calculado
            activeOrbs[i].transform.position = transform.position + new Vector3(x, 0, z);
        }
    }

    // Daño de zona pasivo (Solo Evolución)
    private void ApplyAreaDamage()
    {
        areaTimer -= Time.deltaTime;

        if (areaTimer <= 0f)
        {
            // Detecta enemigos en todo el radio del escudo
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
        // Dibuja el radio en el editor para ver cuánto ocupa
        if (level >= maxLevel)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f); 
            Gizmos.DrawSphere(transform.position, orbitRadius);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, orbitRadius);
        }
    }
}