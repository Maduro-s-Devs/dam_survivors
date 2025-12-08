/// este script lo lleva el boss
using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Estadísticas del Jefe")]
    [SerializeField] private float maxHealth = 5000f; // Vida 
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject bossBodyVisual; // Para parpadear al recibir daño

    [Header("Zona de Detección")]
    [SerializeField] private float detectionRange = 20f; // Zona invisible para detectar al player
    [SerializeField] private Transform bossRotationPart; // La parte que mira al player 

    [Header("Brazo 1: Bolas de Fuego")]
    [SerializeField] private Transform firePointArm;     // El brazo que dispara
    [SerializeField] private GameObject fireballPrefab; 
    [SerializeField] private float attackCycleCooldown = 4f; // Tiempo entre ciclos completos

    [Header("Brazo 2: Invocador")]
    [SerializeField] private SwarmSpawner summonerArm;   // Referencia al script SwarmSpawner del otro brazo
    [SerializeField] private float summonInterval = 15f; // Cada cuánto invoca esbirros

    private Transform playerTransform;
    private bool isAttacking = false;
    private float summonTimer;

    private void Start()
    {
        currentHealth = maxHealth;
        summonTimer = summonInterval;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // INVOCACIÓN (Independiente del ataque)
        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0f)
        {
            if (summonerArm != null) summonerArm.SpawnGroup();
            summonTimer = summonInterval;
        }

        // DETECCIÓN Y ATAQUE
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Si el jugador entra en la zona
        if (distanceToPlayer <= detectionRange)
        {
            // El boss mira al jugador (cabe resaltar que solo la visual, la fisica se quueda igual)
            if (bossRotationPart != null)
            {
                Vector3 lookPos = playerTransform.position;
                lookPos.y = bossRotationPart.position.y; // Mantener altura
                bossRotationPart.LookAt(lookPos);
            }

            // Inicia el ciclo de ataque si es que no lo está haciendo 
            if (!isAttacking)
            {
                StartCoroutine(AttackPatternRoutine());
            }
        }
    }

    // 1 - 2 - 3 prefab esfera con dispersiçon cónica
    private IEnumerator AttackPatternRoutine()
    {
        isAttacking = true;

        // UN PROYECTIL (Calculamos dirección inicial)
        Vector3 directionToPlayer = GetAimDirection();
        SpawnProjectile(directionToPlayer);
        
        yield return new WaitForSeconds(1.5f); // "Al segundo y medio"

        // DOS PROYECTILES Recalculamos target
        directionToPlayer = GetAimDirection(); 
        SpawnProjectile(Quaternion.Euler(0, -5, 0) * directionToPlayer); // Un poco a la izq
        SpawnProjectile(Quaternion.Euler(0, 5, 0) * directionToPlayer);  // Un poco a la der
        
        yield return new WaitForSeconds(1.5f);

        // TRES PROYECTILES cónico cecalculamos target OTRA VEZ
        directionToPlayer = GetAimDirection();
        SpawnProjectile(directionToPlayer); // Centro
        SpawnProjectile(Quaternion.Euler(0, -25, 0) * directionToPlayer); // Izquierda abierta
        SpawnProjectile(Quaternion.Euler(0, 25, 0) * directionToPlayer);  // Derecha abierta

        // Un poco de CD antes de que vuelva a atacar 
        yield return new WaitForSeconds(attackCycleCooldown);
        isAttacking = false;
    }

    // Función auxiliar para calcular dónde está el jugador en este instante
    private Vector3 GetAimDirection()
    {
        if (playerTransform == null) return transform.forward;

        // Tirar a la ultima posición registrada del player
        Vector3 targetPos = playerTransform.position;
        // Apuntar al pecho del jugador, no a los pies
        targetPos.y += 1f; 

        return (targetPos - firePointArm.position).normalized;
    }

    private void SpawnProjectile(Vector3 dir)
    {
        if (fireballPrefab == null || firePointArm == null) return;
        
        GameObject proj = Instantiate(fireballPrefab, firePointArm.position, Quaternion.identity);
        BossProjectile script = proj.GetComponent<BossProjectile>();
        if (script != null) script.Initialize(dir);
    }

    // DAÑO Y MUERTE
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // Integración con números flotantes 
        DamageNumberManager.Instance?.ShowDamage(amount, transform.position + Vector3.up * 2);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    { 
        // Pausar juego y mostrar victoria
        Time.timeScale = 0f;
        
        // MARCOOOS UIUIIUIUIUI
        
        Destroy(gameObject);
    }

    // Dibujar el rango en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}