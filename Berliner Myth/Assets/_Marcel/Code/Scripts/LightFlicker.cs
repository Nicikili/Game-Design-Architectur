using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightComponent;

    // Variables to control the flicker effect
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 1.5f; // Maximum light intensity
    public float flickerSpeed = 0.1f; // Speed of the flickering effect

    // Use this for initialization
    void Start()
    {
        // Get the Light component attached to this GameObject
        lightComponent = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // Randomly change the light intensity to create a flickering effect
        lightComponent.intensity = Random.Range(minIntensity, maxIntensity);

        // Optionally, you can also randomly toggle the light on and off
        if (Random.value > 0.9f) // 10% chance to toggle the light on/off each frame
        {
            lightComponent.enabled = !lightComponent.enabled;
        }

        // Use a coroutine to control the flicker speed
        StartCoroutine(FlickerDelay());
    }

    private System.Collections.IEnumerator FlickerDelay()
    {
        yield return new WaitForSeconds(flickerSpeed);
    }
}

