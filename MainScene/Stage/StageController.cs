using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageController : MonoBehaviour
{
    public StageModel stageModel;
    public StageView stageView;
    public List<GameObject> stages;

    public void OpenStageListButton()
    {
        stageView.gameObject.SetActive(true);
        stageView.InitStageView();
        stageModel = EventSystem.current.currentSelectedGameObject.GetComponent<StageModel>();
        stageModel.DataInit();
        int index = stageModel.stageIndex;

        for (int i = 0; i < stages.Count; i++)
        {
            if (i == index)
            {
                stages[i].SetActive(true);
                continue;
            }
            stages[i].SetActive(false);
        }
        stageView.SetStageName(stageModel.stageName);

        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void StageSelectButton()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        int index = int.Parse(clickedObject.name);
        stageModel.monsterLevel = stageModel.data.monsterLevels[index - 1];
        stageModel.appearEquipmentTypes = stageModel.data.equipmentTypes[index - 1].names;
        stageModel.stageNumber = index;

        stageView.UpdateUI(index, stageModel.monsterLevel, stageModel.data.bossMonsterImageList[index - 1]);

        List<Sprite> equipmentImages = new List<Sprite>();
        foreach (string equipmentType in stageModel.appearEquipmentTypes)
        {
            equipmentImages.Add(EquipmentController.instance.equipmentImages[equipmentType][stageModel.appearEquipmentMaxRank]);
        }
        stageView.SetEquipmentImage(equipmentImages);


        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void BackButtonClick()
    {
        stageView.gameObject.SetActive(false);
        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void BattleStartButtonClick()
    {
        SetStageData(stageModel.stageIndex, stageModel.monsterLevel);
        SceneManager.LoadScene("CombatScene");
    }

    public void ShowAdBattleStartButtonClick()
    {
        SetStageData(stageModel.stageIndex, stageModel.monsterLevel);
        GoogleAdMobManager.instance.ShowAds(() =>
        {
            SaveManager.data.showAttackGauge = true;
            SceneManager.LoadScene("CombatScene");
        });
    }

    public void SetStageData(int stageIndex, int monsterLevel)
    {
        SaveManager.data.currentStageData.stageIndex = stageIndex;
        SaveManager.data.currentStageData.monsterLevel = monsterLevel;

        int plusMonster = stageModel.stageNumber > stageModel.data.monsterAmount ? stageModel.data.monsterAmount : stageModel.stageNumber;
        SaveManager.data.currentStageData.monsterAmount = stageModel.data.monsterAmount + plusMonster;
        SaveManager.data.currentStageData.bossMonsterName = stageModel.data.bossMonsterNameList[stageModel.stageNumber - 1];

        SaveManager.data.currentStageData.monsterNames = stageModel.data.monsterNames[stageModel.stageNumber - 1].names;

        SaveManager.data.currentStageData.appearEquipmentTypes = stageModel.appearEquipmentTypes;
        SaveManager.data.currentStageData.maxAppearEquipmentRank = stageModel.appearEquipmentMaxRank;
        SaveManager.data.currentStageData.stageNumber = stageModel.stageNumber;
    }
}
