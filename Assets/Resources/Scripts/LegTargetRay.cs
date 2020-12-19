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
    
    void Start()
    {
        direction = transform.forward - (transform.up * 1.2f);

        currentTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();
        oldTarget = IKLeg.GetComponent<FastIKFabric>().GetInitialTargetPos();

        //IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(currentTarget);
    }

    // Update is called once per frame
    void Update()
    {
        currentTarget = CalculateRaycastHit();
        if(Vector3.Distance(currentTarget, oldTarget) > maxDistance)
        {
            oldTarget = CalculateRaycastHit();
            IKLeg.GetComponent<FastIKFabric>().ProvideNewPosition(currentTarget);
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
