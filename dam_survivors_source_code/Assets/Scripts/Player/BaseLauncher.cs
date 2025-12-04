using UnityEngine;

public class BaseLauncher : MonoBehaviour
{
    [Header("Identity")]
    public WeaponData weaponData; 

    [Header("Weapon state")]
    public bool isUnlocked = false; 

    [Header("Base Settings")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float baseCooldown = 1.5f;

    [Header("Progression")]
    [Range(1, 10)]
    public int level = 1; 

    protected float currentDamage;
    protected float currentCooldown;
    protected float cooldownTimer;

    protected virtual void Start()
    {
        CalculateStats();
        cooldownTimer = 0f; 
    }

    protected virtual void Update()
    {
        if (!isUnlocked) return;
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            AttemptToFire(); 
            cooldownTimer = currentCooldown; 
        }
    }

    public void Upgrade()
    {
        if (!isUnlocked) ActivateWeapon();
        else
        {
            level++;
            // IMPORTANTE: Si es el escudo, esto tiene que llamar al CalculateStats del escudo
            // para que regenere las bolas con la nueva velocidad/cantidad.
        }
        CalculateStats(); 
    }

    public virtual void ActivateWeapon()
    {
        isUnlocked = true;
        level = 1; 
        cooldownTimer = 0f; 
        CalculateStats();
        Debug.Log($"{gameObject.name} desbloqueada.");
    }

    public virtual void CalculateStats()
    {
        currentDamage = baseDamage * level; 
        currentCooldown = baseCooldown / Mathf.Sqrt(level);
        currentCooldown = Mathf.Max(currentCooldown, 0.1f);
    }

    protected virtual void AttemptToFire() { }

    private void OnValidate() { CalculateStats(); }
}