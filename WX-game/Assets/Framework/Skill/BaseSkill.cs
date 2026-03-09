using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill 
{
    public SkillData Data { get; private set; }
    public bool CanExtract { get; set; }

    public void Init(SkillData data)
    {
        Data = data;
        CanExtract = data.CanExtractInStart;
    }

    public abstract void SkillEnable();
}
