using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPopup;

    private void Start()
    {
        if(SaveManager.data.showTutorial)
        {
            tutorialPopup.SetActive(true);
        }
        else
        {
            tutorialPopup.SetActive(false);
        }
    }


    public void BackButtonClick()
    {
        tutorialPopup.SetActive(false);
        SaveManager.data.showTutorial = false;
    }
}
