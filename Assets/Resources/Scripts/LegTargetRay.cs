using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determine what the next target should be for the IK target
/// </summary>
public class LegTargetRay : MonoBehaviour {
    [HideInInspector]
    public bool CalculatingLerp;
    [HideInInspector]
    public bool FirstIKResolved;
    [Range(-1,1)]
    public int RayCastDirection;
    public float MaxLegDistance = 9f;
    public Transform IKLeg;
    private RaycastHit _primaryTargetHit;
    public float LerpDuration = 0.5f;
    public SimpleMovement PlayerMovement;
    public LegTargetRay OppositeLeg1, OppositeLeg2;
    public delegate void IKCallback();
    public IKCallback IKMethodToCall;

    private const int _avoidColliderMask = 1 << 8;
    private Vector3 _currentTarget, _oldTarget;
    private float _timeElapsed;
    private FastIKFabric _IKLegScript;
    private List<Vector3> _closestColliderPoints;

    void Start() {
        _IKLegScript = IKLeg.GetComponent<FastIKFabric>();

        _currentTarget = _IKLegScript.GetInitialTargetPos();
        _oldTarget = _IKLegScript.GetInitialTargetPos();

        // Setup the delegate callback function to pass to the IK script
        IKMethodToCall = CompletedFirstIK;

        _IKLegScript.ProvideNewIKResolverCallback(IKMethodToCall);
        _IKLegScript.ProvideNewPosition(_currentTarget);
    }

    /// <remarks>
    /// Using late update so that velocity clamp calculates first
    /// </remarks>
    void LateUpdate() {
        // Calculate the intermediate positions between the old and new target to the IK script
        // This will lerp the leg to the new target
        if (CalculatingLerp) {
            Vector3 lerpPos = Vector3.Lerp(_oldTarget, _currentTarget, _timeElapsed / LerpDuration);
            IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(lerpPos);
            _timeElapsed += Time.deltaTime;
            if (_timeElapsed > LerpDuration) {
                CalculatingLerp = false;
                _oldTarget = CalculateRaycastHit();
                return;
            }
        }
        else {
            _currentTarget = CalculateRaycastHit();
            if(Vector3.Distance(_oldTarget, _currentTarget) > MaxLegDistance && !OppositeLeg1.CalculatingLerp && !OppositeLeg2.CalculatingLerp ){
                // If the current position of the leg is too far away from the body the assign a new target to the IK script
                _timeElapsed = 0f;
                CalculatingLerp = true;
            }
        }
    }

    /// <summary>
    /// Calculate the next raycast hit
    /// </summary>
    private Vector3 CalculateRaycastHit() {
        // Below and outwards of the leg is where it naturally rests
        Vector3 primaryTargetDirection = transform.forward - (transform.up * 1.2f);
        // DEBUG: Draws the target raycast
        Debug.DrawRay(transform.position, primaryTargetDirection + (PlayerMovement.GetCurrentVelocity() * 0.65f), Color.cyan);
        // Primary target raycast
        if(Physics.Raycast(transform.position, primaryTargetDirection + (PlayerMovement.GetCurrentVelocity() * 0.65f), out _primaryTargetHit, _IKLegScript.CompleteLength, ~_avoidColliderMask)) {
            return _primaryTargetHit.point;
        }
        // Secondary target raycast
        else {
            CalculateClosestRayHit();

            if (_closestColliderPoints == null || !_closestColliderPoints.Any()) {
                return Vector3.zero;
            }

            Vector3 closestPoint = _closestColliderPoints.First();
            Vector3 targetPoint = primaryTargetDirection + (PlayerMovement.GetCurrentVelocity() * 0.65f) + transform.position;
           
           // This checks for the closest valid point to the primary target 
            foreach (Vector3 point in _closestColliderPoints.Skip(1)) {
                if(Vector3.Distance(point, targetPoint) < Vector3.Distance(closestPoint, targetPoint)) {
                    closestPoint = point;
                }
            }

            return closestPoint;
        }
    }

    /// <summary>
    /// Create a semi-circle of rays for each leg which checks for valid collision points
    /// </summary>
    void CalculateClosestRayHit()
    {
        int raysToShoot = 16;
        float currAngle = 0;

        _closestColliderPoints = new List<Vector3>();

        for (int i = 0; i < Mathf.CeilToInt(raysToShoot / 2); i++) {
            // Get the point of a circe given an angle
            float x = Mathf.Sin (currAngle);
            float z = Mathf.Cos (currAngle);
            
            // Increment the next angle
            currAngle += RayCastDirection * 2 * Mathf.PI / raysToShoot;
        
            // Get the direction vector of the circle
            Vector3 dir = new Vector3 (transform.position.x + (1.5f * x), transform.position.y - (_IKLegScript.CompleteLength * 0.7f), transform.position.z + (1.5f * z)) - transform.position;

            Debug.DrawRay(transform.position, dir, Color.red);

            RaycastHit extraHit;
            if(Physics.Raycast(transform.position, dir, out extraHit, _IKLegScript.CompleteLength * 1.5f, ~_avoidColliderMask)) {
                // If the ray hits a collider, add that collision point to a list for the secondary target calculation
                _closestColliderPoints.Add(extraHit.point);
            }
        }
    }

    /// <summary>
    /// Delegate callback function that activates the body offset script
    /// </summary>
    private void CompletedFirstIK() {
        FirstIKResolved = true;
    }
}
 