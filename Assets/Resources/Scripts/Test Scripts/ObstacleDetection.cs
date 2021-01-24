using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetection : MonoBehaviour
{
    public bool EnableWallDetection, EnableLedgeDetection;
    private float _initialHeight;
    private RaycastHit _surfaceRay;
    private const float moveWallAmount = 0.5f;
    private const float moveLedgeAmount = 0.25f;
    private Vector3 _currentSurfaceNormal;

    private void Awake()
    {
        if(Physics.Raycast(transform.position, -transform.up, out _surfaceRay, Mathf.Infinity)) {
            _initialHeight = (transform.position - _surfaceRay.point).magnitude;
        }
    }

    // Update is called once per frame
    public void Calculate()
    {
        if(Physics.Raycast(transform.position, -transform.up, out _surfaceRay, Mathf.Infinity)) {
            // Get the current height difference from the initial height
            float currHeight = (transform.position - _surfaceRay.point).magnitude;
            float heightDiff = _initialHeight - currHeight;
            
            _currentSurfaceNormal = _surfaceRay.normal;

            if (Mathf.Abs(heightDiff) > 0.05f) {
                // Translate back to the original height if there is a discrepancy
                Vector3 pos = _surfaceRay.point + _surfaceRay.normal.normalized * _initialHeight;
                transform.position = pos;
            }
            // Rotate the body to match that of the surface
            transform.rotation = Quaternion.FromToRotation(transform.up, _surfaceRay.normal) * transform.rotation;
        }

        // Unfortunately, due to local variables not correlating with objects direction vectors at all times, this is the only way to check for all 4 XZ sides of a transform
        // --- Wall Detections ---
        // Forwards
        if(EnableWallDetection) {
            if (WallDetectionCheck(transform.forward, 1f)) {
                TranslatePositionSlightly(transform.forward, moveWallAmount);
            }

            // Backwards
            if (WallDetectionCheck(-transform.forward, 1f)) {
                TranslatePositionSlightly(-transform.forward, moveWallAmount);
            }

            // Rightwards
            if (WallDetectionCheck(transform.right, 1f)) {
                TranslatePositionSlightly(transform.right, moveWallAmount);
            }

            // Leftwards
            if (WallDetectionCheck(-transform.right, 1f)) {
                TranslatePositionSlightly(-transform.right, moveWallAmount);
            }
        }
        // --- Ledge Detections ---
        // Forwards
        if(EnableLedgeDetection) {
            if (LedgeDetectionCheck(transform.forward, 0.5f)) { 
                TranslatePositionSlightly(transform.forward, moveLedgeAmount);
            }

            // Backwards
            if (LedgeDetectionCheck(-transform.forward, 0.5f)) { 
                TranslatePositionSlightly(-transform.forward, moveLedgeAmount);
            }

            // Rightwards
            if (LedgeDetectionCheck(transform.right, 0.5f)) { 
                TranslatePositionSlightly(transform.right, moveLedgeAmount);
            }

            // Leftwards
            if (LedgeDetectionCheck(-transform.right, 0.5f)) { 
                TranslatePositionSlightly(-transform.right, moveLedgeAmount);
            }
        }
    }

    private bool WallDetectionCheck(Vector3 direction, float distance) {
        if(Physics.Raycast(transform.position, direction, out _surfaceRay, distance)) {
            Vector3 pos = _surfaceRay.point + _surfaceRay.normal.normalized * _initialHeight;
            transform.position = pos;
            transform.rotation = Quaternion.FromToRotation(transform.up, _surfaceRay.normal) * transform.rotation;
            return true;
        }
        return false;
    }

    private bool LedgeDetectionCheck(Vector3 direction, float distance) {
        Vector3 ledgeDetectionStartPos = transform.position + (direction * 0.1f) - (transform.up * _initialHeight * 1.1f);
        Vector3 rayDirection = -direction;

        if(Physics.Raycast(ledgeDetectionStartPos, rayDirection, out _surfaceRay, distance)) {
            Vector3 pos = _surfaceRay.point + _surfaceRay.normal.normalized * _initialHeight;
            transform.position = pos;
            transform.rotation = Quaternion.FromToRotation(transform.up, _surfaceRay.normal) * transform.rotation;
            return true;
        }
        return false;
    }
    
    private void TranslatePositionSlightly(Vector3 direction, float moveAmount)
    {
        transform.position += direction * moveAmount;
    }

    public Vector3 GetSurfaceNormal() {
        return _currentSurfaceNormal;
    }
}
