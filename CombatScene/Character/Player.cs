using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : Character
{
    public static Player instance;
    public GameObject targetMonster;
    public PAttack targetMonsterAttack;
    public Button finishAttackButton;
    public GameObject finishAttackButtonParticle;
    public Slider hpBar;
    public Slider staminaBar;
    public Text playerDamageText;
    public Text monsterDamageText;
    public Slider monsterHpBar;
    public Slider monsterStunStackBar;
    public Slider monsterAttackFrameBar;
    public bool inCombat;
    public float maxStamina;
    public float stamina;
    public bool isBoss;
    Animator animator;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void SetStatus()
    {
        maxHP = (Mathf.Pow(1.12f, SaveManager.data.stat["hp"])) * 10;            //플레이어체력
        float equipmentPercent = 0f;
        if (SaveManager.data.currentEquipmentIndex["helmet"] != -1)
        {
            equipmentPercent = SaveManager.data.equipmentsEffect["helmet"][SaveManager.data.currentEquipmentIndex["helmet"]] / 100f;
        }
        maxHP *= (1 + equipmentPercent);
        hp = maxHP;
        maxStamina = Mathf.Log(SaveManager.data.stat["stamina"], 5f) + 10;       //스테미나
        equipmentPercent = 0f;
        if (SaveManager.data.currentEquipmentIndex["shoes"] != -1)
        {
            equipmentPercent = SaveManager.data.equipmentsEffect["shoes"][SaveManager.data.currentEquipmentIndex["shoes"]] / 100f;
        }

        maxStamina *= (1 + equipmentPercent);
        stamina = maxStamina;
    }

    private void Start()
    {
        attack = GetComponent<PAttack>();
        battlePointX = Camera.main.transform.position.x - 3f;
        SetStatus();
        StartCoroutine(Move());
        inCombat = false;
        isBoss = false;
        animator = GetComponent<Animator>();

        SoundManager.instance.Play("Sound/BGM/Background" + (SaveManager.data.currentStageData.stageIndex + 1).ToString(), true);
    }

    private void Update()
    {
        StaminaHeal();
        UpdatePlayerUI();
    }

    public override IEnumerator Move()
    {
        while (transform.position.x < battlePointX)
        {
            transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void SetMonsterInfo(GameObject monster, bool inBattle)
    {
        if (inBattle)
        {
            inCombat = true;
            targetMonster = monster;
            GetComponent<PAttack>().AttackReady();
            GetComponent<PParry>().SetAttackInfo(monster.GetComponent<PAttack>());
            if (!isBoss)
            {
                monsterHpBar.transform.position = monster.GetComponent<Monster>().hpBarTransform.position;
                monsterStunStackBar.transform.position = monsterHpBar.transform.position + new Vector3(0, -0.5f, 0);
            }
            if (SaveManager.data.showAttackGauge)
            {
                targetMonsterAttack = monster.GetComponent<PAttack>();
                monsterAttackFrameBar.transform.position = monster.GetComponent<Monster>().hpBarTransform.position + new Vector3(0, 1f, 0);
                monsterAttackFrameBar.gameObject.SetActive(true);
            }
            monsterHpBar.gameObject.SetActive(true);
            monsterStunStackBar.gameObject.SetActive(true);
            UpdateUI();
        }
        else
        {
            inCombat = false;
            targetMonster = null;
            targetMonsterAttack = null;
            monsterAttackFrameBar.gameObject.SetActive(false);
            monsterHpBar.gameObject.SetActive(false);
            monsterStunStackBar.gameObject.SetActive(false);
            finishAttackButton.interactable = false;
            finishAttackButtonParticle.SetActive(false);
            monsterStunStackBar.value = 0f;
        }

        UpdateUI();
    }

    public override void GetDamage(float damage, bool finishAttack)
    {
        float equipmentPercent = 0f;
        if (SaveManager.data.currentEquipmentIndex["armor"] != -1)
        {
            equipmentPercent = SaveManager.data.equipmentsEffect["armor"][SaveManager.data.currentEquipmentIndex["armor"]] / 100f;
        }
        float percent = 1 - Mathf.Round(Mathf.Pow(0.95f, SaveManager.data.stat["attackSpeed"])) + equipmentPercent > 1 ? 1 : 1 - Mathf.Round(Mathf.Pow(0.95f, SaveManager.data.stat["attackSpeed"])) + equipmentPercent;
        damage = damage - (damage / 2) * percent;             //방어력
        hp -= damage;
        if (hp <= 0)
        {
            GetComponent<PAttack>().attackStatus = AttackStatus.Died;
            Die();
        }
        StartCoroutine(ShowDamageText(damage));
    }

    public void GetDamage(float damage)
    {
        GetDamage(damage, false);
    }
    public IEnumerator ShowDamageText(float damage)
    {
        playerDamageText.transform.position = gameObject.transform.position + new Vector3(-1f, 1f, 0f);
        playerDamageText.text = Math.Round(damage, 1).ToString() + "x";
        playerDamageText.color = Color.red;
        playerDamageText.gameObject.SetActive(true);
        playerDamageText.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        playerDamageText.gameObject.SetActive(false);
    }

    public override void Die()
    {
        SoundManager.instance.Play("Sound/SFX/Die", false, 0.5f);
        animator.Play("Die");
        GameEndController.instance.GameOver();
    }

    public void StaminaUse(float cost)
    {
        stamina -= (cost - cost * (1f - Mathf.Pow(0.95f, SaveManager.data.stat["staminaUse"])));        //스테미나사용량
    }

    public void StaminaHeal()
    {
        if (!GetComponent<PParry>().parrying && stamina <= maxStamina)
        {
            stamina += 2 * Time.deltaTime;
        }
    }

    public void UpdatePlayerUI()
    {
        hpBar.value = hp / maxHP;
        staminaBar.value = stamina / maxStamina;
        if (targetMonsterAttack != null)
        {
            monsterAttackFrameBar.value = targetMonsterAttack.currentFrame / (float)targetMonsterAttack.parryAbleFrame;
        }
    }

    public void UpdateUI()
    {
        if (targetMonster == null)
        {
            return;
        }

        monsterHpBar.value = targetMonster.GetComponent<Character>().hp / targetMonster.GetComponent<Character>().maxHP;
        monsterStunStackBar.value = (float)targetMonster.GetComponent<Monster>().stunStack / (float)targetMonster.GetComponent<Monster>().maxStunStack;
        finishAttackButton.interactable = monsterStunStackBar.value == 1f ? true : false;
        finishAttackButtonParticle.SetActive(finishAttackButton.interactable);
    }
}
