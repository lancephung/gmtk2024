using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MassText : MonoBehaviour
{

    TMP_Text text;
    [SerializeField] GameObject DeathScreen;
    [SerializeField] string[] DeathMsgs;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = ScaleBehavior.Mass + ScaleBehavior.HeldMass + " / 5";
        if (ScaleBehavior.HeldMass > 0)
        {
            text.text = ScaleBehavior.Mass + "+" + ScaleBehavior.HeldMass + " / 3";
        }
        else
        {
            text.text = ScaleBehavior.Mass + " / 3";
        }

        if (ScaleBehavior.Dead && !DeathScreen.activeSelf)
        {
            DeathScreen.SetActive(true);
            GameObject.Find("randomtext").GetComponent<TMP_Text>().text = DeathMsgs[Random.Range(0, DeathMsgs.Length)];
            GameObject.Find("deathcausetext").GetComponent<TMP_Text>().text = ScaleBehavior.CauseOfDeathStr;

        }
    }

    public void Reset()
    {
        Debug.Log("bb");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
