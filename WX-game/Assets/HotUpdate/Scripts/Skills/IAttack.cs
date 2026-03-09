using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack 
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; }
    bool Attack();
}
