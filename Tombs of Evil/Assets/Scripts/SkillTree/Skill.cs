using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG/Player/Create Skill")]
public class Skill : ScriptableObject
{
    public Skill[] SkillParents;

    public SkillUpgrade[] skillUpgrades;

    [Header("Skill values")]
    public Sprite Icon;
    public string description;

    [System.NonSerialized]
    public int skillIndex = 0;

    [System.NonSerialized]
    public bool skillLearned;
    [System.NonSerialized]
    public SkillUpgrade currentSkill;


    public void UpgradeSkill()
    {
        if (!SkillParentsIsLearned())
            return;

        if (skillIndex >= skillUpgrades.Length || skillUpgrades.Length <= 0)
            return;

        //Check if players have sufficient EXP

        currentSkill = skillUpgrades[skillIndex];

        if (!skillLearned)
        {
            skillLearned = true;
        }

        skillIndex++;
    }

    public bool SkillParentsIsLearned()
    {
        if (SkillParents == null)
            return true;

        bool ParentsNotLearend = SkillParents.Any(x => x.skillLearned == false);

        if (!ParentsNotLearend)
            return true;

        return false;
    }

    public SkillUpgrade CurrentSkillUpgrade()
    {
        if (skillIndex >= skillUpgrades.Length)
            return null;

        return skillUpgrades[skillIndex];
    }
}
[Serializable]
public class SkillUpgrade
{
    public string name;
    public string description;
    public int upgradeCost;
    [SerializeField] public UpgradeStats upgradeStat;
}

