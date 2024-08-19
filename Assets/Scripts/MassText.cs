using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MassText : MonoBehaviour
{

    TMP_Text text;
    [SerializeField] GameObject DeathScreen;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ScaleBehavior.HeldMass > 0)
        {
            text.text = ScaleBehavior.Mass.ToString() + "+" + ScaleBehavior.HeldMass + " / 3";
        }
        else
        {
            text.text = ScaleBehavior.Mass.ToString() + " /3";
        }

        if (ScaleBehavior.Dead && !DeathScreen.activeSelf)
        {
            DeathScreen.SetActive(true);
        }
    }

    public void Reset()
    {
        Debug.Log("bb");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
