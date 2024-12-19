using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private InputActionReference zoom;
    [SerializeField] private InputActionReference rotate;
    [SerializeField] private InputActionReference look;

    
    private float zoomDelta;
    private float mouseX;
    private bool isMouseWheelPressed;

    private float rotationSpeed = 0.5f;
    private float zoomSpeed = 3f;
    private float minZoom = 2f;
    private float maxZoom = 30f;
    private float currentZoom = 5f;

    private void Awake()
    {
        // Bind to actions
        zoom.action.performed += x => zoomDelta = x.ReadValue<float>();
        look.action.performed += x =>
        {
            if (isMouseWheelPressed) // Only update mouseX if middle mouse is pressed
                mouseX = x.ReadValue<float>();
        };
        rotate.action.started += _ => isMouseWheelPressed = true;
        rotate.action.canceled += _ => {
            isMouseWheelPressed = false;
            mouseX = 0f; // Reset mouseX to stop rotation when released
        };

    }

    private void OnEnable()
    {
        zoom.action.Enable();
        rotate.action.Enable();
        look.action.Enable();
    }

    private void OnDisable()
    {
        zoom.action.Disable();
        rotate.action.Disable();
        look.action.Disable();
    }

    void Update()
    {
        if (target == null) return;

        HandleRotation();
        HandleZoom();

        Vector3 desiredPosition = target.position - transform.forward * currentZoom;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * zoomSpeed);
    }

    void HandleRotation()
    {
        if (isMouseWheelPressed)
        {
            // Rotate the camera based on mouse X input
            float rotationAngle = mouseX * rotationSpeed;
            transform.RotateAround(target.position, Vector3.up, rotationAngle);
        }
    }

    void HandleZoom()
    {
        if (zoomDelta != 0)
        {
            currentZoom -= zoomDelta * zoomSpeed * Time.deltaTime;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            zoomDelta = 0;
        }
    }
}