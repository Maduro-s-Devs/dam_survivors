using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Survivor/Wave Profile")]
public class WaveProfile : ScriptableObject
{
    [System.Serializable]
    public class WaveGroup
    {
        public GameObject prefab;      // Qué enemigo
        public int enemiesPerGroup;    // Cuántos salen DE GOLPE en cada grupo 
        public int numberOfGroups;     // Cuántas veces se repite este grupo 
        public float timeBetweenGroups;// Tiempo de espera entre un grupo y el siguiente 
        
        [HideInInspector] public float spawnRateWithinGroup = 0.1f; 
    }

    [Header("Configuración de Ráfagas")]
    public List<WaveGroup> groupsInWave;

    [Header("Evento Especial (Jefes/Enjambre Único)")]
    public GameObject specialEnemy;   
    public int specialCount = 1;      
}