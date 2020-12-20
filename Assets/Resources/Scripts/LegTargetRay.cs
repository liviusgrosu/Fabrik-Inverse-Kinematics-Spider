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
    
    bool calculatingLerp;

    public float timeElapsed;
    public float lerpDuration = 0.5f;

    public SimpleMovement playerMovement;

    void Start()
    {
        direction = transform.forward - (transform.up * 1.2f);

        currentTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();
        oldTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();

        IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(currentTarget);
    }

    void Update()
    { 
        Debug.DrawRay(transform.position, direction + (playerMovement.GetCurrentVelocity() * 0.3f), Color.red);
        
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
            if(Vector3.Distance(oldTarget, currentTarget) > maxDistance)
            {
                timeElapsed = 0f;
                calculatingLerp = true;
            }
        }
    }

    Vector3 CalculateRaycastHit()
    {
        Debug.DrawRay(transform.position, direction + (playerMovement.GetCurrentVelocity() * 0.4f), Color.cyan);
        if(Physics.Raycast(transform.position, direction + (playerMovement.GetCurrentVelocity() * 0.4f), out hit, Mathf.Infinity, ~avoidMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
