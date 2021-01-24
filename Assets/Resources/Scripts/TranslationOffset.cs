using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Order execution of the body translation scripts 
/// </summary>

public class TranslationOffset : MonoBehaviour
{
    private LegHeightAverage _legHeightAverage;
    private ObstacleDetection _obstacleDetection;
    private BodyRotationOffset _bodyRotationOffset;

    private void Start() {
        _legHeightAverage = GetComponent<LegHeightAverage>();
        _obstacleDetection = GetComponent<ObstacleDetection>();
        _bodyRotationOffset = GetComponent<BodyRotationOffset>();
    }
    private void Update() {
        //_obstacleDetection.Calculate();
        //_bodyRotationOffset.Calculate();
        _legHeightAverage.Calculate();
    }
}
