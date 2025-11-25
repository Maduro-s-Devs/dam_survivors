// Este script lo usa la camara
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    private Transform target; 

    [Header("Offset Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f); 

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f; 
    [SerializeField] private float minZoom = 5f; 
    [SerializeField] private float maxZoom = 30f;   

    private Controls control;

    private void Awake()
    {
        control = new Controls();
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }

    private void Start()
    {
        // Buscar al objeto con Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }

        // Si se encontró al Player, calculamos el offset inicial si hace falta
        if (target != null && offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        //Leer el Input
        float scrollInput = control.Player.Zoom.ReadValue<float>();

        if (scrollInput != 0f)
        {
            // 'currentDistance' es cuán lejos está ahora mismo
            Vector3 direction = offset.normalized;
            float currentDistance = offset.magnitude;

            // Convertimos el scroll (120 o -120) en 1 o -1 para que sea suave
            float zoomDirection = scrollInput > 0 ? 1f : -1f;
            
            // Restamos para acercar (Zoom In)
            currentDistance -= zoomDirection * zoomSpeed;

            // Es matemáticamente imposible que baje de minZoom.
            currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);
            // Multiplicamos la dirección original por la nueva distancia segura
            offset = direction * currentDistance;
        }
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}