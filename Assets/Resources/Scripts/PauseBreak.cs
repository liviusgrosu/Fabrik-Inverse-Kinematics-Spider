using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBreak : MonoBehaviour
{
    private bool _isPaused;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Break();
        }
    }
}
