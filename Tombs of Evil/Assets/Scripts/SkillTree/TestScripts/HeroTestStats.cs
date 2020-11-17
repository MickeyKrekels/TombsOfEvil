using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTestStats : MonoBehaviour
{
    [SerializeField]
    public PlayerStats stats;

    public void UpdateStats(PlayerStats stats)
    {
        this.stats.health += stats.armor;
        this.stats.mana += stats.armor;
        this.stats.armor += stats.armor;

        this.stats.fireResistance += stats.armor;
        this.stats.frostResistance += stats.armor;
        this.stats.fireDamage += stats.armor;
        this.stats.frostDamage += stats.armor;
    }

}
