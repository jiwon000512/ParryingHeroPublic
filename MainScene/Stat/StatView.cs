using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    public Text statText;
    public GameObject upgradeAmountGameObject;
    public Text upgradeAmountText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        statText.text = SaveManager.data.stat[gameObject.name].ToString();
    }

    public void UpdateUI(string statName)
    {
        statText.text = SaveManager.data.stat[statName].ToString();
    }

    public void UpgradeUI(int value)
    {
        upgradeAmountGameObject.SetActive(true);
        upgradeAmountText.text = value.ToString();
    }

    public void DowngradeUI(int value)
    {
        if (value == 0)
        {
            upgradeAmountGameObject.SetActive(false);
        }
        else
        {
            upgradeAmountText.text = value.ToString();
        }
    }

    public void UpgradeFinish()
    {
        upgradeAmountGameObject.SetActive(false);
        UpdateUI();
    }
}
