using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotationOffset : MonoBehaviour
{
    public Transform[] leftLegs, rightLegs, forwardLegs, backwardLegs;

    [Space(5)]
    public Transform leftLegMarker; 
    public Transform rightLegMarker;
    public Transform forwardLegMarker;
    public Transform backwardLegMarker;

    float timeElapsed = 0f;

    private void Update()
    {
        Vector3 avgLeftLeg = CalculateAverageHeight(leftLegs);
        Vector3 avgRightLeg = CalculateAverageHeight(rightLegs);
        Vector3 avgForwardLeg = CalculateAverageHeight(forwardLegs);
        Vector3 avgBackwardLeg = CalculateAverageHeight(backwardLegs);

        leftLegMarker.position = avgLeftLeg;
        rightLegMarker.position = avgRightLeg;
        forwardLegMarker.position = avgForwardLeg;
        backwardLegMarker.position = avgBackwardLeg;

        Vector3 sideDirection = avgLeftLeg - avgRightLeg;
        Vector3 frontDirection = avgForwardLeg - avgBackwardLeg;

        transform.rotation = Quaternion.LookRotation(frontDirection, sideDirection);
    }

    private Vector3 CalculateAverageHeight(Transform[] legs)
    {
        Vector3 averageLegs = Vector3.zero;
        foreach(Transform leg in legs)
        {
            averageLegs += leg.position;
        }   

        averageLegs /= legs.Length;

        return averageLegs;
    }
}
