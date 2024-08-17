using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("activated");
        if (!collision.TryGetComponent(out ScaleBehavior scale)) return;
        // The player (scale) has reached the checkpoint
        Debug.Log("mission accomplished");
        
    }
}
