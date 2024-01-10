using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentController : MonoBehaviour
{
    public static EquipmentController instance;
    public GameObject newEquipmentParicle;
    public List<Sprite> helmetImages;
    public List<Sprite> armorImages;
    public List<Sprite> shoesImages;
    public List<Sprite> cloakImages;
    public List<Sprite> glovesImages;
    public List<Sprite> pantsImages;
    public Dictionary<string, List<Sprite>> equipmentImages;
    public EquipmentView equipmentView;
    public EquipmentModel equipmentModel;
    public List<EquipmentModel> equipmentModels;
    public Transform equipmentParent;
    public bool equipmentIconCreate = false;

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
        equipmentImages = new Dictionary<string, List<Sprite>>();
        equipmentImages.Add("helmet", helmetImages);
        equipmentImages.Add("armor", armorImages);
        equipmentImages.Add("shoes", shoesImages);
        equipmentImages.Add("cloak", cloakImages);
        equipmentImages.Add("gloves", glovesImages);
        equipmentImages.Add("pants", pantsImages);

        equipmentIconCreate = false;

        if (SaveManager.data.newEquipmentGet)
        {
            newEquipmentParicle.SetActive(true);
        }
    }

    public void EquipmentTabButton()
    {
        if (!equipmentIconCreate)
        {
            Init();
            SaveManager.data.newEquipmentGet = false;
            newEquipmentParicle.SetActive(false);
        }
        UpdateEquipmentView();
        UpdateEquipmentModel();
    }

    public void Init()
    {
        //장비 아이콘 생성
        foreach (KeyValuePair<string, List<int>> equipment in SaveManager.data.equipments)
        {
            for (int i = 0; i < equipment.Value.Count; i++)
            {
                GameObject item = ObjectPoolManager.instance.Instantiate("Prefab/EquipmentPrefab");
                EquipmentModel temp = item.GetComponent<EquipmentModel>();
                temp.equipmentImage.sprite = equipmentImages[equipment.Key][i];
                temp.equipmentType = equipment.Key;
                temp.openEquipmentInfoButton.onClick.AddListener(OpenEquipmentInfoButton);
                temp.amount = equipment.Value[i];
                temp.rank = i;

                int price = 0;                                                  //장비 가격
                switch (temp.rank)
                {
                    case 0:
                        price = GetEquipmentPrice(1);
                        break;
                    case 1:
                        price = GetEquipmentPrice(10);
                        break;
                    case 2:
                        price = GetEquipmentPrice(30);
                        break;
                    case 3:
                        price = GetEquipmentPrice(50);
                        break;
                    case 4:
                        price = GetEquipmentPrice(70);
                        break;
                    case 5:
                        price = GetEquipmentPrice(90);
                        break;
                }
                temp.price = price;

                switch (temp.equipmentType)
                {
                    case "helmet":
                        temp.equipmentEffect += "체력 ";
                        break;
                    case "armor":
                        temp.equipmentEffect += "방어력 ";
                        break;
                    case "gloves":
                        temp.equipmentEffect += "공격력 ";
                        break;
                    case "shoes":
                        temp.equipmentEffect += "기력 ";
                        break;
                    case "cloak":
                        temp.equipmentEffect += "치명타 확률 ";
                        break;
                    case "pants":
                        temp.equipmentEffect += "공격속도 ";
                        break;
                }

                temp.equipmentEffect += SaveManager.data.equipmentsEffect[temp.equipmentType][temp.rank].ToString();
                temp.equipmentEffect += "% 증가";

                item.transform.SetParent(equipmentParent);
                item.transform.localScale = new Vector3(1, 1, 1);

                if (equipment.Value[i] == 0)
                {
                    temp.equipmentImage.color = Color.black;
                    temp.openEquipmentInfoButton.interactable = false;
                }
                equipmentModels.Add(temp);
            }
        }

        equipmentIconCreate = true;
    }

    public int GetEquipmentPrice(int x)
    {
        return (int)Mathf.Floor(Mathf.Pow(1.07f, x)) * 10;
    }

    public void UpdateEquipmentModel()
    {
        //장비 정보 업데이트
        foreach (EquipmentModel equipment in equipmentModels)
        {
            equipment.amount = SaveManager.data.equipments[equipment.equipmentType][equipment.rank];
            if (equipment.amount > 0)
            {
                equipment.equipmentImage.color = Color.white;
                equipment.openEquipmentInfoButton.interactable = true;

                if (SaveManager.data.currentEquipmentIndex[equipment.equipmentType] == equipment.rank)
                {
                    equipment.equipping.SetActive(true);
                }
                else
                {
                    equipment.equipping.SetActive(false);
                }
            }
        }
    }

    public void UpdateEquipmentView()
    {
        //착용중인 장비 이미지 업데이트
        foreach (KeyValuePair<string, int> currentEquipment in SaveManager.data.currentEquipmentIndex)
        {
            if (currentEquipment.Value != -1)
            {
                equipmentView.Equip(currentEquipment.Key, equipmentImages[currentEquipment.Key][currentEquipment.Value]);
            }
        }
    }

    public void OpenEquipmentInfoButton()
    {
        equipmentModel = EventSystem.current.currentSelectedGameObject.GetComponent<EquipmentModel>();
        equipmentView.OpenEquipmentInfo();
        equipmentView.UpdateUI(equipmentModel.equipmentImage, equipmentModel.equipmentEffect, equipmentModel.amount, equipmentModel.price);

        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }

    public void EquipButton()
    {
        SaveManager.data.currentEquipmentIndex[equipmentModel.equipmentType] = equipmentModel.rank;
        SaveManager.Save();
        UpdateEquipmentView();
        UpdateEquipmentModel();
        equipmentView.CloseEquipmentInfo();
        SoundManager.instance.Play("Sound/SFX/Equip");
    }

    public void SellButton()
    {
        if (equipmentModel.amount <= 1)
        {
            return;
        }
        equipmentView.Sell(--SaveManager.data.equipments[equipmentModel.equipmentType][equipmentModel.rank]);
        equipmentModel.amount--;

        SaveManager.data.soul += (uint)equipmentModel.price;
        SaveManager.Save();

        SoundManager.instance.Play("Sound/SFX/Sell");
    }

    public void BackButton()
    {
        equipmentView.CloseEquipmentInfo();
        SoundManager.instance.Play("Sound/SFX/ButtonClick");
    }
}
