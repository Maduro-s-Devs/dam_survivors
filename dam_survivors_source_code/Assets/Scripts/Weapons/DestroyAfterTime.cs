// este script lo pueden utilizar los efectos de VFX para explosiones
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // Tiempo antes de desaparecer

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}