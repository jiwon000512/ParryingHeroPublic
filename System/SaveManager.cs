using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public static Data data = new Data();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Load();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Save()
    {
        SaveGame.Save<Data>("save", data);
    }

    public static void Load()
    {
        if (!SaveGame.Exists("save"))
        {
            data = new Data();
            return;
        }
        data = SaveGame.Load<Data>("save", data);
    }
}

public class Data
{
    public int level = 1;
    public uint soul = 0;
    public uint getSoul = 0;
    public bool showTutorial = true;
    public Dictionary<string, int> stat = new Dictionary<string, int>()
    {
        {"hp", 1},
        {"damage", 1},
        {"stamina", 1},
        {"staminaUse", 1},
        {"defense", 1},
        {"criticalPercentage", 1},
        {"attackSpeed", 1},
    };

    public Dictionary<string, List<int>> equipments = new Dictionary<string, List<int>>()
    {
        {"helmet", new List<int>(){0,0,0,0,0,0}},
        {"armor", new List<int>(){0,0,0,0,0,0}},
        {"shoes", new List<int>(){0,0,0,0,0,0}},
        {"cloak", new List<int>(){0,0,0,0,0,0}},
        {"gloves", new List<int>(){0,0,0,0,0,0}},
        {"pants", new List<int>(){0,0,0,0,0,0}},
    };

    public Dictionary<string, int> currentEquipmentIndex = new Dictionary<string, int>
    {
        {"helmet",-1},
        {"armor", -1},
        {"shoes", -1},
        {"cloak", -1},
        {"gloves", -1},
        {"pants", -1},
    };

    public Dictionary<string, List<int>> equipmentsEffect = new Dictionary<string, List<int>>()
    {
        {"helmet", new List<int>(){2,5,8,10,15,30}},           //체력
        {"armor", new List<int>(){2,5,8,10,15,30}},            //방어력
        {"shoes", new List<int>(){5,8,10,15,30,50}},            //기력
        {"cloak", new List<int>(){1,3,5,8,12,20}},            //치명타 확률
        {"gloves", new List<int>(){2,5,7,12,18,30}},           //공격력
        {"pants", new List<int>(){2,5,8,10,15,30}},            //공격속도
    };

    public int accessibleStageIndex = 1;
    public bool newEquipmentGet = false;
    public CurrentStageData currentStageData = new CurrentStageData();
    public bool showAttackGauge = false;
}

public struct CurrentStageData
{
    public int stageIndex;
    public int stageNumber;
    public int monsterLevel;
    public int monsterAmount;
    public string[] appearEquipmentTypes;
    public int maxAppearEquipmentRank;
    public string bossMonsterName;
    public string[] monsterNames;
};
