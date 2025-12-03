// este script, lo lleva el player
using UnityEngine;

public class BaseLauncher : MonoBehaviour
{
    [Header("Identity")]
    public WeaponData weaponData; // ScriptableObject que define el arma

    [Header("Weapon state")]
    // Si está FALSE, el arma existe pero no dispara. 
    // Lo ponemos false por defecto para que vengan bloqueadas.
    [SerializeField] public bool isUnlocked = false; 

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
        //  Si el arma no está desbloqueada, no hacemos nada.
        if (!isUnlocked) return;

        // Gestión del temporizador automática
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            AttemptToFire(); 
            cooldownTimer = currentCooldown; 
        }
    }

    public void Upgrade()
    {
        if (!isUnlocked)
        {
            ActivateWeapon();
        }
        else
        {
            level++;
        }
        CalculateStats();
    }

    // Método que llamará tu UI para desbloquear el arma (Marcos)
    public void ActivateWeapon()
    {
        isUnlocked = true;
        level = 1; 
        // Reiniciar el timer para que dispare nada más desbloquearla
        cooldownTimer = 0f; 
        CalculateStats();
        Debug.Log($"{gameObject.name} ha sido desbloqueada.");
    }

    public void CalculateStats()
    {
        currentDamage = baseDamage * level; 
        currentCooldown = baseCooldown / Mathf.Sqrt(level);
        currentCooldown = Mathf.Max(currentCooldown, 0.1f);
    }

    protected virtual void AttemptToFire()
    {
        // Lógica vacía por defecto. Las armas son las que deciden como van adisparar o lanzarse
    }

    private void OnValidate()
    {
        CalculateStats();
    }
}