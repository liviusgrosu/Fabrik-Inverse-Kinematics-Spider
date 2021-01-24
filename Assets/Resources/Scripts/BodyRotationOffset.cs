using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate and apply the rotation offset from the body to the feet
/// so that the body retains a stable movement posture
/// </summary>
public class BodyRotationOffset : MonoBehaviour {
    public bool DisplayDebug;
    public Transform[] LeftLegs, RightLegs, ForwardLegs, BackwardLegs;

    public Transform LeftLegMarker, RightLegMarker, ForwardLegMarker, BackwardLegMarker;
    
    public void Calculate() {

        // Debug.DrawRay(transform.position, transform.up, Color.red);
        // Debug.DrawRay(transform.position, transform.right, Color.green);
        // Debug.DrawRay(transform.position, transform.forward, Color.blue);
        // Debug.Break();

        // Calculate the average height of each pair of legs (front, rear, right, left)
        Vector3 avgLeftLeg = CalculateAverageHeight(LeftLegs);
        Vector3 avgRightLeg = CalculateAverageHeight(RightLegs);
        Vector3 avgForwardLeg = CalculateAverageHeight(ForwardLegs);
        Vector3 avgBackwardLeg = CalculateAverageHeight(BackwardLegs);

        if (DisplayDebug) {
            // DEBUG: show the pair average position in 3d space
            LeftLegMarker.position = avgLeftLeg;
            RightLegMarker.position = avgRightLeg;
            ForwardLegMarker.position = avgForwardLeg;
            BackwardLegMarker.position = avgBackwardLeg;
        }

        // Get the average front and side direction rotation
        Vector3 sideDirection = avgRightLeg - avgLeftLeg;
        Vector3 frontDirection = avgForwardLeg - avgBackwardLeg;

        // Retain the current Y rotation before applying new rotation
        float currentYRot = transform.rotation.eulerAngles.y;

        // Apply the direction rotation as an offset to the body
        transform.rotation = Quaternion.LookRotation(frontDirection, sideDirection);
        transform.rotation *= Quaternion.AngleAxis(-90f, transform.forward);

        // Lock the Y axis rotation of the body
        Quaternion currentRotation = transform.rotation;
        currentRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, currentYRot, transform.rotation.eulerAngles.z);
        transform.rotation = currentRotation;
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
