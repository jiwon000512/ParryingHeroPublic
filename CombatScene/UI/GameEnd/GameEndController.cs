using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndController : MonoBehaviour
{
    public static GameEndController instance;
    public GameEndView gameEndView;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameEndView = GetComponent<GameEndView>();
    }

    public void GameOver()
    {
        gameEndView.GameOver();
    }

    public void StageClear()
    {
        GetItem();
        NextStageOpen();
        gameEndView.StageClear();
    }

    public void GetItem()
    {
        string[] equipmentTypes = SaveManager.data.currentStageData.appearEquipmentTypes;
        for (int i = 0; i < equipmentTypes.Length; i++)
        {
            float percent = Random.Range(0f, 100f);
            int rank = -1;
            if (percent <= 8f)
            {
                rank = 5;
            }
            else if (percent <= 20f)
            {
                rank = 4;
            }
            else if (percent <= 30f)
            {
                rank = 3;
            }
            else if (percent <= 50f)
            {
                rank = 2;
            }
            else if (percent <= 60f)
            {
                rank = 1;
            }
            else
            {
                rank = 0;
            }

            if (rank != -1)
            {
                rank = rank < SaveManager.data.currentStageData.maxAppearEquipmentRank ? rank : SaveManager.data.currentStageData.maxAppearEquipmentRank;
                SaveManager.data.equipments[equipmentTypes[i]][rank]++;
                if (SaveManager.data.equipments[equipmentTypes[i]][rank] == 1)
                {
                    SaveManager.data.newEquipmentGet = true;
                }
            }
        }
        SaveManager.Save();
    }

    public void NextStageOpen()
    {
        int stageNumber = 0;
        for (int i = 0; i < SaveManager.data.currentStageData.stageIndex; i++)
        {
            stageNumber += 10;
        }
        stageNumber += SaveManager.data.currentStageData.stageNumber;
        if (stageNumber == SaveManager.data.accessibleStageIndex)        //다음 스테이지 오픈
        {
            SaveManager.data.accessibleStageIndex++;
        }
        SaveManager.Save();
    }

    public void AdInit()
    {
        SaveManager.data.showAttackGauge = false;
    }

    public void JustReturnButton()
    {
        AdInit();
        SaveManager.data.soul = 0;
        SaveManager.data.getSoul = 0;
        gameEndView.ReturnToMainScene();
        SaveManager.Save();
    }

    public void ShowADReturnButton()
    {
        GoogleAdMobManager.instance.ShowAds(() =>
        {
            AdInit();
            SaveManager.data.getSoul = 0;
            SaveManager.Save();
            gameEndView.ReturnToMainScene();
        });
    }

    public void JustClearButton()
    {
        AdInit();
        SaveManager.data.getSoul = 0;
        gameEndView.ReturnToMainScene();
        SaveManager.Save();
    }

    public void ShowADClearButton()
    {
        GoogleAdMobManager.instance.ShowAds(() =>
        {
            SaveManager.data.soul += SaveManager.data.getSoul;
            SaveManager.data.getSoul = 0;
            AdInit();
            SaveManager.Save();
            gameEndView.ReturnToMainScene();
        });
    }
}
