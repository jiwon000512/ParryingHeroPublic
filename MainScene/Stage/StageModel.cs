using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageModel : MonoBehaviour
{
    public StageData data;
    public Button stageSelectButton;
    public GameObject cantAccess;
    public string stageName;
    public int stageIndex;
    public int stageNumber;
    public int monsterLevel;
    public string[] appearEquipmentTypes;
    public int appearEquipmentMaxRank;

    private void Awake()
    {
        stageSelectButton = GetComponent<Button>();
    }
    private void OnEnable()
    {
        int accessibleStage = SaveManager.data.accessibleStageIndex;
        for (int i = 0; i < data.stageIndex - 1; i++)
        {
            accessibleStage -= 10;
        }
        if (accessibleStage < int.Parse(gameObject.name))
        {
            cantAccess.SetActive(true);
            stageSelectButton.interactable = false;
        }
        else
        {
            cantAccess.SetActive(false);
            stageSelectButton.interactable = true;
        }
    }
    public void DataInit()
    {
        stageIndex = data.stageIndex - 1;
        appearEquipmentMaxRank = data.stageIndex * 2 - 1 <= 5 ? data.stageIndex * 2 - 1 : 5;            //장비최대랭크 : 5
    }
}
