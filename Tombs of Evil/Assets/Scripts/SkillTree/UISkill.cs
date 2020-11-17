using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UISkill : MonoBehaviour, IPointerClickHandler
{
    [Header("Skill")]

    public Skill currentSkill;

    [Header("UI")]
    public Image image;
    public Text skillLevel;
    public Text cost;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentSkill == null)
        {
            Debug.LogError("No skill assigned");
            return;
        }

        if (!currentSkill.skillLearned)
        {
            SetImageAlpha(.5f);
        }
        else
        {
            SetImageAlpha(1);
        }

        image.sprite = currentSkill.Icon;
        skillLevel.text = currentSkill.skillIndex.ToString();
        var result = currentSkill.CurrentSkillUpgrade();
        if (result == null)
        {
            cost.text = "Skill maxed";
        }
        else
        {

        cost.text = currentSkill.CurrentSkillUpgrade().upgradeCost.ToString() + " EXP";
        }
    }

    private void SetImageAlpha(float val)
    {
        if(val > 1)
        {
            val = 1;
        }
        if(val < 0)
        {
            val = 0;
        }

        var tempColor = image.color;
        tempColor.a = val;
        image.color = tempColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        currentSkill.UpgradeSkill();
        UpdateUI();
    }
}
