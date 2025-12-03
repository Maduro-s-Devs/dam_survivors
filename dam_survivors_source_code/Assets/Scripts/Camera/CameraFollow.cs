using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("CONFIGURACIÓN AUTOMÁTICA")]
    [SerializeField] private Renderer mapRenderer;

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
    private Camera cam;
    private Controls control;
    private Plane groundPlane; // Plano matemático del suelo (Y=0)

    private void Awake() { control = new Controls(); }
    private void OnEnable() { control.Enable(); }
    private void OnDisable() { control.Disable(); }

    private void Start()
    {
        cam = GetComponent<Camera>();
        groundPlane = new Plane(Vector3.up, Vector3.zero); // Creamos un suelo infinito matemático en Y=0

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            target = playerObj.transform;
            
            // Forzamos la cámara a la posición ideal
            // Ignoramos dónde la pusiste en la escena. La ponemos en su sitio por código.
            currentOffset = defaultOffset;
            transform.position = target.position + currentOffset;
            transform.rotation = Quaternion.Euler(rotationAngleX, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (target == null || mapRenderer == null) return;

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

        // Aplicar 
        transform.position = finalPosition;
    }

    private Vector3 CorrectPositionInsideMap(Vector3 desiredPos)
    {
        // Obtenemos los límites reales del objeto mapa (Lava)
        Bounds mapBounds = mapRenderer.bounds;
        
        // Colocamos la cámara imaginariamente donde quiere ir
        transform.position = desiredPos;

        // Calculamos dónde miran los bordes de la pantalla
        float topZ = GetGroundPointFromScreen(new Vector3(0.5f, 1f, 0)).z; // Borde Superior
        float bottomZ = GetGroundPointFromScreen(new Vector3(0.5f, 0f, 0)).z; // Borde Inferior
        float rightX = GetGroundPointFromScreen(new Vector3(1f, 0.5f, 0)).x; // Borde Derecho
        float leftX = GetGroundPointFromScreen(new Vector3(0f, 0.5f, 0)).x; // Borde Izquierdo

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
        Ray ray = cam.ViewportPointToRay(viewportPoint);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}