using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotationOffset : MonoBehaviour
{
    public Transform[] leftLegs, rightLegs;

    public Transform leftLegMarker, rightLegMarker;

    private void Update()
    {
        Vector3 avgLeftLeg = CalculateAverageHeight(leftLegs);
        Vector3 avgRightLeg = CalculateAverageHeight(rightLegs);

        leftLegMarker.position = avgLeftLeg;
        rightLegMarker.position = avgRightLeg;

        Vector3 avgLeftRightDirection = avgRightLeg - avgLeftLeg;

         
        Quaternion rotation = Quaternion.LookRotation(avgLeftRightDirection, transform.forward);
        //transform.rotation = rotation;
        Debug.Log(rotation);
        Debug.Break();

        
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
