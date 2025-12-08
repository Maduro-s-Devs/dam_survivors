using UnityEngine;

public class KeepOnLoad : MonoBehaviour
{
    private static KeepOnLoad instance;

    private void Awake()
    {
        // Asegura que solo haya UN Player
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe un Player, lo destruye para no tener duplicados.
            Destroy(gameObject);
        }
    }
}