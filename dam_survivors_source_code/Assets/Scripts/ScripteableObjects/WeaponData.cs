using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Survivor/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Base Info (lvl 1-9)")]
    public string weaponName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Descriptions per Level")]
    [Tooltip("Index 0 = Info para subir a Nivel 2. Index 1 = Para subir a Nivel 3...")]
    [TextArea(2, 3)] // Hace la caja de texto más grande en el editor
    public List<string> levelUpDescriptions;

    [Header("Evolution (lvl 10)")]
    public string evolvedName;          // Ej: "Fuego Infernal"
    [TextArea] public string evolvedDescription; // Ej: "Deja zonas de fuego permanente."
    public Sprite evolvedIcon;          // El icono rojo/dorado chulo
}