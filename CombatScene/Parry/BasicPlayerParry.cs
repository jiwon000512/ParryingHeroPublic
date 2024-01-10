using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BasicPlayerParry : PParry
{
    public GameObject sword;
    public Button parryButton;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void SetAttackInfo(PAttack attack)
    {
        enemyAttack = attack;
    }

    public override void Parry()
    {
        if (Player.instance.stamina <= staminaCost)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }
        if (parrying)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }
        if (GetComponent<PAttack>().finishAttacking)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }
        if (!Player.instance.inCombat)
        {
            SoundManager.instance.Play("Sound/SFX/Deny");
            return;
        }

        parrying = true;
        animator.SetBool("Parry", true);
        GetComponent<PAttack>().Parrying(true);

        if (enemyAttack.attackStatus == AttackStatus.Attack)
        {
            parryParticle.SetActive(false);
            parryParticle.SetActive(true);
            enemyAttack.Parried();
            enemyAttack.gameObject.GetComponent<Monster>().stunStack++;
            SoundManager.instance.Play("Sound/SFX/Parry");
            sword.transform.DOShakePosition(1f, 2);
        }
        else
        {
            SoundManager.instance.Play("Sound/SFX/Guard");
        }
        Player.instance.StaminaUse(staminaCost);
        Player.instance.UpdateUI();

        StartCoroutine(Delay(delayTime));
        parryButton.interactable = false;
    }

    public IEnumerator Delay(float value)
    {
        yield return new WaitForSeconds(value);
        animator.SetBool("Parry", false);
        GetComponent<PAttack>().Parrying(false);
        parrying = false;
        parryButton.interactable = true;
    }
}
