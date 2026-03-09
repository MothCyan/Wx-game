using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class SkillManager : SerializedMonoBehaviour
{
        private static SkillManager _instance;
        public static SkillManager Instance => _instance;

        void Awake()
        {
            if (_instance == null) { _instance = this; DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);

            // 初始化：让每个 BaseSkill 的 Data 指向对应的 SkillData
            foreach (var pair in skillDictionary)
            {
                if (pair.Key != null && pair.Value != null)
                    pair.Value.Init(pair.Key);
            }
        }

        [OdinSerialize, ShowInInspector]
        private Dictionary<SkillData, BaseSkill> skillDictionary = new Dictionary<SkillData, BaseSkill>();
    
        public void AddSkill(SkillData data, BaseSkill skill)
        {
            if (!skillDictionary.ContainsKey(data))
            {
                skillDictionary[data] = skill;
            }
            else
            {
                Debug.LogWarning($"技能 {data.skillName} 已经存在，无法添加重复技能");
            }
        }
    
        public BaseSkill GetSkill(SkillData data)
        {
            if (skillDictionary.TryGetValue(data, out BaseSkill skill))
            {
                return skill;
            }
            else
            {
                Debug.LogWarning($"技能 {data.skillName} 不存在");
                return null;
            }
        }

        /// <summary>
        /// 随机抽取 count 个技能 Data，可重复，CanExtract=false 的技能不参与抽取
        /// </summary>
        public List<SkillData> RandomExtractSkills(int count = 3)
        {
            // 过滤出可抽取的技能
            List<SkillData> extractable = new List<SkillData>();
            foreach (var pair in skillDictionary)
            {
                if (pair.Value.CanExtract)
                    extractable.Add(pair.Value.Data);
            }

            if (extractable.Count == 0)
            {
                Debug.LogWarning("没有可抽取的技能！");
                return new List<SkillData>();
            }

            // 随机抽取（允许重复）
            List<SkillData> result = new List<SkillData>();
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, extractable.Count);
                result.Add(extractable[index]);
            }

            return result;
        }

        /// <summary>
        /// 根据 SkillData 激活技能，执行 SkillEnable，并解锁 PreliminaryDatas 对应技能的 CanExtract
        /// </summary>
        public BaseSkill EnableSkill(SkillData data)
        {
            if (!skillDictionary.TryGetValue(data, out BaseSkill skill))
            {
                Debug.LogWarning($"技能 {data.skillName} 不存在");
                return null;
            }

            // 执行技能
            skill.SkillEnable();

            // 若标记了 OnlyEnableOnce，Enable 后立即将自身设为不可抽取
            if (data.OnlyEnableOnce)
            {
                skill.CanExtract = false;
                Debug.Log($"[SkillManager] 技能 {data.skillName} 已 Enable，标记为不可再抽取");
            }

            // 将 PreliminaryDatas 中的技能 CanExtract 设为 true
            if (data.PreliminaryDatas != null)
            {
                foreach (SkillData preliminary in data.PreliminaryDatas)
                {
                    if (preliminary == null) continue;
                    if (skillDictionary.TryGetValue(preliminary, out BaseSkill preliminarySkill))
                    {
                        preliminarySkill.CanExtract = true;
                    }
                    else
                    {
                        Debug.LogWarning($"前置技能 {preliminary.skillName} 不存在于字典中");
                    }
                }
            }

            return skill;
        }
}
