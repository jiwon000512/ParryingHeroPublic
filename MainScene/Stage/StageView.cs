using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageView : MonoBehaviour
{
    public Text stageNameText;
    public Text currentStageIndexText;
    public Text monsterStrengthText;
    public Image bossMonsterSprite;
    public Button battleStartButton;
    public Button showAdBattleStartButton;
    public List<Image> appearEquipmentImageList;
    public Sprite equipmentInitSprite;

    public void InitStageView()
    {
        Color color = Color.white;
        color.a = 0;
        bossMonsterSprite.color = color;

        monsterStrengthText.text = " - ";
        currentStageIndexText.text = " - ";

        foreach (Image appearEquipmentImage in appearEquipmentImageList)
        {
            appearEquipmentImage.sprite = equipmentInitSprite;
        }

        battleStartButton.interactable = false;
        showAdBattleStartButton.interactable = false;
    }

    public void SetStageName(string stageName)
    {
        stageNameText.text = stageName;
    }

    public void UpdateUI(int currentStageIndex, int monsterLevel, Sprite monsterSprite)
    {
        currentStageIndexText.text = currentStageIndex.ToString();
        monsterStrengthText.text = monsterLevel.ToString();
        bossMonsterSprite.color = Color.white;
        bossMonsterSprite.sprite = monsterSprite;

        battleStartButton.interactable = true;
        showAdBattleStartButton.interactable = true;
    }

    public void SetEquipmentImage(List<Sprite> images)
    {
        for (int i = 0; i < appearEquipmentImageList.Count; i++)
        {
            appearEquipmentImageList[i].sprite = images[i];
        }
    }
}
