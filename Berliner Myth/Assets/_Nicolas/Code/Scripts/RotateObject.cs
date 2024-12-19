using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 5f; // Time to complete 360° rotation

    private float rotationSpeed;

    private void Start()
    {
        // Calculate rotation speed (degrees per second)
        rotationSpeed = 360f / rotationDuration;
    }

    private void Update()
    {
        // Rotate the object around its Y-axis
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}