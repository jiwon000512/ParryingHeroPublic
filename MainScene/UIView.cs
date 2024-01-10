using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    Level,
    Soul,
}
public class UIView : MonoBehaviour
{
    public static UIView levelUI, SoulUI;
    public UIType uiType;
    public Text uiText;

    private void Awake()
    {
        if (uiType == UIType.Level)
        {
            levelUI = this;
        }
        else if (uiType == UIType.Soul)
        {
            SoulUI = this;
        }
    }

    private void Update()
    {
        if (uiType == UIType.Level)
        {
            uiText.text = SaveManager.data.level.ToString();
        }
        else if (uiType == UIType.Soul)
        {
            uiText.text = SaveManager.data.soul.ToString();
        }
    }
}
