   using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetFinder : MonoBehaviour
{
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
    public TargetFinder OppositeLeg1, OppositeLeg2;
    public SimpleMovement PlayerMovement;
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
            if (Vector3.Distance(_oldTarget, _currentTarget) > MaxLegDistance && !OppositeLeg1.CalculatingLerp && !OppositeLeg2.CalculatingLerp) {
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
        Vector3 primaryTargetDirection = Quaternion.AngleAxis(25 * Input.GetAxisRaw("Turn"), Vector3.up) * (transform.forward * 1.5f) + PlayerMovement.GetCurrentVelocity() * 2f;
        // DEBUG: Draws the target raycast
        Debug.DrawRay(primaryTargetDirection + transform.position, -transform.up * _IKLegScript.CompleteLength * 1.4f, Color.red);
        // Primary target raycast
        if(Physics.Raycast(primaryTargetDirection + transform.position, -transform.up, out _primaryTargetHit, _IKLegScript.CompleteLength * 1.4f, ~_avoidColliderMask)) {
            return _primaryTargetHit.point;
        }
        // Secondary target raycast
        else {
            if (_closestColliderPoints == null || !_closestColliderPoints.Any()) {
                return Vector3.zero;
            }

            Vector3 closestPoint = _closestColliderPoints.First();
            Vector3 targetPoint = primaryTargetDirection + transform.position;
           
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
    /// Delegate callback function that activates the body offset script
    /// </summary>
    private void CompletedFirstIK() {
        FirstIKResolved = true;
    }
}
