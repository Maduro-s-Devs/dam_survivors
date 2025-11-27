// Este script lo lleva el prefab del Molotov 
using UnityEngine;
using System.Collections;

public class MolotovProjectile : MonoBehaviour
{
    [Header("Configuración de Explosión")]
    [SerializeField] private float explosionRadius = 3f; // Radio de daño
    [SerializeField] private float flightDuration = 1f;  // Tiempo de vuelo
    [SerializeField] private float delayBetweenExplosions = 0.3f; // Ritmo de las explosiones
    
    [Header("Efectos Visuales")]
    [SerializeField] private GameObject explosionVFX;    // Prefab del efecto de explosión (Futuro VFX)

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

        // Iniciamos la secuencia
        StartCoroutine(ProcessExplosions());
    }

    private IEnumerator ProcessExplosions()
    {
        // 1. EXPLOSIÓN PRINCIPAL (Roja)
        CreateExplosion(transform.position, true);
        yield return new WaitForSeconds(delayBetweenExplosions);

        // 2. EXPLOSIONES EXTRA (Naranjas) - 1 por nivel extra
        int extraExplosions = weaponLevel - 1;

        for (int i = 0; i < extraExplosions; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * explosionRadius;
            Vector3 extraPos = transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);

            // Dibujamos una línea verde en la escena para ver dónde va
            Debug.DrawLine(transform.position, extraPos, Color.green, 1f);

            CreateExplosion(extraPos, false);
            
            yield return new WaitForSeconds(delayBetweenExplosions);
        }

        Destroy(gameObject);
    }

    private void CreateExplosion(Vector3 center, bool isMain)
    {
        // --- FUTURO VFX: INSTANCIAR EXPLOSIÓN ---
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, center, Quaternion.identity);
        }
        
        // --- VISUALIZACIÓN DEBUG ---
        // (Esto sustituye al VFX mientras no lo tenemos)
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = center;
        debugSphere.transform.localScale = Vector3.one * (explosionRadius * 2); 
        
        Destroy(debugSphere.GetComponent<Collider>()); // Sin físicas
        
        Renderer rend = debugSphere.GetComponent<Renderer>();
        if(rend != null) 
        {
            // Rojo para la principal, Naranja para las secundarias
            rend.material.color = isMain ? new Color(1, 0, 0, 0.5f) : new Color(1, 0.5f, 0, 0.5f);
        }
        
        Destroy(debugSphere, 0.5f); // Se borra en 0.5s

        // DAÑO EN ÁREA
        Collider[] enemiesHit = Physics.OverlapSphere(center, explosionRadius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider hit in enemiesHit)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        Debug.Log($"¡BOOM! Explosión (Nivel Arma: {weaponLevel})");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}