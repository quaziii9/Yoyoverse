using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IDropHandler
{
    public GameObject skillDescriptionUI;
    public TMP_Text skillNameText; // 스킬 이름 텍스트
    public Image skillIconImage; // 스킬 아이콘 이미지
    public TMP_Text skillDescriptionText; // 스킬 설명 텍스트
    public TMP_Text skillCooldownText; // 스킬 쿨타임 텍스트
    public TMP_Text skillDamageText; // 스킬 데미지 텍스트
    public TMP_Text skillRangeText; // 스킬 범위 텍스트

    // 드래그한 스킬 아이콘을 기존 슬롯 아이콘 과 스왑
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            SkillIcon draggedSkillIcon = eventData.pointerDrag.GetComponent<SkillIcon>();
            if (draggedSkillIcon != null)
            {
                SkillIcon existingSkillIcon = GetComponentInChildren<SkillIcon>();
                if (existingSkillIcon != null && existingSkillIcon != draggedSkillIcon)
                {
                    existingSkillIcon.iconImage.raycastTarget = false;

                    Transform originalParent = draggedSkillIcon.originalParent;
                    if (originalParent != null)
                    {
                        draggedSkillIcon.originalParent = existingSkillIcon.transform.parent;
                        existingSkillIcon.transform.SetParent(originalParent);
                        existingSkillIcon.transform.localPosition = Vector3.zero;

                        existingSkillIcon.iconImage.raycastTarget = true;

                        SkillSlot originalSlot = originalParent.GetComponent<SkillSlot>();
                        if (originalSlot != null)
                        {
                            originalSlot.UpdateSkillDescription(existingSkillIcon.GetSkillData());
                        }
                    }
                }

                draggedSkillIcon.transform.SetParent(transform);
                draggedSkillIcon.transform.localPosition = Vector3.zero;
                draggedSkillIcon.originalParent = transform;

                UpdateSkillDescription(draggedSkillIcon.GetSkillData());
            }
        }
    }

    // 스킬 설명을 업데이트
    public void UpdateSkillDescription(SkillData skillData)
    {
        skillNameText.text = skillData.name;
        skillIconImage.sprite = Resources.Load<Sprite>($"Skill/Skill_{skillData.name}");
        skillDescriptionText.text = skillData.description;
        skillCooldownText.text = $"쿨타임: {skillData.cooldown} 초";
        skillDamageText.text = $"데미지 : {skillData.attackMultiplier}";
        skillRangeText.text = $"스킬 범위 : {skillData.range}";

        skillDescriptionUI.SetActive(true);
        EventManager<UIEvents>.TriggerEvent(UIEvents.UpdateSkillDescription);
    }

    // 스킬 설명을 초기화
    public void ClearSkillDescription()
    {
        skillNameText.text = "";
        skillIconImage.sprite = null;
        skillDescriptionText.text = "";
        skillCooldownText.text = "";
        skillDamageText.text = "";
        skillRangeText.text = "";

        skillDescriptionUI.SetActive(false);
    }
}