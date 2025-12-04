using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("CONFIGURACIÓN AUTOMÁTICA")]
    [SerializeField] private Renderer mapRenderer;
    [SerializeField] private Camera referenceCamera; // <--- NUEVO: Para asignar la cámara hija

    [Header("Ajustes de Vista (Centrado)")]
    // Estos valores definen el ángulo y distancia ideal.
    // X=0 asegura que el jugador esté centrado horizontalmente.
    [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 18f, -12f); 
    [SerializeField] private float rotationAngleX = 60f; // Ángulo de picado

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 2f; 
    [SerializeField] private float minZoom = 5f; 
    [SerializeField] private float maxZoom = 25f;   

    private Transform target; 
    private Vector3 currentOffset;
    
    // Ya no usamos "cam" local, usamos "referenceCamera"
    private Controls control;
    private Plane groundPlane; // Plano matemático del suelo (Y=0)

    private void Awake() { control = new Controls(); }
    private void OnEnable() { control.Enable(); }
    private void OnDisable() { control.Disable(); }

    private void Start()
    {
        // 1. INTENTO DE AUTODETECCIÓN
        // Si no has arrastrado la cámara al inspector, la buscamos en los hijos
        if (referenceCamera == null)
        {
            referenceCamera = GetComponentInChildren<Camera>();
        }

        // Si sigue sin encontrarla, error grave
        if (referenceCamera == null)
        {
            Debug.LogError("🛑 ERROR CRÍTICO: El script CameraFollow (en CameraHolder) no encuentra ninguna Cámara en sus hijos.");
            return;
        }

        groundPlane = new Plane(Vector3.up, Vector3.zero); // Creamos un suelo infinito matemático en Y=0

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            target = playerObj.transform;
            
            // Forzamos la posición ideal
            currentOffset = defaultOffset;
            
            // Movemos el HOLDER (Padre)
            transform.position = target.position + currentOffset;
            
            // Rotamos el HOLDER (Padre)
            // Importante: La cámara hija debe tener rotación 0,0,0 para que herede esto
            transform.rotation = Quaternion.Euler(rotationAngleX, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (target == null || mapRenderer == null || referenceCamera == null) return;

        // --- 1. Lógica de Zoom ---
        float scrollInput = control.Player.Zoom.ReadValue<float>();
        if (scrollInput != 0f)
        {
            Vector3 direction = currentOffset.normalized;
            float dist = currentOffset.magnitude;
            dist -= (scrollInput > 0 ? 1f : -1f) * zoomSpeed;
            dist = Mathf.Clamp(dist, minZoom, maxZoom);
            currentOffset = direction * dist;
        }

        // Posición Ideal (Siguiendo al jugador)
        Vector3 finalPosition = target.position + currentOffset;
        
        // Corregimos la posición si se sale del mapa.
        finalPosition = CorrectPositionInsideMap(finalPosition);

        // Aplicar al PADRE (CameraHolder)
        transform.position = finalPosition;
    }

    private Vector3 CorrectPositionInsideMap(Vector3 desiredPos)
    {
        // Obtenemos los límites reales del objeto mapa (Lava)
        Bounds mapBounds = mapRenderer.bounds;
        
        // Mover temporalmente el transform para hacer el cálculo del rayo
        Vector3 originalPos = transform.position;
        transform.position = desiredPos;

        // Calculamos dónde miran los bordes de la pantalla usando la CÁMARA HIJA
        float topZ = GetGroundPointFromScreen(new Vector3(0.5f, 1f, 0)).z; 
        float bottomZ = GetGroundPointFromScreen(new Vector3(0.5f, 0f, 0)).z; 
        float rightX = GetGroundPointFromScreen(new Vector3(1f, 0.5f, 0)).x; 
        float leftX = GetGroundPointFromScreen(new Vector3(0f, 0.5f, 0)).x; 

        // Restaurar posición para no romper nada si era inválida
        transform.position = originalPos;

        // Corregimos Z (Arriba/Abajo)
        float correctionZ = 0f;
        if (topZ > mapBounds.max.z) correctionZ = mapBounds.max.z - topZ;
        if (bottomZ < mapBounds.min.z) correctionZ = mapBounds.min.z - bottomZ;

        // Corregimos X (Izquierda/Derecha)
        float correctionX = 0f;
        if (rightX > mapBounds.max.x) correctionX = mapBounds.max.x - rightX;
        if (leftX < mapBounds.min.x) correctionX = mapBounds.min.x - leftX;

        // Devolvemos la posición corregida
        return desiredPos + new Vector3(correctionX, 0, correctionZ);
    }

    // Convierte un punto de la pantalla (0-1) a coordenada del mundo en el suelo
    private Vector3 GetGroundPointFromScreen(Vector3 viewportPoint)
    {
        // Usamos la cámara hija para lanzar el rayo
        Ray ray = referenceCamera.ViewportPointToRay(viewportPoint);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}