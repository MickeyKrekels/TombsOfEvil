using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New attribute", menuName = "RPG/Player/Create Attribute")]
public class SkillType : ScriptableObject
{
    public Sprite Icon;
    public string description;
    public int skillPointsSpent;

    public List<Skill> skills = new List<Skill>();
}
