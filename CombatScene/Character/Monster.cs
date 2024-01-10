using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Monster : Character
{
    public MonsterData monsterData;
    public Transform hpBarTransform;
    public Vector3 returnPosition;
    public Animator animator;
    public int maxStunStack;
    public int stunStack;
    public int dropSoul;

    private void OnEnable()
    {
        if (animator == null)
        {
            return;
        }
        StartCoroutine(Move());
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        attack = GetComponent<PAttack>();
        //SetStatus();
        battlePointX = Camera.main.transform.position.x + 3f;
        StartCoroutine(Move());

    }

    public override void SetStatus()
    {
        SetStatus(monsterData.hp, monsterData.stunStack, monsterData.dropSoul);
    }

    public void SetStatus(float hp, int stunStack, int dropSoul)
    {
        this.maxHP = hp;
        this.hp = maxHP;
        this.maxStunStack = stunStack;
        this.stunStack = 0;
        this.dropSoul = dropSoul;
        Player.instance.UpdateUI();
    }

    public override IEnumerator Move()
    {
        animator.SetBool("Move", true);
        returnPosition = new Vector3(MonsterSpawnManager.instance.transform.position.x, transform.position.y, transform.position.z);
        while (transform.position.x > battlePointX)
        {
            transform.position += new Vector3(-1, 0, 0) * 15f * Time.deltaTime;
            yield return null;
        }
        animator.SetBool("Move", false);
        Player.instance.SetMonsterInfo(gameObject, true);
        SetStatus();
        GetComponent<PAttack>().AttackReady();
    }

    public override void GetDamage(float damage, bool finishAttack)
    {
        if (finishAttack)
        {
            damage *= 2;
        }
        else
        {
            float equipmentPercent = 0f;
            if(SaveManager.data.currentEquipmentIndex["cloak"] != -1)
            {
                equipmentPercent = SaveManager.data.equipmentsEffect["cloak"][SaveManager.data.currentEquipmentIndex["cloak"]] / 100f;
            }
            float critical = UnityEngine.Random.Range(0f, 1f);
            if (critical <= 1 - Mathf.Pow(0.99f, SaveManager.data.stat["criticalPercentage"]) + equipmentPercent)              //크리티컬확률
            {
                damage *= 2;
            }
        }

        hp -= damage;
        if (hp <= 0.1f)
        {
            GetComponent<PAttack>().attackStatus = AttackStatus.Died;
            Die();
        }
        StartCoroutine(ShowDamageText(damage));
        Player.instance.UpdateUI();
    }

    public void GetDamage(float damage)
    {
        GetDamage(damage, false);
    }

    public IEnumerator ShowDamageText(float damage)
    {
        Player.instance.monsterDamageText.transform.position = gameObject.transform.position + new Vector3(1f, 1f, 0f);
        Player.instance.monsterDamageText.text = Math.Round(damage, 1).ToString() + "x";
        Player.instance.monsterDamageText.color = Color.red;
        Player.instance.monsterDamageText.gameObject.SetActive(true);
        Player.instance.monsterDamageText.DOFade(0f, 0.4f);
        yield return new WaitForSeconds(0.4f);
        Player.instance.monsterDamageText.gameObject.SetActive(false);
    }

    public override void Die()
    {
        SoundManager.instance.Play("Sound/SFX/Die", false, 0.5f);
        StartCoroutine(DieAfterAnim());
    }

    IEnumerator DieAfterAnim()
    {
        animator.Play("Die");
        while (true)
        {
            yield return null;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
            {
                transform.position = returnPosition;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                Player.instance.SetMonsterInfo(gameObject, false);
                ObjectPoolManager.instance.Return(gameObject, returnPosition);
                MonsterSpawnManager.instance.alreadySpawned = false;
                break;
            }
        }
        Player.instance.UpdateUI();
        SaveManager.data.getSoul += (uint)dropSoul;
        SaveManager.data.soul += (uint)dropSoul;
        //SaveManager.Save();
    }
}
