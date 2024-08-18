using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("activated");
        if (!collision.TryGetComponent(out ScaleBehavior scale)) return;
        // The player (scale) has reached the checkpoint
        
    }
}
