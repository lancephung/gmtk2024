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
    public int currentIndex = 0;
    public bool active;
    public Color activeColor;
    public Color inactiveColor;
    public Color lockedColor;

    // Start is called before the first frame update
    void Start()
    {
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
            currentIndex += 1;
            currentIndex %= options.Count;
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuUp").started += (context) => {
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
                currentIndex = options.IndexOf(option);
                UpdatePosition();
                Select();
            });
            eventTrigger.triggers.Add(pointerClickEntry);
        }
    }

    void UpdatePosition()
    {
        RectTransform option = options[currentIndex];
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
        iconRectTransform.localPosition = new Vector2(iconRectTransform.localPosition.x, option.localPosition.y - option.rect.size.y / 2);
    }

    void Select()
    {
        if (!active)
        {
            return;
        }

        if (options[currentIndex].name == "new game")
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
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

            if (option.name == "continue" && !GameManager.hasSave || option.name == "level select" && !GameManager.hasSave)
            {
                option.GetComponent<TMP_Text>().color = lockedColor;
            }
        }
    }

    void Update()
    {
        UpdateColors();
    }
}
