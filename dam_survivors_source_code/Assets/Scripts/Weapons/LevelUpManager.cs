using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class LevelUpManager : MonoBehaviour
{
    [Header("Referencias Generales")]
    public WeaponManager weaponManager; 
    public GameObject levelUpPanel;     
    public Transform cardsContainer;    
    
    [Header("Prefabs de Cartas")]
    public GameObject cardPrefab;          // Carta normal
    public GameObject evolutionCardPrefab; // Carta dorada/especial

    [Header("UI Extra")]
    public GameObject gameplayHUD;      
    public GameObject allMaxedPanel; // Panel "Todo al Máximo"

    [Header("Configuración Animación")]
    public float delayBetweenCards = 0.15f; 

    void Start()
    {
        levelUpPanel.SetActive(false);
        if(allMaxedPanel != null) allMaxedPanel.SetActive(false);
        if(gameplayHUD != null) gameplayHUD.SetActive(true);
    }

    // --- ENTRADA 1: COFRE ---
    public void OpenChest(bool isEvolvedChest)
    {
        if (isEvolvedChest)
        {
            BaseLauncher weaponToEvolve = GetReadyEvolution();

            if (weaponToEvolve != null)
            {
                SetupPanel();
                CreateCards(new List<BaseLauncher> { weaponToEvolve });
            }
            else
            {
                // Si es cofre evo pero no hay nada listo, damos mejora normal
                ShowLevelUpOptions(); 
            }
        }
        else
        {
            ShowLevelUpOptions(); // Cofre normal = Level Up normal
        }
    }

    // --- ENTRADA 2: SUBIDA DE NIVEL (XP) ---
    public void ShowLevelUpOptions()
    {
        SetupPanel();

        // Buscamos mejoras (Filtrando las evoluciones)
        List<BaseLauncher> options = GetNormalUpgrades(3);

        if (options.Count > 0)
        {
            CreateCards(options);
        }
        else
        {
            ShowMaxLevelMessage();
        }
    }

    // --- LÓGICA INTERNA ---

    void SetupPanel()
    {
        Time.timeScale = 0f;
        if(gameplayHUD != null) gameplayHUD.SetActive(false);
        foreach (Transform child in cardsContainer) Destroy(child.gameObject);
        levelUpPanel.SetActive(true);
    }

    void CreateCards(List<BaseLauncher> launchers)
    {
        int index = 0;
        foreach (var launcher in launchers)
        {
            if(launcher.weaponData != null)
            {
                // DECISIÓN: ¿Usamos carta normal o carta de evolución?
                GameObject prefabToUse = cardPrefab;

                // Si está lista para evolucionar (Nivel 9 -> 10) usamos la carta especial
                if (launcher.isUnlocked && launcher.level == 9 && evolutionCardPrefab != null)
                {
                    prefabToUse = evolutionCardPrefab;
                }

                GameObject newCard = Instantiate(prefabToUse, cardsContainer);
                
                // Calculamos retraso para efecto cascada
                float myDelay = index * delayBetweenCards;
                
                newCard.GetComponent<UpgradeCardUI>().Setup(launcher, this, myDelay);
                index++;
            }
        }
    }

    void ShowMaxLevelMessage()
    {
        levelUpPanel.SetActive(true);
        if (allMaxedPanel != null) allMaxedPanel.SetActive(true);
        else CloseLevelUpMenu(); // Si no hay panel, cerramos para no bloquear
    }

    // Filtro para mejoras normales (Oculta las que esperan evolución)
    List<BaseLauncher> GetNormalUpgrades(int amount)
    {
        List<BaseLauncher> allWeapons = new List<BaseLauncher>(weaponManager.GetAllWeapons());
        
        // Quitamos Nivel 10 (Max) y Nivel 9 (Esperando cofre)
        allWeapons.RemoveAll(w => (w.isUnlocked && w.level >= 10) || (w.isUnlocked && w.level == 9));

        List<BaseLauncher> selected = new List<BaseLauncher>();

        for (int i = 0; i < amount; i++)
        {
            if (allWeapons.Count == 0) break;
            int randomIndex = Random.Range(0, allWeapons.Count);
            selected.Add(allWeapons[randomIndex]);
            allWeapons.RemoveAt(randomIndex);
        }
        return selected;
    }

    // Filtro para encontrar armas listas para evolucionar
    BaseLauncher GetReadyEvolution()
    {
        List<BaseLauncher> allWeapons = weaponManager.GetAllWeapons();
        List<BaseLauncher> ready = allWeapons.Where(w => w.isUnlocked && w.level == 9).ToList();
        if (ready.Count > 0) return ready[Random.Range(0, ready.Count)];
        return null;
    }

    public void SelectUpgrade(WeaponData data)
    {
        weaponManager.ApplyUpgradeToWeapon(data);
        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        levelUpPanel.SetActive(false);
        if(allMaxedPanel != null) allMaxedPanel.SetActive(false);
        if(gameplayHUD != null) gameplayHUD.SetActive(true);
        Time.timeScale = 1f;
    }
}