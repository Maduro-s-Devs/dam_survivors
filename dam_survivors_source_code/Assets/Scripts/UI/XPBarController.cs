using UnityEngine;
using UnityEngine.UI;
using TMPro; // <--- NUEVO: Necesario para usar Textos

public class XPBarController : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image xpFillImage; 
    public TextMeshProUGUI levelText; // <--- NUEVO: Arrastra aquí tu texto "lvl number"

    [Header("Configuración Visual")]
    public float smoothSpeed = 5f;
    [Range(0f, 1f)] public float maxVisualFill = 0.5f; 

    private float targetFillAmount = 0f;

    void Update()
    {
        if (xpFillImage == null) return;

        if (Mathf.Abs(xpFillImage.fillAmount - targetFillAmount) > 0.001f)
        {
            xpFillImage.fillAmount = Mathf.Lerp(xpFillImage.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }

    public void UpdateXP(float currentXP, float requiredXP)
    {
        float ratio = currentXP / requiredXP;
        ratio = Mathf.Clamp01(ratio);
        targetFillAmount = ratio * maxVisualFill;
    }

    public void OnLevelUp()
    {
        if(xpFillImage != null) xpFillImage.fillAmount = 0f;
        targetFillAmount = 0f;
    }

    // --- NUEVA FUNCIÓN PARA ACTUALIZAR EL TEXTO ---
    public void UpdateLevelText(int newLevel)
    {
        if (levelText != null)
        {
            // Puedes cambiar el formato aquí. Ej: "LVL 5" o solo "5"
            levelText.text = newLevel.ToString(); 
        }
    }
}