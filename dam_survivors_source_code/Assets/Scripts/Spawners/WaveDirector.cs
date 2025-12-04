using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveDirector : MonoBehaviour
{
    [System.Serializable]
    public class WaveScheduleItem
    {
        public WaveProfile waveData;   // El archivo de la oleada
        public float startTime;        // Segundo exacto en que empieza
    }

    [Header("Configuración de Tiempo")]
    [SerializeField] private List<WaveScheduleItem> schedule; 
    
    [Header("Configuración de Spawn")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private float spawnRadius = 15f; 
    [SerializeField] private LayerMask invalidLayers; // Capas prohibidas 
    [SerializeField] private float spawnHeightOffset = 1.5f; // Para que no nazcan enterrados
    
    private GameTimer gameTimer;
    private int currentWaveIndex = 0;
    private float nextWaveTime = 0f;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        gameTimer = FindObjectOfType<GameTimer>();

        // Ordenamos la lista por tiempo para evitar errores humanos
        schedule.Sort((x, y) => x.startTime.CompareTo(y.startTime));

        if (schedule.Count > 0)
        {
            nextWaveTime = schedule[0].startTime;
        }
    }

    private void Update()
    {
        if (currentWaveIndex >= schedule.Count) return;

        // Si el reloj llega al momento de la siguiente oleada...
        if (gameTimer.CurrentTime >= nextWaveTime)
        {
            StartWave(schedule[currentWaveIndex].waveData);
            
            currentWaveIndex++;
            
            if (currentWaveIndex < schedule.Count)
            {
                nextWaveTime = schedule[currentWaveIndex].startTime;
            }
        }
    }

    private void StartWave(WaveProfile profile)
    {
        Debug.Log($"<color=yellow> >>> INICIANDO OLEADA: {profile.name} (T: {Mathf.Floor(gameTimer.CurrentTime)}s) <<< </color>");
        StartCoroutine(ProcessWave(profile));
    }

    private IEnumerator ProcessWave(WaveProfile profile)
    {
        // EVENTO ESPECIAL (Invocador, etc)
        if (profile.specialEnemy != null)
        {
            for (int i = 0; i < profile.specialCount; i++) SpawnEnemy(profile.specialEnemy);
        }

        // GRUPOS DE ENEMIGOS
        foreach (var groupConfig in profile.groupsInWave)
        {
            StartCoroutine(SpawnGroupRoutine(groupConfig));
        }
        yield return null;
    }

    private IEnumerator SpawnGroupRoutine(WaveProfile.WaveGroup config)
    {
        for (int i = 0; i < config.numberOfGroups; i++)
        {
            for (int j = 0; j < config.enemiesPerGroup; j++)
            {
                SpawnEnemy(config.prefab);
                yield return new WaitForSeconds(0.1f); 
            }

            if (i < config.numberOfGroups - 1)
            {
                yield return new WaitForSeconds(config.timeBetweenGroups);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || playerTransform == null) return;

        Vector3 spawnPos = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;

        // Búsqueda de suelo válido
        while (!validPositionFound && attempts < 10)
        {
            attempts++;
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 potentialPos = playerTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            RaycastHit hit;
            if (Physics.Raycast(potentialPos + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                // Evitar lava
                if (((1 << hit.collider.gameObject.layer) & invalidLayers) != 0) continue;

                spawnPos = hit.point + (Vector3.up * spawnHeightOffset);
                validPositionFound = true;
            }
        }

        if (validPositionFound)
        {
            GameObject enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);

            // ---  ESCALADO DINÁMICO (Minuto 8+) ---
            float currentTime = gameTimer.CurrentTime;
            float scalingStartTime = 480f; // 8 Minutos

            if (currentTime > scalingStartTime)
            {
                float timeOver = currentTime - scalingStartTime;
                
                // +10% de estadísticas cada 10 segundos extra
                float multiplier = 1f + (timeOver * 0.01f); 

                EnemyController controller = enemyObj.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.ApplyDifficultyScaling(multiplier);
                }
            }
            // -----------------------------------------------
        }
    }
}