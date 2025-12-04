using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Configuración del Cofre")]
    [Tooltip("Si es TRUE, intenta evolucionar un arma. Si es FALSE, da una mejora normal.")]
    public bool isEvolvedChest = false;
    private bool isOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isOpened) return;

        if (other.CompareTag("Player"))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        LevelUpManager manager = FindObjectOfType<LevelUpManager>();
        
        if (manager != null)
        {
            manager.OpenChest(isEvolvedChest);
        }

        Destroy(gameObject);
    }
}