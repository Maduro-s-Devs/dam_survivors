using UnityEngine;
using UnityEngine.UI; // Necesario para manipular la UI
using System.Collections.Generic; // Para usar Listas

[RequireComponent(typeof(Image))] // Esto asegura que el objeto tenga una Imagen
public class RandomImageDisplay : MonoBehaviour
{
    [Header("Colección de Imágenes")]
    [Tooltip("Arrastra aquí todas las imágenes que quieras que salgan aleatoriamente")]
    public List<Sprite> imageList;

    [Header("Configuración")]
    [Tooltip("Si es True, la imagen mantendrá sus proporciones originales (no se estirará)")]
    public bool preserveAspect = true;

    private Image targetImage;

    void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    void Start()
    {
        // 1. Comprobamos que la lista no esté vacía para evitar errores
        if (imageList.Count > 0)
        {
            // 2. Elegimos un número aleatorio entre 0 y el total de imágenes
            int randomIndex = Random.Range(0, imageList.Count);

            // 3. Asignamos la imagen ganadora
            targetImage.sprite = imageList[randomIndex];

            // 4. Ajuste opcional para que no se deforme
            targetImage.preserveAspect = preserveAspect;

            // Si quieres que recupere su tamaño real en píxeles, descomenta esta línea:
            // targetImage.SetNativeSize(); 
        }
        else
        {
            Debug.LogWarning("⚠️ La lista de imágenes aleatorias está vacía.");
        }
    }
}