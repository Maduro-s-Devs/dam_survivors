using UnityEngine;

public class BaseLauncher : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float baseCooldown = 1.5f;

    [Header("Progression")]
    // Esta variable 'level' permite el escalado manual
    [Range(1, 10)]
    public int level = 1; 

    // Variables protegidas para que los scripts hijos como MagicWand puedan usarlas
    protected float currentDamage;
    protected float currentCooldown;
    protected float cooldownTimer;

    protected virtual void Start()
    {
        CalculateStats();
        cooldownTimer = 0f; // Listo para disparar inmediatamente
    }

    protected virtual void Update()
    {
        // Gestión del temporizador automática para todas las armas
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            AttemptToFire(); // Ejecuta la lógica de disparo específica del arma que se requiera
            cooldownTimer = currentCooldown; // Reinicia el timer usando el cooldown calculado
        }
    }

    // Calcula las estadísticas actuales basadas en el nivel
    public void CalculateStats()
    {
        // El daño se multiplica con el nivel
        currentDamage = baseDamage * level; 
        
        // El cooldown se reduce. Usamos raíz cuadrada para un escalado suave, evitamos dividir por cero o tener cooldowns negativos
        currentCooldown = baseCooldown / Mathf.Sqrt(level);
        currentCooldown = Mathf.Max(currentCooldown, 0.1f);
    }

    // Método virtual vacío diseñado para ser sobrescrito (override) por las armas hijas
    protected virtual void AttemptToFire()
    {
        // Lógica vacía por defecto. Las armas son las que eligen como disparar
    }

    // Permite ver los cambios de estadísticas en tiempo real en el Editor
    private void OnValidate()
    {
        CalculateStats();
    }
}