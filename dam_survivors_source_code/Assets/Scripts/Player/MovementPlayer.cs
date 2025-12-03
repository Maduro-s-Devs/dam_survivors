using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovementPlayer : MonoBehaviour
{
    //////////////////////////////VARIABLES////////////////////////////////////////////
    private bool puedeMoverse = true;

    [Header("Ajustes de Movimiento")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float acceleration = 12f; 
    [SerializeField] private float deceleration = 12f;  
    [SerializeField] private float rotationSpeed = 30f; 

    [Header("Ajustes de Gravedad")]
    [SerializeField] private float gravity = -20f; // Fuerza hacia abajo (fuerte para que no flote en cuestas)
    private float verticalVelocity; // Velocidad de caída acumulada

    private Vector2 planeDirection;
    private float currentSpeed = 0f; 
    private Vector3 lastMoveDirection; 

    private Controls control;
    private CharacterController characterController;

    //////////////////////////////FUNCIONES UNITY//////////////////////////////////////
    private void Awake()
    {
        control = new Controls();
        characterController = GetComponent<CharacterController>();
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
            Vector3 inputDirection = new Vector3(planeDirection.x, 0f, planeDirection.y);

            if (inputDirection.magnitude > 1f)
                inputDirection.Normalize();

            // ACELERAR Y FRENAR (Horizontal)
            if (inputDirection.magnitude >= 0.1f)
            {
                lastMoveDirection = inputDirection;
                currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            }

            // isGrounded es una propiedad mágica del CharacterController que nos dice si tocamos suelo
            if (characterController.isGrounded && verticalVelocity < 0)
            {
                // Si estamos en el suelo, reseteamos la caída a un valor pequeño negativo
                // para mantenerlo "pegado" al suelo (si pones 0, a veces rebota en bajadas)
                verticalVelocity = -5f; 
            }

            // Acumulamos gravedad constantemente
            verticalVelocity += gravity * Time.deltaTime;

            // Combinamos Horizontal + Vertical
            Vector3 horizontalMove = lastMoveDirection * currentSpeed;
            Vector3 verticalMove = Vector3.up * verticalVelocity;

            // Movemos el personaje sumando ambas fuerzas
            characterController.Move((horizontalMove + verticalMove) * Time.deltaTime);

            // ROTAR
            if (lastMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }  
    }
}