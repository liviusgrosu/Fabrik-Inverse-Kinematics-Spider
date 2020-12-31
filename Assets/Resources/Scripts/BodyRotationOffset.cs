using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate and apply the rotation offset from the body to the feet
/// so that the body retains a stable movement posture
/// </summary>
public class BodyRotationOffset : MonoBehaviour {
    public Transform[] LeftLegs, RightLegs, ForwardLegs, BackwardLegs;

    public Transform LeftLegMarker, RightLegMarker, ForwardLegMarker, BackwardLegMarker;
    
    private void Update() {
        // Calculate the average height of each pair of legs (front, rear, right, left)
        Vector3 avgLeftLeg = CalculateAverageHeight(LeftLegs);
        Vector3 avgRightLeg = CalculateAverageHeight(RightLegs);
        Vector3 avgForwardLeg = CalculateAverageHeight(ForwardLegs);
        Vector3 avgBackwardLeg = CalculateAverageHeight(BackwardLegs);

        // DEBUG: show the pair average position in 3d space
        LeftLegMarker.position = avgLeftLeg;
        RightLegMarker.position = avgRightLeg;
        ForwardLegMarker.position = avgForwardLeg;
        BackwardLegMarker.position = avgBackwardLeg;

        // Get the average front and side direction rotation
        Vector3 sideDirection = avgLeftLeg - avgRightLeg;
        Vector3 frontDirection = avgForwardLeg - avgBackwardLeg;

        // Apply the direction rotation as an offset to the body
        transform.rotation = Quaternion.LookRotation(frontDirection, sideDirection);
    }


    /// <summary>
    /// Calculate the average y position of each leg tail bone
    /// </summary>
    /// <param name="legs">The tailbone of each leg</param>
    /// <returns>The average leg position</returns>
    private Vector3 CalculateAverageHeight(Transform[] legs) {
        Vector3 averageLegs = Vector3.zero;
        foreach(Transform leg in legs) {
            averageLegs += leg.position;
        }   
        averageLegs /= legs.Length;
        return averageLegs;
    }
}
