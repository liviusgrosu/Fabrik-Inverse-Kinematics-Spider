using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple movement controls added to the player entity that
/// </summary>
public class SimpleMovement : MonoBehaviour {
    public float MovementSpeed;
    public float MaxMovementSpeed = 2f;
    public float TurningSpeed;
    private float MaxMovementSpeedSquared;
    private Rigidbody _rb;

    void Start() {
        // Calculate the max speed
        MaxMovementSpeedSquared = MaxMovementSpeed * MaxMovementSpeed;
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        
        // Movement input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        // Rotation input
        float turningInput = Input.GetAxisRaw("Turn");
        transform.RotateAround(transform.position, Vector3.up, turningInput * TurningSpeed * Time.deltaTime);

        // Create a movement vector of the inputs and apply that as a force to the player rigidbody
        Vector3 movement = new Vector3 (horizontalInput, 0.0f, verticalInput);
        // Move relative to the rotaton of the player
        Vector3 velocity = transform.rotation * movement;
        _rb.AddForce(velocity * MovementSpeed, ForceMode.Impulse);

        if (_rb.velocity.sqrMagnitude > MaxMovementSpeedSquared) {
            // Cap the velocity if it exceeds it
            _rb.velocity = _rb.velocity.normalized * MaxMovementSpeed;
        }

        if (horizontalInput == 0 && verticalInput == 0) {
            // Halt the velocity if no keyboard input is present
            _rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Get the current player velocity
    /// </summary>
    /// <returns>Current player direction velocity</returns>
    public Vector3 GetCurrentVelocity() {
        return _rb.velocity;
    }
}
