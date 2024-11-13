using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference zoom;
    [SerializeField] private InputActionReference rotate;
    [SerializeField] private InputActionReference look;

    [SerializeField] Transform target;

    private float mouseScrollY;
    private float mouseX;
    private bool isMouseWheelPressed;

    private float rotationSpeed = 0.5f;
    private float zoomSpeed = 5f;
    private float minZoom = 2f;
    private float maxZoom = 10f;
    private float currentZoom = 5f;

    private void OnEnable()
    {
        zoom.action.performed += OnZoom;
        rotate.action.performed += OnRotate;
        look.action.started += OnMiddleMouseHold;
        look.action.canceled += OnMiddleMouseRelease;

        zoom.action.Enable();
        rotate.action.Enable();
        look.action.Enable();
    }

    private void OnDisable()
    {
        zoom.action.performed -= OnZoom;
        rotate.action.performed -= OnRotate;
        look.action.started -= OnMiddleMouseHold;
        look.action.canceled -= OnMiddleMouseRelease;

        zoom.action.Disable();
        rotate.action.Disable();
        look.action.Disable();
    }

    private void Update()
    {
        if (target == null) return;

        HandleRotation();
        HandleZoom();

        Vector3 desiredPosition = target.position - transform.forward * currentZoom;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * zoomSpeed);
    }

    private void OnZoom(InputAction.CallbackContext ctx)
    {
        mouseScrollY = ctx.ReadValue<float>();
    }

    private void OnRotate(InputAction.CallbackContext ctx)
    {
        mouseX = ctx.ReadValue<float>();
    }

    private void OnMiddleMouseHold(InputAction.CallbackContext ctx)
    {
        isMouseWheelPressed = true;
    }

    private void OnMiddleMouseRelease(InputAction.CallbackContext ctx)
    {
        isMouseWheelPressed = false;
    }

    private void HandleRotation()
    {
        if (isMouseWheelPressed)
        {
            float rotationAngle = mouseX * rotationSpeed;
            transform.RotateAround(target.position, Vector3.up, rotationAngle);
        }
    }

    private void HandleZoom()
    {
        currentZoom -= mouseScrollY * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }
}