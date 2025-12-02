// Este script lo utilizan lons enemigo
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

    [Header("Drops Configuration")]
    [SerializeField] private List<LootItem> lootTable;

    [Header("Special chests settings")]
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
                // Que los items caigan al suelo
                Vector3 itemPos = new Vector3(transform.position.x, 0f, transform.position.z);
                Instantiate(item.prefab, itemPos, Quaternion.identity);
            }
        }
    }

    private void SpawnChest()
    {
        GameObject chestToSpawn = normalChestPrefab;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // EVOLUCIÓN
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
            // CALCULAR POSICIÓN EN EL SUELO (Y = 0)
            // Ignoramos la altura del enemigo y lo ponemos a ras de suelo.
            Vector3 dropPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            // Instanciamos el cofre en el suelo
            GameObject chest = Instantiate(chestToSpawn, dropPosition, Quaternion.identity);

            // HACER QUE MIRE AL JUGADOR
            if (player != null)
            {
                // Hacemos que el frente del cofre apunte al jugador
                chest.transform.LookAt(player.transform);

                // IMPORTANTE: Corregir inclinación
                // Forzamos que la rotación en X y Z sea 0 para que el cofre esté plano en el suelo.
                Vector3 currentRot = chest.transform.eulerAngles;
                chest.transform.eulerAngles = new Vector3(0, currentRot.y, 0);
            }
        }
    }
}