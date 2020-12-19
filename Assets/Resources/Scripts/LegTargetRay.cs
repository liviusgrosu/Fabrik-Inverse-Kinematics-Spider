using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTargetRay : MonoBehaviour
{
    RaycastHit hit;
    Vector3 direction;
    public float rayDistance = 12f;
    public float maxDistance = 5f;
    int avoidMask = 1 << 8;

    public Transform IKLeg;

    Vector3 currentTarget, oldTarget;
    
    bool calculatingLerp;

    public float timeElapsed;
    float lerpDuration = 0.5f;

    void Start()
    {
        direction = transform.forward - (transform.up * 1.2f);

        currentTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();
        oldTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();

        IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(currentTarget);
    }

    void Update()
    {
        if (calculatingLerp)
        {
            if (timeElapsed > lerpDuration)
            {
                calculatingLerp = false;
                oldTarget = CalculateRaycastHit();
                return;
            }
            Vector3 lerpPos = Vector3.Lerp(oldTarget, currentTarget, timeElapsed / lerpDuration);
            IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(lerpPos);
            timeElapsed += Time.deltaTime;
        }
        else
        {
            currentTarget = CalculateRaycastHit();
            if(Vector3.Distance(currentTarget, oldTarget) > maxDistance)
            {
                calculatingLerp = true;
            }
        }
    }

    Vector3 CalculateRaycastHit()
    {
        Debug.DrawRay(transform.position, direction * rayDistance, Color.cyan);
        if(Physics.Raycast(transform.position, direction * rayDistance, out hit, Mathf.Infinity, ~avoidMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
