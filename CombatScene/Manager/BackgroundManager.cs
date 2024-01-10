using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public List<GameObject> backgrounds;


    private void Start()
    {
        int currentStageIndex = SaveManager.data.currentStageData.stageIndex;

        for(int i=0; i<backgrounds.Count; i++)
        {
            if(i == currentStageIndex)
            {
                backgrounds[i].SetActive(true);
                continue;
            }
            backgrounds[i].SetActive(false);
        }
    }
}
