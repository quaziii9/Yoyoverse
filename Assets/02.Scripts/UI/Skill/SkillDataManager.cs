using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    public string skillDataJson; // JSON 파일 이름을 Unity 에디터에서 할당
    public GameObject skillIconPrefab; // SkillIcon 프리팹
    public Transform skillIconList; // GridLayoutGroup를 가지는 오브젝트

    private List<SkillData> _activeSkills;

    private void Start()
    {
        LoadSkillData();
        CreateActiveSkillIcons();
    }

    private void LoadSkillData()
    {
        _activeSkills = DataLoader<SkillData>.LoadDataFromJson(skillDataJson).FindAll(skill => skill.skillType == "Active");
    }

    private void CreateActiveSkillIcons()
    {
        foreach (SkillData skill in _activeSkills)
        {
            GameObject skillIcon = Instantiate(skillIconPrefab, skillIconList);
            SkillIcon skillIconScript = skillIcon.GetComponent<SkillIcon>();
            skillIconScript.Initialize(skill, skillIconList);
            // 아이콘 이미지 할당
            skillIconScript.iconImage.sprite = Resources.Load<Sprite>($"Skill/Skill_{skill.name}");
        }
    }
}