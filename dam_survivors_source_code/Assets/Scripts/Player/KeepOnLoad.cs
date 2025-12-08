using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para detectar escenas

public class KeepOnLoad : MonoBehaviour
{
    private static KeepOnLoad instance;

    private void Awake()
    {
        // Asegura que solo haya UN Player (Singleton)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe un Player (ej. volviste al menú y entraste de nuevo), destruye el nuevo
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento "sceneLoaded"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se ejecuta AUTOMÁTICAMENTE cada vez que carga una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verifica si la escena cargada es la del Boss
        if (scene.name == "BossArena") 
        {
            Debug.Log("Llegada a la Boss Arena detectada. Teletransportando...");
            
            // CharacterController hay que desactivarlo antes de moverlo
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // ASIGNAR LA POSICIÓN 
            transform.position = new Vector3(142f, 51f, 102f);
            
            // transform.rotation = Quaternion.Euler(0, 0, 0); 

            // Reactivamos el controller
            if (cc != null) cc.enabled = true;
        }
    }
}