using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections; // Necesario para Corutinas

public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ESTRUCTURA (Arrastra el HIJO 'Visuals')")]
    public RectTransform visualsContainer; 
    public CanvasGroup visualsCanvasGroup;

    [Header("EFECTO REVELACIÓN (Arrastra la imagen blanca)")]
    public CanvasGroup flashOverlay; 

    [Header("CONTENIDO")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI levelText;
    public Button cardButton;

    [Header("ANIMACIÓN CAÍDA")]
    public float fallDuration = 0.7f; 
    public float startHeight = 1000f; 
    public AnimationCurve entryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("ANIMACIÓN HOVER")]
    public float hoverScale = 1.15f;
    public float hoverSpeed = 15f;

    [Header("ANIMACIÓN CLICK (Selección)")]
    public float clickDuration = 0.25f; // Tiempo que tarda la animación antes de cerrar
    public float clickScale = 1.25f;    // Tamaño máximo al clicar
    public AnimationCurve clickCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 1.2f), new Keyframe(1, 0)); 
    // La curva va de 1 (Normal) -> 1.2 (Grande) -> 0 (Desaparece)

    // Variables internas
    private WeaponData myData;
    private LevelUpManager manager;
    private bool isInteractable = false;
    private bool isClicked = false; // Para evitar doble clic
    private Vector3 targetScale;
    
    // Variables de control de animación
    private bool isAnimatingEntry = false;
    private float animationTimer = 0f;
    private float startDelay = 0f;
    private float delayTimer = 0f;

    public void Setup(BaseLauncher launcher, LevelUpManager mngr, float delay)
    {
        manager = mngr;
        myData = launcher.weaponData;
        startDelay = delay;
        
        FillData(launcher);

        if(visualsContainer == this.GetComponent<RectTransform>())
        {
            return;
        }

        // Estado inicial
        if (visualsCanvasGroup != null) visualsCanvasGroup.alpha = (startDelay > 0) ? 0f : 1f;
        if (flashOverlay != null) flashOverlay.alpha = 1f; 
        
        if (visualsContainer != null)
        {
            visualsContainer.anchoredPosition = new Vector2(0, startHeight);
            visualsContainer.localScale = Vector3.one;
        }
        
        targetScale = Vector3.one;
        isAnimatingEntry = true;
        animationTimer = 0f;
        delayTimer = 0f;
        isClicked = false; // Resetear estado de clic
        
        isInteractable = false;
        if(cardButton != null) cardButton.interactable = false;
    }

    void Update()
    {
        // Si ya hemos clicado, el Update deja de controlar la escala para no interferir
        if (isClicked) return; 

        if (isAnimatingEntry)
        {
            if (visualsContainer == null) return;

            // FASE 0: CASCADA
            if (delayTimer < startDelay)
            {
                delayTimer += Time.unscaledDeltaTime;
                return; 
            }
            if (visualsCanvasGroup != null) visualsCanvasGroup.alpha = 1f;

            // FASE 1: CAÍDA
            animationTimer += Time.unscaledDeltaTime;
            
            float t = Mathf.Clamp01(animationTimer / fallDuration);
            float curveT = entryCurve.Evaluate(t);

            float currentY = Mathf.LerpUnclamped(startHeight, 0f, curveT);
            visualsContainer.anchoredPosition = new Vector2(0, currentY);
            
            if (flashOverlay != null) flashOverlay.alpha = 1f - t;

            if (animationTimer >= fallDuration)
            {
                isAnimatingEntry = false;
                visualsContainer.anchoredPosition = Vector2.zero;
                if (flashOverlay != null) flashOverlay.alpha = 0f;
                
                isInteractable = true;
                if(cardButton != null) cardButton.interactable = true;
            }
        }
        else
        {
            // FASE 2: HOVER
            if (visualsContainer != null)
            {
                visualsContainer.localScale = Vector3.Lerp(visualsContainer.localScale, targetScale, Time.unscaledDeltaTime * hoverSpeed);
            }
        }
    }

    // --- AQUÍ ESTÁ LA NUEVA ANIMACIÓN DE CLICK ---
    IEnumerator AnimateClick()
    {
        isClicked = true; // Bloqueamos todo lo demás
        isInteractable = false; // Ya no detecta hover

        float timer = 0f;
        Vector3 initialScale = visualsContainer.localScale; // Empezamos desde el tamaño actual (quizás estaba grande por el hover)

        // Hacemos un flash blanco rápido al clicar
        if(flashOverlay != null) flashOverlay.alpha = 0.5f; 

        while (timer < clickDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / clickDuration);
            
            // Usamos la curva para hacer el efecto de golpe
            // Evaluate(t) nos dará valores como 1.2, 1.3... y al final 0 si queremos que desaparezca
            float scaleMultiplier = clickCurve.Evaluate(t);
            
            visualsContainer.localScale = Vector3.one * scaleMultiplier;

            // Desvanecemos el flash blanco rápidamente
            if(flashOverlay != null) flashOverlay.alpha = Mathf.Lerp(0.5f, 0f, t * 2);

            yield return null;
        }

        // --- MOMENTO DE LA VERDAD ---
        // Llamamos al Manager para aplicar la mejora y cerrar
        if (manager != null) manager.SelectUpgrade(myData);
    }

    // --- RELLENAR DATOS ---
    void FillData(BaseLauncher launcher)
    {
        if(myData == null) return;
        if(iconImage != null && myData.icon != null) iconImage.sprite = myData.icon;
        if(nameText != null) nameText.text = myData.weaponName;

        // Lógica de textos
        if (launcher.isUnlocked && launcher.level == 9)
        {
            // EVOLUCIÓN
            if(myData.evolvedIcon != null && iconImage != null) iconImage.sprite = myData.evolvedIcon;
            if(nameText != null) nameText.text = myData.evolvedName;
            if(descText != null) descText.text = myData.evolvedDescription;
            if(levelText != null) { levelText.text = "¡EVOLVE!"; levelText.color = Color.magenta; }
        }
        else if (!launcher.isUnlocked)
        {
            if(levelText != null) { levelText.text = "¡NEW!"; levelText.color = Color.yellow; }
            
            if(descText != null) 
            {
                if(myData.levelUpDescriptions.Count > 0) 
                    descText.text = myData.levelUpDescriptions[0]; // ELEMENTO 0 AQUÍ
                else 
                    descText.text = "Desbloquea este arma.";
            }
        }
        else
        {
            if(levelText != null) { levelText.text = $"LEVEL {launcher.level} -> {launcher.level + 1}"; levelText.color = Color.cyan; }
            
            if(descText != null)
            {
                int idx = launcher.level; 

                // Verificamos que la lista tenga ese elemento para no dar error
                if (myData.levelUpDescriptions != null && idx < myData.levelUpDescriptions.Count && idx >= 0)
                    descText.text = myData.levelUpDescriptions[idx];
                else
                    descText.text = "Mejora estadísticas.";
            }
        }

        if(cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnClickCard);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { if (isInteractable && !isClicked) targetScale = Vector3.one * hoverScale; }
    public void OnPointerExit(PointerEventData eventData) { if (isInteractable && !isClicked) targetScale = Vector3.one; }
    
    void OnClickCard() 
    { 
        // En lugar de llamar al manager directamente, iniciamos la animación
        if (isInteractable && !isClicked) StartCoroutine(AnimateClick()); 
    }
}