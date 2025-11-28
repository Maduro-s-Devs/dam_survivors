// este script lo lleva el player
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementPlayer : MonoBehaviour
{
    //////////////////////////////VARIABLES////////////////////////////////////////////
    private bool puedeMoverse = true;

    [Header("settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float acceleration = 12f; // Qué tan rápido arranca
    [SerializeField] private float deceleration = 12f;  // Qué tan rápido frena 

    [SerializeField] private float rotationSpeed = 30f; 

    private Vector2 planeDirection;
    private float currentSpeed = 0f; // Velocidad actual
    private Vector3 lastMoveDirection; // Recordar la dirección al frenar

    private Controls control; 

    //////////////////////////////FUNCIONES UNITY//////////////////////////////////////
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (puedeMoverse)
        {
            // LEER INPUT
            planeDirection = control.Player.Move.ReadValue<Vector2>();
            
            // Calculamos la dirección del Input (lo que pulsa el jugador)
            Vector3 inputDirection = new Vector3(planeDirection.x, 0f, planeDirection.y);

            if (inputDirection.magnitude > 1f)
                inputDirection.Normalize();

            // (ACELERAR Y FRENAR)
            if (inputDirection.magnitude >= 0.1f)
            {
                // El jugador está pulsando teclas, guardamos esta dirección como la última conocida
                lastMoveDirection = inputDirection;

                // Aceleramos hacia la velocidad máxima
                currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                // El jugador soltó las tecla, por lo que mantenemos 'lastMoveDirection'
                // 2. Desaceleramos la velocidad hacia 0
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            }

            // MOVER
            // Usamos 'lastMoveDirection' (que recuerda el rumbo) y 'currentSpeed' (que sube y baja )
            transform.position += lastMoveDirection * currentSpeed * Time.deltaTime;

            // ROTAR (SUAVIZADO)
            // Rotamos hacia donde nos estamos moviendo realmente
            if (lastMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }  
    }
}