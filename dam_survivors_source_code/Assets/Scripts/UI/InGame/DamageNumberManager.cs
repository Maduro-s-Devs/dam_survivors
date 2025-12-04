using UnityEngine;
using System.Collections;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject digitPrefab; 

    [Header("Configuración Cascada")]
    [Tooltip("Rapidez con la que salen los números (0.05 es ametralladora)")]
    public float staggerDelay = 0.06f; 
    
    [Tooltip("Separación horizontal. Para fuente tamaño 8, usa 3.5 o 4.")]
    public float spacing = 3.5f; 

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ShowDamage(float amount, Vector3 position)
    {
        string damageText = Mathf.RoundToInt(amount).ToString();
        StartCoroutine(SpawnDigitsRoutine(damageText, position));
    }

    IEnumerator SpawnDigitsRoutine(string text, Vector3 centerPos)
    {
        Vector3 rightDir = Camera.main.transform.right;
        
        // Centrar el texto
        float totalWidth = (text.Length - 1) * spacing;
        float startOffset = -totalWidth / 2f;

        // Altura de spawn
        Vector3 origin = centerPos + new Vector3(0, 2.5f, 0);

        for (int i = 0; i < text.Length; i++)
        {
            // Posición calculada
            Vector3 spawnPos = origin + (rightDir * (startOffset + (i * spacing)));

            if (digitPrefab != null)
            {
                GameObject digitObj = Instantiate(digitPrefab, spawnPos, Quaternion.identity);
                DamagePopup script = digitObj.GetComponent<DamagePopup>();
                
                if (script != null)
                {
                    // Color Amarillo/Dorado
                    script.Setup(text[i].ToString(), new Color(1f, 0.8f, 0f)); 
                }
            }

            // Destiempo
            yield return new WaitForSeconds(staggerDelay);
        }
    }
}