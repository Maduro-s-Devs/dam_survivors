using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
    [Header("General References")]
    public GameObject uiPanel; 
    public WeaponManager weaponManager;
    public PlayerExperience playerExperience;
    public GameTimer gameTimer; // <--- NUEVA REFERENCIA

    [Header("Player UI Reference")]
    public TextMeshProUGUI playerLevelText; 

    [Header("Boss Skip UI")]
    public TextMeshProUGUI bossSkipText; // <--- ARRASTRA AQUÍ TU TEXTO "[0] GO TO BOSS"

    [Header("Weapon UI Rows (Order 1, 2, 3, 4)")]
    public WeaponRowUI[] weaponRows; 

    // --- INPUT SYSTEM ---
    private Controls controls; 
    private bool isMenuOpen = false;

    [System.Serializable]
    public class WeaponRowUI
    {
        public string nameId; 
        public TextMeshProUGUI weaponNameText;  
        public TextMeshProUGUI statusText;      
    }

    private void Awake()
    {
        controls = new Controls();
        controls.UI.DebugMenu.performed += ctx => ToggleMenu();
    }

    private void OnEnable() => controls.UI.Enable();
    private void OnDisable() => controls.UI.Disable();

    void Start()
    {
        // Auto-encontrar referencias
        if (weaponManager == null) weaponManager = FindObjectOfType<WeaponManager>();
        if (playerExperience == null) playerExperience = FindObjectOfType<PlayerExperience>();
        if (gameTimer == null) gameTimer = FindObjectOfType<GameTimer>(); // <--- BUSCAMOS EL TIMER

        if(uiPanel != null) uiPanel.SetActive(false);
        
        // Inicializar texto del Boss si existe
        if (bossSkipText != null)
        {
            bossSkipText.text = "[0] SKIP TO BOSS (End Timer)";
            bossSkipText.color = Color.red; // Color de peligro/importante
        }
    }

    void Update()
    {
        if (isMenuOpen)
        {
            HandleKeyboardInput();
            UpdateUI(); 
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if(uiPanel != null) uiPanel.SetActive(isMenuOpen);
    }

    // --- KEYBOARD INPUT (1-5 y 0) ---
    void HandleKeyboardInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Armas (1-4)
        if (kb.digit1Key.wasPressedThisFrame) TryUpgradeWeapon(0);
        if (kb.digit2Key.wasPressedThisFrame) TryUpgradeWeapon(1);
        if (kb.digit3Key.wasPressedThisFrame) TryUpgradeWeapon(2);
        if (kb.digit4Key.wasPressedThisFrame) TryUpgradeWeapon(3);
        
        // Player (5)
        if (kb.digit5Key.wasPressedThisFrame) LevelUpPlayer();

        // BOSS FIGHT (0)
        if (kb.digit0Key.wasPressedThisFrame) SkipToBoss();
    }

    void TryUpgradeWeapon(int index)
    {
        if (weaponManager == null) return;
        var weapons = weaponManager.GetAllWeapons();

        if (index >= 0 && index < weapons.Count)
        {
            BaseLauncher weapon = weapons[index];
            if (!weapon.isUnlocked) weapon.ActivateWeapon();
            else if (weapon.level < 10) weapon.Upgrade();
        }
    }

    void LevelUpPlayer()
    {
        if (playerExperience != null) playerExperience.AddExperience(1000f);
    }

    // --- FUNCIÓN PARA IR AL BOSS ---
    void SkipToBoss()
    {
        if (gameTimer != null)
        {
            // Llamamos a tu función que pone el tiempo en el límite
            gameTimer.DebugSkipToEnd();
            Debug.Log("DAM SURVIVORS: Saltando al final del contador...");
            
            // Opcional: Cerrar menú para ver la transición
            ToggleMenu(); 
        }
    }

    // --- UI UPDATES ---
    void UpdateUI()
    {
        // 1. Player Text
        if (playerLevelText != null && playerExperience != null)
        {
            playerLevelText.text = $"[5] PLAYER LVL: {playerExperience.CurrentLevel}";
            playerLevelText.color = Color.white; 
        }

        // 2. Weapon Rows
        if (weaponManager != null)
        {
            var weapons = weaponManager.GetAllWeapons();
            
            for (int i = 0; i < weaponRows.Length; i++)
            {
                if (i >= weapons.Count) 
                {
                    if(weaponRows[i].weaponNameText) weaponRows[i].weaponNameText.text = "---";
                    continue; 
                }

                BaseLauncher weapon = weapons[i];
                WeaponRowUI row = weaponRows[i];

                if (row.weaponNameText != null)
                {
                    string wName = weapon.weaponData != null ? weapon.weaponData.name : weapon.name;
                    row.weaponNameText.text = $"[{i + 1}] {wName}";
                }

                if (row.statusText != null)
                {
                    if (!weapon.isUnlocked)
                    {
                        row.statusText.text = "LOCKED";
                        row.statusText.color = Color.red; 
                    }
                    else if (weapon.level >= 10)
                    {
                        row.statusText.text = "MAX [10]";
                        row.statusText.color = Color.yellow;
                    }
                    else
                    {
                        row.statusText.text = $"LVL {weapon.level}";
                        row.statusText.color = Color.cyan;
                    }
                }
            }
        }
    }
}