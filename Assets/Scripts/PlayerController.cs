using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera playerCamera;
    public WeaponController weapon;

    public float walkSpeed = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Mouse Parameters")]
    [SerializeField] public float mouseSensitivity = 0.2f;

    [Header("Jump Parameters")]
    public float jumpForce = 5.0f;
    public float gravity = 9.8f;

    Vector3 moveDirection = Vector3.zero;
    public bool canMove = true;

    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 currentMovement;

    private float verticalRotation;
    private float upDownRange = 80.0f;

    private bool isMoving = false;
    private bool isDashing = false;

    [Header("Dash Parameters")]
    public float dashForce = 10f;
    public float dashLength = .2f;



    //public float dashMultiplier = 5f;



    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Sprint");

        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;

        dashAction.performed += _ => StartCoroutine(DashCouroutine());
        dashAction.canceled += _ => isDashing = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }



    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();

    }



    void HandleMovement()
    {
        float verticalSpeed = moveInput.y * walkSpeed;
        float horizontalSpeed = moveInput.x * walkSpeed;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        HandleGravityAndJumping();
        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = moveInput.y != 0 || moveInput.x != 0;
    }

    private IEnumerator DashCouroutine()
    {
        if (isMoving && !isDashing) {
            print("DASHING");
            isDashing = true;
            float startTime = Time.time;
            float verticalSpeed = moveInput.y * dashForce;
            float horizontalSpeed = moveInput.x * dashForce;


            Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
            horizontalMovement = transform.rotation * horizontalMovement;

            currentMovement.x = horizontalMovement.x;
            currentMovement.z = horizontalMovement.z;

            while(Time.time < startTime + dashLength)
            {
                characterController.Move(currentMovement * Time.deltaTime);
                yield return null;
            }
        }
    }

    void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (jumpAction.triggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        //Debug.Log("verticalRotation: " + verticalRotation + "\nmouseXRotation: " + mouseXRotation + "\nlookInput.x: " + lookInput.x);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
