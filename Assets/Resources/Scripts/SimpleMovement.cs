using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed;
    public float maxSpeed = 2f;
    private float maxSpeedSquared;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        maxSpeedSquared = maxSpeed * maxSpeed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get keyboard input (WASD)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3 (horizontalInput, 0.0f, verticalInput);
        rb.AddForce(movement * speed, ForceMode.Impulse);

        if (rb.velocity.sqrMagnitude > maxSpeedSquared)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if (horizontalInput == 0 && verticalInput == 0)
        {
            rb.velocity = Vector3.zero;
        }

    }

    public Vector3 GetCurrentVelocity()
    {
        return rb.velocity;
    }
}
