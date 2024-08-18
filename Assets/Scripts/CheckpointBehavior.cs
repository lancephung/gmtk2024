using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("activated");
        if (collision.GetComponent<ScaleBehavior>() == null) return;
        // The player (scale) has reached the checkpoint
        AudioManager.PlaySound("win");
        enabled = false;
    }
}
