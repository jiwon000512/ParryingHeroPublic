using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PParry : MonoBehaviour
{
    public Animator animator;
    public GameObject parryParticle;
    public PAttack enemyAttack;
    public bool parrying;
    public float delayTime;
    public float staminaCost;

    public virtual void SetAttackInfo(PAttack attack) { }
    public virtual void Parry() { }
}
