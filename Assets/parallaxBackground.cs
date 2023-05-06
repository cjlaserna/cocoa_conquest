using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxBackground : MonoBehaviour
{
    public float parallaxScale = 1f;    // The proportion of the camera's movement to move the background by.
    public float smoothing = 1f;        // How smooth the parallax effect should be.

    private Transform cam;              // Reference to the main camera's transform.
    private Vector3 previousCamPos;     // The position of the camera in the previous frame.
    private float backgroundWidth;      // The width of the background sprite.
    private Vector3 startPosition;      // The starting position of the background.

    void Awake()
    {
        // Set up the references.
        cam = Camera.main.transform;

        // Get the width of the background sprite.
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Start()
    {
        // The previous frame had the current frame's camera position.
        previousCamPos = cam.position;
        startPosition = transform.position;
    }

    void Update()
    {
        // The parallax is the opposite of the camera movement because the previous frame multiplied by the scale.
        float parallax = (previousCamPos.x - cam.position.x) * parallaxScale;

        // Set a target x position which is the current position plus the parallax.
        float backgroundTargetPosX = transform.position.x + parallax;

        // Create a target position which is the background's current position with its target x position.
        Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, transform.position.y, transform.position.z);

        // Fade between the current position and the target position using lerp.
        transform.position = Vector3.Lerp(transform.position, backgroundTargetPos, smoothing * Time.deltaTime);

        // Check if the background has completely scrolled past its starting point and move it to the end of the loop if necessary.
        if (cam.position.x < transform.position.x - Mathf.Floor(backgroundWidth / 4f))
        {
            transform.position = new Vector3(transform.position.x - Mathf.Floor(backgroundWidth / 2), transform.position.y, transform.position.z);
        }
        else if (cam.position.x > transform.position.x + Mathf.Floor(backgroundWidth / 4f))
        {
            transform.position = new Vector3(transform.position.x + Mathf.Floor(backgroundWidth / 2), transform.position.y, transform.position.z);
        }


        // Set the previousCamPos to the camera's position at the end of this frame.
        previousCamPos = cam.position;
    }
}
