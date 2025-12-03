using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [Header("Referencias Generales")]
    public WeaponManager weaponManager; // Script del Player
    public GameObject levelUpPanel;     // El panel negro de fondo
    public Transform cardsContainer;    // Donde se crean las cartas
    public GameObject cardPrefab;       // El prefab de la carta

    [Header("UI Juego (HUD)")]
    public GameObject gameplayHUD;      // La vida, XP, joystick... para ocultarlo

    [Header("Configuración Cascada")]
    public float delayBetweenCards = 0.15f; // Tiempo de espera entre carta y carta

    void Start()
    {
        levelUpPanel.SetActive(false);
        if(gameplayHUD != null) gameplayHUD.SetActive(true);
    }

    // Esta función la llama el PlayerExperience al subir de nivel
    public void ShowLevelUpOptions()
    {
        // 1. Pausar el juego
        Time.timeScale = 0f;

        // 2. Ocultar el HUD para limpiar la pantalla
        if(gameplayHUD != null) gameplayHUD.SetActive(false);
        
        // 3. Limpiar cartas viejas
        foreach (Transform child in cardsContainer) Destroy(child.gameObject);

        // 4. Obtener 3 armas aleatorias
        List<BaseLauncher> options = GetRandomWeapons(3);

        // 5. Crear las cartas con EFECTO CASCADA
        int index = 0;

        foreach (var launcher in options)
        {
            if(launcher.weaponData != null)
            {
                GameObject newCard = Instantiate(cardPrefab, cardsContainer);
                
                // Calculamos el retraso: Carta 1 = 0s, Carta 2 = 0.15s, Carta 3 = 0.30s
                float myDelay = index * delayBetweenCards;
                
                // Pasamos el retraso al Setup de la carta
                newCard.GetComponent<UpgradeCardUI>().Setup(launcher, this, myDelay);
                
                index++;
            }
        }

        // 6. Mostrar el panel
        levelUpPanel.SetActive(true);
    }

    // Lógica para elegir armas al azar
    List<BaseLauncher> GetRandomWeapons(int amount)
    {
        List<BaseLauncher> allWeapons = new List<BaseLauncher>(weaponManager.GetAllWeapons());
        List<BaseLauncher> selected = new List<BaseLauncher>();

        // Opcional: No mostrar armas que ya están a nivel máximo
        allWeapons.RemoveAll(w => w.level >= 10 && w.isUnlocked);

        for (int i = 0; i < amount; i++)
        {
            if (allWeapons.Count == 0) break;
            int randomIndex = Random.Range(0, allWeapons.Count);
            selected.Add(allWeapons[randomIndex]);
            allWeapons.RemoveAt(randomIndex);
        }
        return selected;
    }

    // Al elegir una carta
    public void SelectUpgrade(WeaponData data)
    {
        weaponManager.ApplyUpgradeToWeapon(data);
        
        levelUpPanel.SetActive(false);
        
        // Volver a mostrar el HUD y reanudar el tiempo
        if(gameplayHUD != null) gameplayHUD.SetActive(true);
        Time.timeScale = 1f;
    }
}