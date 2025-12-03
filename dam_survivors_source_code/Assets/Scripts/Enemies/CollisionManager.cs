//este script lo llevan los objetos atravesables por enemigos con tag Enemy y AntiPlayer
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private void Start()
    {
        // Buscamos los IDs de las capas por su nombre
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int antiPlayerLayer = LayerMask.NameToLayer("AntiPlayer");

        // Comprobamos que las capas existan
        if (enemyLayer == -1 || antiPlayerLayer == -1)
        {
            Debug.LogError("¡ERROR! Faltan las capas 'Enemy' o 'AntiPlayer'. Créalas en el menú Layers.");
            return;
        }

        // ORDEN SUPREMA: "Vosotros dos, ignoraros"
        // true = Ignorar colisión / false = Chocar
        Physics.IgnoreLayerCollision(enemyLayer, antiPlayerLayer, true);
        
        Debug.Log("Física configurada: Los Enemigos ahora atraviesan los muros AntiPlayer.");
    }
}