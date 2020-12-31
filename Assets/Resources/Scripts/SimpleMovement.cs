using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple movement controls added to the player entity that
/// </summary>
public class SimpleMovement : MonoBehaviour {
    public float Speed;
    public float MaxSpeed = 2f;
    private float MaxSpeedSquared;
    private Rigidbody _rb;

    void Start() {
        // Calculate the max speed
        MaxSpeedSquared = MaxSpeed * MaxSpeed;
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        // Get keyboard input (WASD)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Create a movement vector of the inputs and apply that as a force to the player rigidbody
        Vector3 movement = new Vector3 (horizontalInput, 0.0f, verticalInput);
        _rb.AddForce(movement * Speed, ForceMode.Impulse);

        if (_rb.velocity.sqrMagnitude > MaxSpeedSquared) {
            // Cap the velocity if it exceeds it
            _rb.velocity = _rb.velocity.normalized * MaxSpeed;
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
