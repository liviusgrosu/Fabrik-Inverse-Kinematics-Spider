using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate and apply the position offset from the body to the feet
/// so that the body retains a stable movement posture
/// </summary>
public class BodyPositionOffset : MonoBehaviour {
    public Transform newPosMarker; 

    public Transform avergeLegMarker;

    public List<LegTargetRay> rays;

    public List<Transform> tailBones;

    private float startingOffset;
    
    private bool initialized;

    void Update() {
        // Wait until all the legs have calculated their targets and IK before checking for body offset
        bool allowedToCalculate = true;
        foreach(LegTargetRay ray in rays) {
            if (!ray.FirstIKResolved) {
                allowedToCalculate = false;
            }
        }
        if(allowedToCalculate) {
            if (!initialized) {
                // Get the starting body offset
                // This will be the reference for onwards
                Vector3 bodyPos = transform.position;
                startingOffset = bodyPos.y - CalculateAverageY();
                // DEBUG: show the body offset position in 3d space
                avergeLegMarker.position = new Vector3(transform.position.x, CalculateAverageY(), transform.position.z);
                initialized = true;
            }
            // Apply the new body offset
            newPosMarker.position = new Vector3(transform.position.x, startingOffset + CalculateAverageY(), transform.position.z);
            transform.position = new Vector3(transform.position.x, startingOffset + CalculateAverageY(), transform.position.z);
        }
    }

    /// <summary>
    /// Calculate the average y position of each leg tail bone
    /// </summary>
    /// <returns>The average leg position</returns>
    float CalculateAverageY() {
        float averageHeight = 0;
        foreach (Transform tailBone in tailBones) {
            averageHeight += tailBone.position.y;
        }
        averageHeight /= tailBones.Count;
        return averageHeight;
    }
}
