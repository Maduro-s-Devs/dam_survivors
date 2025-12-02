// Este script lo lleva el Player
using UnityEngine;

public class SlashLauncher : BaseLauncher
{
    [Header("Slash Settings")]
    [SerializeField] private GameObject slashPrefab;         // Prefab normal 
    [SerializeField] private float baseWidth = 2f;           // Ancho inicial
    [SerializeField] private float baseLength = 2f;          // Largo inicial (Hacia delante)
    
    [Header("Level scale")]
    [SerializeField] private float widthMultiplier = 0.5f;   // Cuánto ancha por nivel
    [SerializeField] private float lengthMultiplier = 0.5f;  // Cuánto alarga por nivel

    [Header("Final evolution Level 10")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private GameObject ultimateSlashPrefab; // Prefab evolucionado 
    [SerializeField] private float lifestealPerHit = 1f;     // Robo de vida

    protected override void Start()
    {
        base.Start();
        // Es la que el player va a tener activada por defecto
        isUnlocked = true; 
    }

    protected override void AttemptToFire()
    {
        // Calcular Tamaño según Nivel
        float currentWidth = baseWidth + ((level - 1) * widthMultiplier);
        float currentLength = baseLength + ((level - 1) * lengthMultiplier);

        // Elegir Prefab (Normal o Ultimate)
        GameObject prefabToUse = slashPrefab;
        bool isEvolved = false;

        if (level >= maxLevel)
        {
            isEvolved = true;
            if (ultimateSlashPrefab != null) prefabToUse = ultimateSlashPrefab;
            
            // Si evoluciona, hacerlo gigante de golpe
            currentWidth *= 1.5f;
            currentLength *= 1.5f;
        }

        // GOLPE FRONTAL (Matemática para crecer solo hacia adelante)
        // El centro del objeto debe estar a (Largo / 2) metros delante del jugador.
        // Así, la parte trasera del objeto estará justo en el jugador (0).
        float forwardOffset = currentLength / 2f; 
        Vector3 spawnPos = transform.position + (transform.forward * forwardOffset);

        CreateSlash(spawnPos, transform.rotation, currentWidth, currentLength, isEvolved, prefabToUse);

        // GOLPE TRASERO (Solo si evoluciona)
        if (isEvolved)
        {
            // Hacemos lo mismo pero hacia atrás (-transform.forward)
            Vector3 backPos = transform.position - (transform.forward * forwardOffset);
            Quaternion backRot = transform.rotation * Quaternion.Euler(0, 180, 0);
            
            CreateSlash(backPos, backRot, currentWidth, currentLength, isEvolved, prefabToUse);
        }
    }

    private void CreateSlash(Vector3 pos, Quaternion rot, float width, float length, bool evolved, GameObject prefab)
    {
        // Instanciamos el prefab elegido
        GameObject slash = Instantiate(prefab, pos, rot);
        
        // Hacemos que sea hijo del player para que se mueva con él durante el ataque
        slash.transform.SetParent(transform); 

        // Aplicar el tamaño calculado
        // X = Ancho, Z = Largo (Hacia delante)
        slash.transform.localScale = new Vector3(width, 1f, length);

        // Configurar el script de daño
        SlashAttack script = slash.GetComponent<SlashAttack>();
        if (script != null)
        {
            script.Initialize(currentDamage, evolved, lifestealPerHit);
        }
    }
}