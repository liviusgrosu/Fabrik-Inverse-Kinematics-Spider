using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBreak : MonoBehaviour
{
    private bool _isPaused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Break();
        }
    }
}
