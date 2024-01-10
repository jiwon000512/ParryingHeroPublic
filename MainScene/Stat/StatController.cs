using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
    public Dictionary<string, int> upgradingStatDic;
    public int plusLevel;
    public uint cost;
    public List<StatView> statViews = new List<StatView>();
    public Text costText;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        plusLevel = 0;
        cost = 0;
        costText.text = cost.ToString();
        costText.color = Color.white;

        foreach (StatView statView in statViews)
        {
            statView.DowngradeUI(0);
        }
    }

    public void Init()
    {
        plusLevel = 0;
        upgradingStatDic = new Dictionary<string, int>();
    }

    public void Upgrade()
    {
        if(cost > SaveManager.data.soul)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        StatUpgradeList statUpgradeList = clickedObject.GetComponent<StatUpgradeList>();
        int currenPoint = 0;
        string statName = null;
        for (int i = 0; i < statUpgradeList.upgradeList.Count; i++)
        {
            statName = statUpgradeList.upgradeList[i].gameObject.name;
            if (upgradingStatDic.TryGetValue(statName, out currenPoint))
            {
                upgradingStatDic[statName]++;
            }
            else
            {
                upgradingStatDic.Add(statName, 1);
            }

            statUpgradeList.upgradeList[i].UpgradeUI(upgradingStatDic[statName]);
        }

        plusLevel++;
        UpdateCost(true, SaveManager.data.stat[statName]+currenPoint);
        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void DownGrade()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        StatUpgradeList statUpgradeList = clickedObject.GetComponent<StatUpgradeList>();
        bool minusLevel = true;
        int currenPoint = 0;
        string statName = null;
        for (int i = 0; i < statUpgradeList.upgradeList.Count; i++)
        {
            statName = statUpgradeList.upgradeList[i].gameObject.name;
            if (upgradingStatDic.TryGetValue(statName, out currenPoint))
            {
                if (currenPoint > 0)
                {
                    upgradingStatDic[statName]--;
                    statUpgradeList.upgradeList[i].DowngradeUI(upgradingStatDic[statName]);
                }
                else
                {
                    SoundManager.instance.Play("Sound/SFX/Deny");
                    minusLevel = false;
                    upgradingStatDic.Remove(statName);
                }
            }
            else
            {
                SoundManager.instance.Play("Sound/SFX/Deny");
                return;
            }
        }

        if (minusLevel)
        {
            plusLevel--;
            UpdateCost(false, SaveManager.data.stat[statName]+currenPoint);
        }
        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void UpdateCost(bool plus, int level)
    {
        if (plus)
        {
            cost += GetUpgradeCost(level);
        }
        else
        {
            if(cost < GetUpgradeCost(level))
            {
                cost = 0;
            }
            else
            {
                cost -= GetUpgradeCost(level);
            }
        }

        if (cost > SaveManager.data.soul)
        {
            costText.color = Color.red;
        }
        else
        {
            costText.color = Color.white;
        }
        costText.text = cost.ToString();

        if (plusLevel == 0)
        {
            costText.color = Color.white;
            costText.text = "0";
        }
    }

    public uint GetUpgradeCost(int level)
    {
        float tempCost = 10f;

        tempCost *= Mathf.Pow(1.08f, level);     //업그레이드소울

        return (uint)tempCost;
    }

    public void Consider()
    {
        if(SaveManager.data.soul < cost)
        {
            return;
        }


        foreach (KeyValuePair<string, int> item in upgradingStatDic)
        {
            SaveManager.data.stat[item.Key] += item.Value;
        }

        foreach (StatView statView in statViews)
        {
            statView.UpgradeFinish();
        }

        SaveManager.data.level += plusLevel;
        SaveManager.data.soul -= cost;
        SaveManager.Save();

        Init();
        cost = 0;
        costText.text = cost.ToString();
        costText.color = Color.white;
        SoundManager.instance.Play("Sound/SFX/Sell");
    }
}
