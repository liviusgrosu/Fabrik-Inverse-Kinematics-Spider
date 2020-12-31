using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTargetRay : MonoBehaviour
{
    RaycastHit hit;
    Vector3 direction;
    public float rayDistance = 12f;
    public float maxDistance = 9f;
    int avoidMask = 1 << 8;

    public Transform IKLeg;

    Vector3 currentTarget, oldTarget;
    
    [HideInInspector]
    public bool calculatingLerp;

    public float timeElapsed;
    public float lerpDuration = 0.5f;

    public SimpleMovement playerMovement;

    public LegTargetRay oppositeLeg1, oppositeLeg2;
    public bool firstIKResolved;

    public delegate void IKCallback();
    public IKCallback IKMethodToCall;

    [Range(-1,1)]
    public int rayCastDirection;

    private FastIKFabric IKLegScript;

    private List<Vector3> closestColliderPoints;

    void Start()
    {
        IKMethodToCall = CompletedFirstIK;

        direction = transform.forward - (transform.up * 1.2f);

        IKLegScript = IKLeg.GetComponent<FastIKFabric>();

        currentTarget = IKLegScript.GetInitialTargetPos();
        oldTarget = IKLegScript.GetInitialTargetPos();

        IKLegScript.ProvideNewIKResolverCallback(IKMethodToCall);
        IKLegScript.ProvideNewPosition(currentTarget);
    }

    // Using late update so that velocity clamp calculates
    void LateUpdate()
    {
        if (calculatingLerp)
        {
            Vector3 lerpPos = Vector3.Lerp(oldTarget, currentTarget, timeElapsed / lerpDuration);
            IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(lerpPos);
            timeElapsed += Time.deltaTime;
            if (timeElapsed > lerpDuration)
            {
                calculatingLerp = false;
                oldTarget = CalculateRaycastHit();
                return;
            }
        }
        else
        {
            currentTarget = CalculateRaycastHit();
            if(Vector3.Distance(oldTarget, currentTarget) > maxDistance && !oppositeLeg1.calculatingLerp && !oppositeLeg2.calculatingLerp )
            {
                timeElapsed = 0f;
                calculatingLerp = true;
            }
        }
    }

    Vector3 CalculateRaycastHit()
    {
        Debug.DrawRay(transform.position, direction + (playerMovement.GetCurrentVelocity() * 0.65f), Color.cyan);
        if(Physics.Raycast(transform.position, direction + (playerMovement.GetCurrentVelocity() * 0.65f), out hit, IKLegScript.completeLength, ~avoidMask))
        {
            return hit.point;
        }
        else
        {
            CalculateClosestRayHit();

            if (closestColliderPoints == null || !closestColliderPoints.Any())
            {
                return Vector3.zero;
            }

            Vector3 closestPoint = closestColliderPoints.First();
            Vector3 targetPoint = direction + (playerMovement.GetCurrentVelocity() * 0.65f) + transform.position;

            foreach (Vector3 point in closestColliderPoints.Skip(1))
            {
                if(Vector3.Distance(point, targetPoint) < Vector3.Distance(closestPoint, targetPoint))
                {
                    closestPoint = point;
                }
            }

            return closestPoint;
        }
    }

    void CalculateClosestRayHit()
    {
        int raysToShoot = 16;
        float currAngle = 0;

        closestColliderPoints = new List<Vector3>();

        for (int i = 0; i < Mathf.CeilToInt(raysToShoot / 2); i++)
        {
            float x = Mathf.Sin (currAngle);
            float z = Mathf.Cos (currAngle);
            
            currAngle += rayCastDirection * 2 * Mathf.PI / raysToShoot;
        
            Vector3 dir = new Vector3 (transform.position.x + (1.5f * x), transform.position.y - (IKLegScript.completeLength * 0.7f), transform.position.z + (1.5f * z)) - transform.position;

            Debug.DrawRay(transform.position, dir, Color.red);

            RaycastHit extraHit;

            if(Physics.Raycast(transform.position, dir, out extraHit, IKLegScript.completeLength * 1.5f, ~avoidMask))
            {
                closestColliderPoints.Add(extraHit.point);
            }
        }
    }

    private void CompletedFirstIK()
    {
        firstIKResolved = true;
    }
}
 