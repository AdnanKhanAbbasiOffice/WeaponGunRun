using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
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
        float swerveInput = 0f;

#if UNITY_STANDALONE || UNITY_EDITOR
        // Desktop input using mouse
        swerveInput = Input.GetAxis("Mouse X");
#elif UNITY_ANDROID || UNITY_IOS
        // Mobile input using touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            swerveInput = Mathf.Clamp(touchPos.x - transform.position.x, -1f, 1f);
        }
#endif

        // Calculate swerve amount based on input
        float swerveAmount = Mathf.Clamp(swerveInput * swerveSpeed * Time.deltaTime, -maxSwerveAmount, maxSwerveAmount);

        // Calculate the new x-position
        float newX = Mathf.Clamp(transform.position.x + swerveAmount, minX, maxX);

        // Apply swerve to the object's position, considering the x-axis limits
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
        rb.MovePosition(newPosition);
    }
}
