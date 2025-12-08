// este script lo lleva el proyectil del boss
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lifeTime = 5f; // Que se destruya

    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
        
        // Rotamos el proyectil para que mire hacia donde va
        transform.rotation = Quaternion.LookRotation(moveDirection);
        
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Movimiento rectilíneo constante
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}