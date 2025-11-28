// Este script lo lleva el prefab del Molotov 
using UnityEngine;
using System.Collections;

public class MolotovProjectile : MonoBehaviour
{
    [Header("Configuración de Explosión")]
    [SerializeField] private float explosionRadius = 3f; // Radio de daño
    [SerializeField] private float flightDuration = 1f;  // Tiempo de vuelo
    [SerializeField] private float delayBetweenExplosions = 0.3f; // Ritmo de las explosiones
    
    [Header("Configuración Evolución")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private float napalmDuration = 5f;      // Cuánto dura el fuego en el suelo
    [SerializeField] private float napalmRadius = 6f;        // Radio gigante
    [SerializeField] private float napalmDamageInterval = 1.0f; // Daño por tick

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject explosionVFX;    // Explosión Normal (Niveles 1-9)
    [SerializeField] private GameObject napalmStartVFX;  // Explosión Inicial (Nivel 10)
    [SerializeField] private GameObject napalmZoneVFX;   // Fuego persistente (Suelo)

    // Variables internas
    private Vector3 startPos;
    private Vector3 targetPos;
    private float arcHeight;
    private float damage;
    private int weaponLevel; // Nivel del arma

    private float timeElapsed = 0f;
    private bool hasExploded = false;

    // Inicialización (Llamada por el Launcher)
    public void Initialize(Vector3 start, Vector3 target, float height, float dmg, int level)
    {
        startPos = start;
        targetPos = target;
        arcHeight = height;
        damage = dmg;
        weaponLevel = Mathf.Max(1, level); // Nos aseguramos de que al menos sea nivel 1
    }

    private void Update()
    {
        if (hasExploded) return;

        // --- LÓGICA DE VUELO ---
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / flightDuration;

        if (t >= 1f)
        {
            ExplodeSequence();  
            return;
        }

        // Posición lineal + Curva
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.y += arcHeight * Mathf.Sin(t * Mathf.PI);

        transform.position = currentPos;
        transform.Rotate(Vector3.right * 720 * Time.deltaTime);
    }

    private void ExplodeSequence()
    {
        hasExploded = true;

        // Ocultamos la botella visualmente pero no destruimos el objeto aún
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // ¿Modo Normal o Modo Napalm?
        if (weaponLevel >= maxLevel)
        {
            StartCoroutine(NapalmMode());
        }
        else
        {
            StartCoroutine(ProcessExplosions());
        }
    }

    // --- EXPLOSIONES EN CADENA (Niveles 1-9) ---
    private IEnumerator ProcessExplosions()
    {
        // EXPLOSIÓN PRINCIPAL (Usa el VFX normal)
        CreateExplosion(transform.position, true, explosionVFX);
        yield return new WaitForSeconds(delayBetweenExplosions);

        // EXPLOSIONES EXTRA - 1 por nivel extra
        int extraExplosions = weaponLevel - 1;

        for (int i = 0; i < extraExplosions; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * explosionRadius;
            Vector3 extraPos = transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);

            // Dibujamos una línea verde en la escena para ver dónde va
            Debug.DrawLine(transform.position, extraPos, Color.green, 1f);

            CreateExplosion(extraPos, false, explosionVFX);
            
            yield return new WaitForSeconds(delayBetweenExplosions);
        }

        Destroy(gameObject);
    }

    // --- ZONA DE NAPALM (Nivel 10+) ---
    private IEnumerator NapalmMode()
    {
        Debug.Log("¡MOLOTOV EVOLUCIONADO! Zona de Napalm activada.");

        // EXPLOSIÓN INICIAL ESPECÍFICA (Usa el VFX de Napalm Start)
        // Esto hace el daño de impacto inicial y muestra la explosión única
        CreateExplosion(transform.position, true, napalmStartVFX);

        // Instanciar VFX de Fuego Persistente (Suelo)
        GameObject fireZone = null;
        if (napalmZoneVFX != null)
        {
            fireZone = Instantiate(napalmZoneVFX, transform.position, Quaternion.identity);
        }
        
        // Bucle de Daño en el tiempo (DoT)
        float timer = 0f;
        while (timer < napalmDuration)
        {
            // Aplicar daño en el radio gigante (reutilizamos la lógica de daño)
            ApplyDamageToArea(transform.position, napalmRadius, damage * 1f); // 100% daño por tick

            yield return new WaitForSeconds(napalmDamageInterval);
            timer += napalmDamageInterval;
        }

        // Limpieza
        if (fireZone != null) Destroy(fireZone);
        Destroy(gameObject);
    }

    // Función unificada para crear explosiones con VFX personalizado
    private void CreateExplosion(Vector3 center, bool isMain, GameObject vfxToUse)
    {
        // --- INSTANCIAR EXPLOSIÓN ---
        if (vfxToUse != null)
        {
            Instantiate(vfxToUse, center, Quaternion.identity);
        }
        
        // --- VISUALIZACIÓN DEBUG (Comentada) ---
        /*
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = center;
        debugSphere.transform.localScale = Vector3.one * (explosionRadius * 2); 
        Destroy(debugSphere.GetComponent<Collider>()); 
        Destroy(debugSphere, 0.5f); 
        */

        // DAÑO EN ÁREA
        ApplyDamageToArea(center, explosionRadius, damage);
        
        if(weaponLevel < maxLevel)
            Debug.Log($"¡BOOM! Explosión (Nivel Arma: {weaponLevel})");
    }

    // Función auxiliar para aplicar daño (para no repetir código)
    private void ApplyDamageToArea(Vector3 center, float radius, float dmgAmount)
    {
        Collider[] enemiesHit = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider hit in enemiesHit)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(dmgAmount);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        
        // Dibujamos también el radio de Napalm en Naranja para verlo en el editor
        Gizmos.color = new Color(1, 0.5f, 0, 1);
        Gizmos.DrawWireSphere(transform.position, napalmRadius);
    }
}