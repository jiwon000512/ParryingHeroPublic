using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BasicAttack : PAttack
{
    public GameObject finishAttackEffect;
    public bool effect;
    private void Update()
    {
        AttackControll();
    }

    public override void AttackControll()
    {
        if (target != null && target.GetComponent<Character>().hp <= 0)             //적 죽음
        {
            attackStatus = AttackStatus.AttackEnd;
            return;
        }

        if (target != null && target.GetComponent<PAttack>().finishAttacking)        //적이 마무리 공격중에는 공격 불가
        {
            attackStatus = AttackStatus.CanAttack;
            return;
        }

        if (!Player.instance.inCombat)
        {
            return;
        }

        switch (attackStatus)                                                       //공격상태별체크
        {
            case AttackStatus.CanAttack:
                AttackReady();
                return;
            case AttackStatus.AttackEnd:
                return;
            case AttackStatus.Parrying:
                return;
            case AttackStatus.Parried:
                Parried();
                return;
            case AttackStatus.Died:
                return;
            default:
                break;
        }

        if (currentFrame == 0)                                                      //공격 프레임별 체크
        {
            AttackStart();
        }
        else if (currentFrame >= parryAbleFrame && currentFrame < parryDisableFrame)
        {
            Attack();
        }
        else if (currentFrame >= parryDisableFrame)
        {
            AttackEnd();
        }

        currentFrame++;
    }


    //다음공격세팅
    public override void SetAttackInfo(AttackData attackData)
    {
        target = Player.instance.gameObject == this.gameObject ? Player.instance.targetMonster : Player.instance.gameObject;
        attackParticle.transform.position = target.transform.position;

        attackName = attackData.attackName;
        //데미지 설정
        if (target == Player.instance.gameObject)
        {
            damage = (Mathf.Pow(1.1f, SaveManager.data.currentStageData.monsterLevel)) + 2;                     //몬스터데미지
        }
        else
        {
            damage = (Mathf.Pow(1.155f, SaveManager.data.stat["damage"])) + 2;                                //플레이어데미지
            float equipmentPercent = 0f;
            if(SaveManager.data.currentEquipmentIndex["gloves"]!=-1)
            {
                equipmentPercent = SaveManager.data.equipmentsEffect["gloves"][SaveManager.data.currentEquipmentIndex["gloves"]]/100f;
            }
            damage *= (1 + equipmentPercent);
        }

        parryAbleFrame = attackData.parryAbleFrame;
        parryDisableFrame = attackData.parryDisableFrame;
        delay = attackData.delay;

        parried = false;
        currentFrame = 0;

        if (attackStatus != AttackStatus.Parrying)
        {
            attackStatus = AttackStatus.Ready;
        }

        if (finishAttacking)
        {
            attackStatus = AttackStatus.Ready;
        }
    }

    public void FinishAttack()
    {
        if (finishAttacking)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }
        if (target.GetComponent<Character>().hp <= 0)
        {
            return;
        }
        SoundManager.instance.Play("Sound/SFX/Player/FinishAttackStart");
        target.GetComponent<Monster>().stunStack = 0;
        SetAttackInfo(finishAttackData);
        StopAllCoroutines();
        finishAttacking = true;

        finishAttackEffect.SetActive(true);
        Color color = finishAttackEffect.GetComponent<Image>().color;
        color.a = 0f;
        finishAttackEffect.GetComponent<Image>().color = color;
        finishAttackEffect.GetComponent<Image>().DOFade(0.5f, 0.4f);

        Player.instance.UpdateUI();
    }

    public IEnumerator FinishAttackEffect()
    {
        Time.timeScale = 0.1f;
        finishAttackEffect.GetComponent<Image>().DOFade(0f, 0.3f);
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 0.5f;
        target.transform.DOShakePosition(0.4f, 2f);
        yield return new WaitForSecondsRealtime(0.1f);
        finishAttackEffect.SetActive(false);
        Time.timeScale = 1f;
    }

    public override void AttackReady()
    {
        if (finishAttacking)
        {
            return;
        }
        AttackData attackData = attackDatas[Random.Range(0, attackDatas.Count)];
        SetAttackInfo(attackData);
    }

    public override void AttackStart()
    {
        if (Player.instance.hp <= 0 || attackStatus == AttackStatus.Parrying)
        {
            return;
        }

        if (Player.instance.inCombat)
        {
            if (Player.instance.gameObject == gameObject)
            {
                animator.Play(attackName);
            }
            else
            {
                animator.SetTrigger(attackName);
            }
        }
    }

    public override void Attack()
    {
        attackStatus = AttackStatus.Attack;

        if (!effect && finishAttacking)
        {
            StartCoroutine(FinishAttackEffect());
            effect = true;
        }
    }

    public override void AttackEnd()
    {
        if (parried)
        {
            return;
        }
        if (GetComponent<Character>().hp <= 0)
        {
            return;
        }

        string type = gameObject.name == "Player" ? "Player" : "Monster";
        SoundManager.instance.Play("Sound/SFX/" + type + "/" + attackName, false, 0.5f);
        attackStatus = AttackStatus.AttackEnd;
        currentFrame = 0;
        attackParticle.SetActive(false);
        attackParticle.SetActive(true);

        target.GetComponent<Character>().GetDamage(damage, finishAttacking);        //데미지입힘
        target.transform.DOShakePosition(0.3f, 0.7f);

        StartCoroutine(AttackCoolDown(delay));
    }

    public override void Parried()
    {
        if (parried)
        {
            return;
        }
        parried = true;
        attackStatus = AttackStatus.Parried;
        animator.SetTrigger("Attacked");
        StartCoroutine(AttackCoolDown(delay));
    }

    public override void Parrying(bool parrying)
    {
        if (parrying)
        {
            attackStatus = AttackStatus.Parrying;
        }
        else
        {
            if (finishAttacking)
            {
                attackStatus = AttackStatus.Ready;
                return;
            }
            attackStatus = AttackStatus.AttackEnd;
            StopAllCoroutines();
            StartCoroutine(AttackCoolDown(1f));
        }
    }

    public IEnumerator AttackCoolDown(float delay)
    {
        if (gameObject == Player.instance.gameObject)
        {
            float equipmentPercent = 0f;
            if(SaveManager.data.currentEquipmentIndex["pants"] != -1)
            {
                equipmentPercent = SaveManager.data.equipmentsEffect["pants"][SaveManager.data.currentEquipmentIndex["pants"]] / 100f;
            }
            float percent = 1 - Mathf.Round(Mathf.Pow(0.98f, SaveManager.data.stat["attackSpeed"])) + equipmentPercent > 1 ? 1 : 1 - Mathf.Round(Mathf.Pow(0.95f, SaveManager.data.stat["attackSpeed"])) + equipmentPercent;
            float delayAfterStat = delay - (delay / 2) * percent;         //공격속도
            yield return new WaitForSeconds(delayAfterStat);
        }
        else
        {
            float randomDelay = Random.Range(0.7f, 2f);
            yield return new WaitForSeconds(randomDelay);
        }

        if (attackStatus != AttackStatus.Parrying)
        {
            attackStatus = AttackStatus.CanAttack;
            finishAttacking = false;
            effect = false;
        }
    }
}
