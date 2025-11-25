// este script, lo lleva el invocador y el Boss
using UnityEngine;
using System.Collections.Generic;

public class SwarmSpawner : MonoBehaviour
{
    // Creamos una "orden" para definir qué enemigo y cuántos de ese tipo queremos
    [System.Serializable]
    public struct SpawnUnit
    {
        public GameObject prefab; // El tipo de enemigo
        public int count;         // Cuántos de este tipo concreto
    }

    [Header("Configuración de la Oleada")]
    // Puedes añadir tantos tipos diferentes como quieras pulsando "+" en el Inspector.
    [SerializeField] private List<SpawnUnit> enemiesToSpawn; 

    [Header("Configuración de Tiempo")]
    [SerializeField] private bool spawnOnAwake = true;    // ¿Invoca nada más nacer?
    [SerializeField] private bool loopSpawning = false;   // ¿Sigue invocando infinitamente?
    [SerializeField] private float spawnInterval = 10f;   // Tiempo entre casteo y casteo (si loop es true)

    [Header("Configuración de Área")]
    [SerializeField] private float spawnRadius = 3f;      // Distancia a la que aparecen

    private float timer;

    private void Start()
    {
        // Inicializamos el timer con el intervalo para que si tiene que esperar, espere desde el principio o si spawnea al inicio, lo hace y luego resetea.
        timer = spawnInterval;

        if (spawnOnAwake)
        {
            SpawnGroup();
        }
    }

    private void Update()
    {
        // Ejecutamos la lógica de tiempo si está marcado el bucle
        if (loopSpawning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SpawnGroup();
                timer = spawnInterval; // Reiniciar el contador
            }
        }
    }

    //  Invocación y debug, para poder detectar errores
    public void SpawnGroup()
    {
        // Verificamos si la lista está vacía
        if (enemiesToSpawn == null || enemiesToSpawn.Count == 0)
        {
            Debug.LogWarning("SwarmSpawner: ¡La lista 'Enemies To Spawn' está vacía!");
            return;
        }

        // Recorremos la lista de tipos de enemigos que has configurado
        foreach (SpawnUnit unit in enemiesToSpawn)
        {
            if (unit.prefab == null) continue; // Por si no hay prefab, se ignora

            // Para cada tipo, hacemos un bucle según su cantidad específica
            for (int i = 0; i < unit.count; i++)
            {
                // Generamos una posición aleatoria alrededor
                Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

                Instantiate(unit.prefab, spawnPos, Quaternion.identity);
            }
        }
    }

    // Para ver el radio en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}