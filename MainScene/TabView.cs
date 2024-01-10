using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabView : MonoBehaviour
{
    public List<Button> buttons;
    public List<GameObject> views;

    private void Start()
    {
        SoundManager.instance.Play("Sound/BGM/MainSceneBackground", true);
        Init();
    }
    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        buttons[0].interactable = false;
        views[0].SetActive(true);
        for (int i = 1; i < buttons.Count; i++)
        {
            buttons[i].interactable = true;
            views[i].SetActive(false);
        }
    }

    public void ButtonClick()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        int index = 0;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].gameObject == clickedObject)
            {
                index = i;
                buttons[i].interactable = false;
                continue;
            }
            buttons[i].interactable = true;
        }

        for (int i = 0; i < views.Count; i++)
        {
            if (index == i)
            {
                views[i].SetActive(true);
                continue;
            }
            views[i].SetActive(false);
        }

        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }
}
