using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Data", menuName = "Scriptable Object/Attack Data", order = int.MaxValue)]
public class AttackData : ScriptableObject
{
    public string attackName;
    public int parryAbleFrame;
    public int parryDisableFrame;
    public float delay;
}
