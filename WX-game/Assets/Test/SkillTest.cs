using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
    public List<SkillData> SkillDataList = new List<SkillData>(3);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkillDataList = SkillManager.Instance.RandomExtractSkills(3);
            Debug.Log($"[SkillTest] 抽到 {SkillDataList.Count} 个技能：");
            for (int i = 0; i < SkillDataList.Count; i++)
                Debug.Log($"  [{i}] {SkillDataList[i]?.skillName ?? "null"}");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (SkillDataList.Count > 0) SkillManager.Instance.EnableSkill(SkillDataList[0]);
            else Debug.LogWarning("[SkillTest] 请先按Space抽取技能");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (SkillDataList.Count > 1) SkillManager.Instance.EnableSkill(SkillDataList[1]);
            else Debug.LogWarning("[SkillTest] 技能列表不足2个");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (SkillDataList.Count > 2) SkillManager.Instance.EnableSkill(SkillDataList[2]);
            else Debug.LogWarning("[SkillTest] 技能列表不足3个");
        }
    }
}
