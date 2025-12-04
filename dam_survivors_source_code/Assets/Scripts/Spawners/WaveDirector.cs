using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveDirector : MonoBehaviour
{
    [Header("Configuration")]
    // Arrastra aquí tus archivos Wave 1, Wave 2, etc.
    [SerializeField] private List<WaveProfile> allWaves; 
    
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private float spawnRadius = 15f; // Distancia a la que aparecen los enemigos   

    // Referencia interna al Timer para coordinar (opcional si usamos lógica de secuencia)
    private GameTimer gameTimer;
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private void Start()
    {
        // Buscamos al Player automáticamente si no está asignado
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        // Buscamos el Timer (necesario para eventos de fin de juego/Boss)
        gameTimer = FindObjectOfType<GameTimer>();

        // Arrancamos la primera oleada inmediatamente
        StartNextWave();
    }

    private void Update()
    {
        // Aquí podrías añadir lógica si necesitas pausar oleadas o esperar al Timer
        // De momento, el sistema funciona por secuencia: acaba una -> empieza la otra.
    }

    private void StartNextWave()
    {
        // Si se acabaron las oleadas, paramos (aquí iría la lógica de esperar al Boss)
        if (currentWaveIndex >= allWaves.Count)
        {
            Debug.Log("--- ¡FIN DE TODAS LAS OLEADAS! (Esperando al Minuto 10) ---");
            return;
        }

        WaveProfile profile = allWaves[currentWaveIndex];
        Debug.Log($"<color=yellow> >>> INICIANDO OLEADA {currentWaveIndex + 1} <<< </color>");
        
        // Iniciamos el proceso de esta oleada
        StartCoroutine(ProcessWave(profile));
    }

    // Corrutina principal que gestiona la vida de una Oleada completa
    private IEnumerator ProcessWave(WaveProfile profile)
    {
        isWaveActive = true;

        // 1. EVENTO ESPECIAL (Si el archivo tiene un Special Enemy, como el Invocador)
        if (profile.specialEnemy != null)
        {
            Debug.Log($"[EVENTO] Spawn Especial: {profile.specialEnemy.name}");
            for (int i = 0; i < profile.specialCount; i++) 
            {
                SpawnEnemy(profile.specialEnemy);
            }
        }

        // 2. PROCESAR GRUPOS DE ENEMIGOS
        // Creamos una lista de tareas para ejecutar todos los grupos configurados
        List<Coroutine> activeGroups = new List<Coroutine>();

        foreach (var groupConfig in profile.groupsInWave)
        {
            // Lanzamos la rutina de ese grupo (ej: Zánganos) y guardamos la referencia
            Coroutine c = StartCoroutine(SpawnGroupRoutine(groupConfig));
            activeGroups.Add(c);
        }

        // Esperamos a que TODOS los grupos de esta oleada terminen de salir
        foreach (var c in activeGroups) yield return c;

        // 3. FIN DE LA OLEADA
        Debug.Log($"<color=green> --- Oleada {currentWaveIndex + 1} completada --- </color>");
        
        isWaveActive = false;
        currentWaveIndex++;
        
        // Aquí podrías poner un "yield return new WaitForSeconds(5);" si quieres descanso entre oleadas
        // Si no, arrancamos la siguiente inmediatamente:
        StartNextWave();
    }

    // Rutina que gestiona las ráfagas repetitivas (ej: "12 enemigos cada 5s")
    private IEnumerator SpawnGroupRoutine(WaveProfile.WaveGroup config)
    {
        // Repetimos tantas veces como diga "Number Of Groups"
        for (int i = 0; i < config.numberOfGroups; i++)
        {
            // Spawnear el bloque entero de enemigos (ej: los 12 de golpe)
            for (int j = 0; j < config.enemiesPerGroup; j++)
            {
                SpawnEnemy(config.prefab);
                
                // Pequeñísimo retardo (0.1s) para que no salgan todos en el mismo milisegundo exacto
                // y la CPU no sufra un pico
                yield return new WaitForSeconds(0.1f); 
            }

            Debug.Log($"[SPAWNER] Grupo {i+1}/{config.numberOfGroups} de {config.prefab.name} lanzado.");

            // Si aún quedan grupos por salir, esperamos el tiempo definido (ej: 5s)
            if (i < config.numberOfGroups - 1)
            {
                yield return new WaitForSeconds(config.timeBetweenGroups);
            }
        }
    }

    // Lógica para crear el enemigo en el mundo
    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || playerTransform == null) return;

        // 1. Calcular posición aleatoria en un círculo alrededor del jugador
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // 2. Ajustar la altura al suelo (Raycast) para que no nazcan en el aire o bajo tierra
        spawnPos = GetGroundPosition(spawnPos);

        // 3. Instanciar
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // Función auxiliar para encontrar el suelo
    private Vector3 GetGroundPosition(Vector3 pos)
    {
        RaycastHit hit;
        // Lanzamos un rayo desde el cielo (+50 metros) hacia abajo
        if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            return hit.point; // Devolvemos el punto de impacto con la lava/tierra
        }
        return pos; // Si no encuentra suelo, usamos la posición original
    }
}