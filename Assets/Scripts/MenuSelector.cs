using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour
{
    public List<RectTransform> options;
    public GameObject icon;
    public Vector2 goal;
    public int currentIndex = 0;
    public bool active;
    public Color activeColor;
    public Color inactiveColor;
    public Color lockedColor;
    public float x;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
        x = iconRectTransform.localPosition.x;
        options = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null && child != icon.transform)
            {
                options.Add(child.GetComponent<RectTransform>());
            }
        }

        UpdateColors();

        InputSystem.actions.FindAction("MenuDown").started += (context) => {
            if (this == null) { 
                return;
            }
            currentIndex += 1;
            currentIndex %= options.Count;
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuUp").started += (context) => {
            if (this == null) { 
                return;
            }
            if (currentIndex == 0)
            {
                currentIndex = options.Count - 1;
            }
            else
            {
                currentIndex -= 1;
                currentIndex %= options.Count;
            }
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuSelect").started += (context) => {
            Select();
        };

        foreach (RectTransform option in options)
        {
            EventTrigger eventTrigger = option.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((data) =>
            {
                currentIndex = options.IndexOf(option);
                UpdatePosition();
            });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            pointerClickEntry.callback.AddListener((data) =>
            {
                Select();
            });
            eventTrigger.triggers.Add(pointerClickEntry);
        }
    }

    void UpdatePosition()
    {
        RectTransform option = options[currentIndex];
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
        goal = new Vector2(x, option.localPosition.y - option.rect.size.y / 2);
    }

    void Select()
    {
        if (!active)
        {
            return;
        }
        Debug.Log(options[currentIndex].name);
        Debug.Log(transform.parent.name);

        if (options[currentIndex].name == "new game")
        {
            if (!GameManager.hasSave)
            {
                GameManager.hasSave = true;
                GameManager.level = 1;
                GameManager.highest_level = 1;
            }
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }

        if (options[currentIndex].name == "continue")
        {
            if (!GameManager.hasSave)
            {
                return;
            }

            SceneManager.LoadScene("Level" + GameManager.level.ToString(), LoadSceneMode.Single);
        }

        if (options[currentIndex].name == "level select")
        {
            this.active = false;
            transform.parent.parent.GetComponentInChildren<CanvasGroup>().alpha = 0.0f;
            GameObject menu = GameObject.Find("level select");
            menu.GetComponent<CanvasGroup>().alpha = 1;
            menu.GetComponentInChildren<MenuSelector>().active = true;
        }

        if (options[currentIndex].name == "resume")
        {
            transform.GetComponentInParent<Pause>().open = false;
        }

        if (options[currentIndex].name == "retry")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        if (options[currentIndex].name == "main menu")
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    void UpdateColors()
    {
        foreach (RectTransform option in options)
        {
            if (options[currentIndex].name == option.name)
            {
                option.GetComponent<TMP_Text>().color = activeColor;
            }
            else
            {
                option.GetComponent<TMP_Text>().color = inactiveColor;
            }

            if (option.name == "continue" && !GameManager.hasSave)
            {
                option.GetComponent<TMP_Text>().color = lockedColor;
            }
            else if (option.name == "continue" && GameManager.hasSave)
            {
                option.GetComponent<TMP_Text>().text = "continue | level " + GameManager.level.ToString();
            }
        }
    }

    void Update()
    {
        UpdateColors();
        if (goal != Vector2.zero)
        {
            icon.GetComponent<RectTransform>().localPosition = Vector2.Lerp(icon.GetComponent<RectTransform>().localPosition, goal, 0.2f);
        }
    }
}
