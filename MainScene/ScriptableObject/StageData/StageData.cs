using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class NameArray
{
    public string[] names;
}

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public class StageData : ScriptableObject
{
    public int stageIndex;
    public int monsterAmount;
    public NameArray[] monsterNames;
    public List<string> bossMonsterNameList;
    public List<Sprite> bossMonsterImageList;
    public NameArray[] equipmentTypes;
    public List<int> monsterLevels;
}
