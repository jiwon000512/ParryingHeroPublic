using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentView : MonoBehaviour
{
    public Image helmetImage;
    public Image armorImage;
    public Image shoesImage;
    public Image cloakImage;
    public Image glovesImage;
    public Image pantsImage;

    public Image currentEquipmentImage;
    public Text currentEquipmentEffectText;
    public Text currentEquipmentAmountText;
    public Text currentEquipmentPriceText;
    public GameObject equipmentView;
    public GameObject equipmentViewPanel;

    public void OpenEquipmentInfo()
    {
        equipmentView.SetActive(true);
        equipmentViewPanel.SetActive(true);
    }

    public void CloseEquipmentInfo()
    {
        equipmentView.SetActive(false);
        equipmentViewPanel.SetActive(false);
    }

    public void UpdateUI(Image image, string effect, int amount, int price)
    {
        currentEquipmentImage.sprite = image.sprite;
        currentEquipmentEffectText.text = effect;
        currentEquipmentAmountText.text = amount.ToString();
        currentEquipmentPriceText.text = "판매가격 : " + price.ToString();
    }

    public void Equip(string type, Sprite sprite)
    {
        switch (type)
        {
            case "helmet":
                helmetImage.sprite = sprite;
                break;
            case "armor":
                armorImage.sprite = sprite;
                break;
            case "shoes":
                shoesImage.sprite = sprite;
                break;
            case "cloak":
                cloakImage.sprite = sprite;
                break;
            case "gloves":
                glovesImage.sprite = sprite;
                break;
            case "pants":
                pantsImage.sprite = sprite;
                break;
            default:
                break;
        }
    }
    public void Sell(int amount)
    {
        currentEquipmentAmountText.text = amount.ToString();
    }
}
