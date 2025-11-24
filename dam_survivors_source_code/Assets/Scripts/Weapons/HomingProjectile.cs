// Script lo porta el prefab del misil
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 10f;
    private Vector3 targetDirection;
    private float damage;

    private void Start()
    {
        // Ordena a Unity que destruya este objeto pasados 5 segundos
        Destroy(gameObject, 5f);
    }

    // Inicializa la dirección y el daño que lleva el proyectil
    public void SetDirectionAndDamage(Vector3 direction, float dmg)
    {
        targetDirection = direction;
        damage = dmg;
    //Rota  el proyectil para que mire en la dirección de viaje
        transform.rotation = Quaternion.LookRotation(direction); 
    }

    private void Update()
    {
        // Mueve el proyectil en la dirección calculada (ultima posición del Enemigo)
        transform.position += targetDirection * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
    // Verificamos si el objeto con el que chocamos es un enemigo
        if (other.CompareTag("Enemy"))
        {
    // TODO: Aquí deberías llamar al script de vida del enemigo para aplicar 'damage'
    // Ejemplo: other.GetComponent<EnemyHealth>()?.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}