using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public PAttack attack;
    public float moveSpeed;
    public float maxHP;
    public float hp;
    public float battlePointX;
    public virtual void SetStatus() { }
    public abstract IEnumerator Move();
    public virtual void Die() { }
    public virtual void GetDamage(float damage, bool finishAttack) { }
}

