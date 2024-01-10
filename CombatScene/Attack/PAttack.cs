using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStatus
{
    CanAttack,
    Ready,
    Attack,
    AttackEnd,
    Parried,
    Parrying,
    Died,
}

public abstract class PAttack : MonoBehaviour
{
    public Animator animator;
    public GameObject target;
    public GameObject attackParticle;
    public List<AttackData> attackDatas;
    public AttackData finishAttackData;
    public AttackStatus attackStatus;
    public string attackName;
    public float damage;
    public float parryAbleFrame;
    public float parryDisableFrame;
    public float currentFrame;
    public float delay;
    public bool parried;
    public bool finishAttacking;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void AttackControll() { }

    public virtual void SetAttackInfo(AttackData attackData) { }

    public virtual void AttackReady() { }
    
    public virtual void AttackStart() { }

    public virtual void Attack() { }

    public virtual void AttackEnd() { }

    public virtual void Parrying(bool parrying) { }
    public virtual void Parried() { }
}
