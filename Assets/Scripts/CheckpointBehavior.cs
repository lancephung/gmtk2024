using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointBehavior : MonoBehaviour
{
    private void Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene.StartsWith("Level"))
        {
            int lvl = int.Parse(scene.Substring(scene.Length - 2));
            GameManager.level = lvl;
            if (lvl > GameManager.highest_level)
            {
                GameManager.highest_level = lvl;
            }
            GameManager.hasSave = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ScaleBehavior>() == null) return;
        if (ScaleBehavior.Dead) return;
        if (!collision.isTrigger) return; // trigger only when the player trigger collides to avoid multiple triggers
        // The player (scale) has reached the checkpoint
        AudioManager.PlaySound("success");

        IEnumerator Delay()
        {
            GameObject.Find("Transition").GetComponent<Animator>().SetTrigger("Exit");

            yield return new WaitForSeconds(0.65f);
        
            if (GameManager.level < 13)
            {
                SceneManager.LoadScene("Level " + (GameManager.level + 1), LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
        }
        StartCoroutine(Delay());
        enabled = false;
    }
}
