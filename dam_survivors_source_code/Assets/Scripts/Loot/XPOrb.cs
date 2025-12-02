using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float xpAmount = 10f; 
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float acceleration = 15f; 

    private Transform playerTransform;
    private bool isMagnetized = false;
    private float currentSpeed;

    private void Start()
    {
        // Buscando al player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerTransform = player.transform;
            currentSpeed = moveSpeed;
        }
        else
        {
            // SI SALE ESTO, ES QUE TU PLAYER NO TIENE EL TAG "Player"
            Debug.LogError("XP ORB ERROR: ¡No encuentro ningún objeto con Tag 'Player' en la escena!");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // CALCULAR DISTANCIA
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // CHEQUEO DE IMÁN
        PlayerExperience playerXP = playerTransform.GetComponent<PlayerExperience>();
        
        if (playerXP != null)
        {
            if (distance <= playerXP.pickupRadius)
            {
                isMagnetized = true;
            }
        }
        else
        {
            Debug.LogWarning("XP ORB: Veo al Player, pero no tiene el script 'PlayerExperience'.");
        }

        // MOVIMIENTO HACIA EL JUGADOR
        if (isMagnetized)
        {
            currentSpeed += acceleration * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, currentSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerExperience playerXP = other.GetComponent<PlayerExperience>();
            if (playerXP != null)
            {
                playerXP.AddExperience(xpAmount);
                Debug.Log($"[XP] Recogido orbe. +{xpAmount} XP.");
            }
            Destroy(gameObject);
        }
    }
}