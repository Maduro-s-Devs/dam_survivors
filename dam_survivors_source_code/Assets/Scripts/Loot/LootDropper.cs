using UnityEngine;
using System.Collections.Generic;

public class LootDropper : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public string name;           
        public GameObject prefab;     
        [Range(0, 100)]
        public float dropChance;      
    }

    [Header("Configuración de Drops")]
    [SerializeField] private List<LootItem> lootTable;

    [Header("Cofres Especiales")]
    [SerializeField] private GameObject normalChestPrefab;    
    [SerializeField] private GameObject evolutionChestPrefab; 
    [Range(0, 100)]
    [SerializeField] private float chestDropChance = 1f;      

    public void DropLoot()
    {
        // CHEQUEO DE COFRE
        if (Random.Range(0f, 100f) <= chestDropChance)
        {
            SpawnChest();
            return; 
        }

        // CHEQUEO DE OTROS ITEMS
        foreach (LootItem item in lootTable)
        {
            if (Random.Range(0f, 100f) <= item.dropChance)
            {
                // Usamos la función inteligente para buscar el suelo
                Vector3 spawnPos = GetGroundPosition();
                Instantiate(item.prefab, spawnPos, Quaternion.identity);
            }
        }
    }

    private void SpawnChest()
    {
        GameObject chestToSpawn = normalChestPrefab;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // LÓGICA DE EVOLUCIÓN
        if (player != null)
        {
            WeaponManager manager = player.GetComponent<WeaponManager>();
            if (manager != null && manager.HasWeaponReadyToEvolve())
            {
                chestToSpawn = evolutionChestPrefab; 
                Debug.Log("¡Condición de Evolución Cumplida! Soltando Cofre Especial.");
            }
        }

        if (chestToSpawn != null)
        {
            // CAMBIO: Usamos la función inteligente para buscar el suelo
            Vector3 dropPosition = GetGroundPosition();

            // Instanciamos el cofre
            GameObject chest = Instantiate(chestToSpawn, dropPosition, Quaternion.identity);

            // CORRECCIÓN DE ROTACIÓN
            if (player != null)
            {
                chest.transform.LookAt(player.transform);
                Vector3 currentRot = chest.transform.eulerAngles;
                chest.transform.eulerAngles = new Vector3(0, currentRot.y, 0);
            }
        }
    }

    // BUSCADOR DE SUELO ---
    private Vector3 GetGroundPosition()
    {
        RaycastHit hit;
        // Lanzamos un rayo desde el ombligo del enemigo (transform.position + un poco arriba) hacia ABAJO
        // para encontrar dónde está el suelo realmente.
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 100f))
        {
            // Si el rayo toca suelo, devolvemos ese punto exacto
            return hit.point;
        }
        
        // Si por lo que sea el rayo no toca nada devolvemos la posición del enemigo tal cual
        return transform.position;
    }
}