using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private InputActionReference movementInputAction;
    [SerializeField] private CinemachineVirtualCamera cam;
    
    private Controls controls;

    private Rigidbody rb;

    private Vector2 movementVector;

    private void OnEnable()
    {
        controls = new Controls();
        
        controls.Enable();
        controls.Player.Movement.performed += OnMove;
        controls.Player.Movement.canceled += OnMove;
        // movementInputAction.action.performed += OnMove;
        // movementInputAction.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.performed -= OnMove;
        controls.Player.Movement.canceled -= OnMove;
        
        // movementInputAction.action.performed -= OnMove;
        // movementInputAction.action.canceled -= OnMove;
    }

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
        Debug.Log(movementVector);
    }

    private void FixedUpdate()
    {
        Vector3 movementSpeed = new Vector3(movementVector.x * speed, 0, movementVector.y * speed);
        Vector3 direction = transform.TransformDirection(movementSpeed);
        rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);
        
        float rotation = cam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        Debug.Log(rotation);
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void Update()
    {

    }
}
