using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test3 : MonoBehaviour
{
    public float swerveSpeed = 5f;  // Speed of swerving movement
    public float maxSwerveAmount = 2f;  // Maximum amount the object can swerve
    public float minX = -3f;  // Minimum x-position
    public float maxX = 3f;   // Maximum x-position

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody2D component attached to the GameObject
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on this GameObject.");
        }
    }

    void Update()
    {
        // Get mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate swerve amount based on mouse position
        float swerveAmount = Mathf.Clamp(mousePos.x - transform.position.x, -maxSwerveAmount, maxSwerveAmount);

        // Calculate the new x-position
        float newX = Mathf.Clamp(transform.position.x + swerveAmount * swerveSpeed * Time.deltaTime, minX, maxX);

        // Apply swerve to the object's position, considering the x-axis limits
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
        rb.MovePosition(newPosition);
    }
}
