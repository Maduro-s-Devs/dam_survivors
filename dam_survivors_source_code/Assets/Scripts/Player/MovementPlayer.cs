using UnityEngine;
using UnityEngine.InputSystem;

public class MovementPlayer : MonoBehaviour
{
    //////////////////////////////VARIABLES////////////////////////////////////////////
    private bool puedeMoverse = true;

    [Header("Ajustes")]
    [SerializeField] private float movementSpeed = 5f;

    [SerializeField] private float rotationSpeed = 30f; 

    private Vector2 planeDirection;

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
            
            Vector3 movementDirection = new Vector3(planeDirection.x, 0f, planeDirection.y);

            if (movementDirection.magnitude > 1f)
                movementDirection.Normalize();

            //MOVER
            transform.position += movementDirection * movementSpeed * Time.deltaTime;

            //ROTAR (SUAVIZADO)
            if (movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }  
    }
}