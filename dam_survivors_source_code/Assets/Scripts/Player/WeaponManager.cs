// este script lo usa Player
using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon stash")]
    // Todos los scripts de armas que tenga el Player (Varita, Molotov, etc.)
    [SerializeField] private List<BaseLauncher> allWeapons; 

    private void Start()
    {
        //  Asegurarnos al inicio de que todas están bloqueadas salvo la primera
        // (Si quisieras que empiece con una por defecto)
    }

    // --- FUNCIONES PARA LA UI (Marcos) ---

    // Pasa el ID del arma (0 = Varita, 1 = Molotov...)
    public void UnlockWeapon(int index)
    {
        if (index >= 0 && index < allWeapons.Count)
        {
            allWeapons[index].ActivateWeapon();
        }
        else
        {
            Debug.LogWarning("WeaponManager: Índice de arma incorrecto.");
        }
    }

    // Subir de nivel un arma específica
    public void LevelUpWeapon(int index)
    {
        if (index >= 0 && index < allWeapons.Count)
        {
            BaseLauncher weapon = allWeapons[index];
            
            // Subimos el nivel
            weapon.level++;
            
            // Recalculamos daño y cooldown
            weapon.CalculateStats();

            Debug.Log($"Arma {weapon.name} subida a Nivel {weapon.level}");
        }
    }

    // Saber si un arma ya la tenemos al máx y no mostrarla en la UI de selección
    public BaseLauncher GetWeapon(int index)
    {
        if (index >= 0 && index < allWeapons.Count)
        {
            return allWeapons[index];
        }
        return null;
    }
}