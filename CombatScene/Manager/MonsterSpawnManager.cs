using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager instance;
    public string[] monsterNames;
    public MonsterData monsterData;
    public string bossMonsterName;
    public int monsterAmount;
    public bool alreadySpawned;
    public bool stageClear;
    public Text bossMonsterNameText;
    public Slider bossMonsterHpBar;
    public Slider bossMonsterStunStackBar;

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

        alreadySpawned = false;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        stageClear = false;
        monsterAmount = 0;
        monsterNames = SaveManager.data.currentStageData.monsterNames;
        bossMonsterName = SaveManager.data.currentStageData.bossMonsterName;
    }

    private void Update()
    {
        Spawn();
    }

    private void Spawn()
    {
        if (alreadySpawned)
        {
            return;
        }
        if (stageClear)
        {
            return;
        }
        else if (!stageClear && monsterAmount > SaveManager.data.currentStageData.monsterAmount)
        {
            GameEndController.instance.StageClear();
            stageClear = true;
            return;
        }

        monsterAmount++;
        if (monsterAmount <= SaveManager.data.currentStageData.monsterAmount)
        {
            int random = Random.Range(0, monsterNames.Length);
            GameObject monster = ObjectPoolManager.instance.Instantiate("MonsterPrefab/" + monsterNames[random]);

            monsterData.hp = (Mathf.Pow(1.07f, SaveManager.data.currentStageData.monsterLevel)) * 10;             //몬스터체력
            monsterData.hp = Mathf.Round(monsterData.hp);
            monsterData.stunStack = Random.Range(1, SaveManager.data.currentStageData.stageIndex + 2);
            monsterData.dropSoul = (int)Mathf.Round(Mathf.Pow(1.1f, SaveManager.data.currentStageData.monsterLevel)) * 2;                  //드랍소울

            monster.GetComponent<Monster>().monsterData = monsterData;
            monster.GetComponent<Monster>().SetStatus();
            monster.transform.position = new Vector3(transform.position.x, monster.transform.position.y, monster.transform.position.z);
        }
        else
        {
            GameObject bossMonster = ObjectPoolManager.instance.Instantiate("MonsterPrefab/Boss/" + bossMonsterName);

            monsterData.hp = (Mathf.Pow(1.07f, SaveManager.data.currentStageData.monsterLevel)) * 10 * 3;             //몬스터체력
            monsterData.hp = Mathf.Round(monsterData.hp);
            monsterData.stunStack = SaveManager.data.currentStageData.stageIndex + 3;
            monsterData.dropSoul = (int)Mathf.Round(Mathf.Pow(1.1f, SaveManager.data.currentStageData.monsterLevel)) * 2 * 5;                  //드랍소울

            bossMonster.GetComponent<Monster>().monsterData = monsterData;
            bossMonster.GetComponent<Monster>().SetStatus();

            bossMonsterNameText.text = bossMonsterName;
            bossMonsterNameText.gameObject.SetActive(true);
            Player.instance.monsterHpBar = bossMonsterHpBar;
            Player.instance.monsterHpBar.gameObject.SetActive(true);
            Player.instance.monsterStunStackBar = bossMonsterStunStackBar;
            Player.instance.isBoss = true;
            bossMonster.transform.position = new Vector3(transform.position.x, bossMonster.transform.position.y, bossMonster.transform.position.z);

            SoundManager.instance.Stop("Sound/BGM/Background" + (SaveManager.data.currentStageData.stageIndex + 1).ToString());
            SoundManager.instance.Play("Sound/BGM/Boss" + (SaveManager.data.currentStageData.stageIndex + 1).ToString(), true);
        }
        alreadySpawned = true;
    }
}
