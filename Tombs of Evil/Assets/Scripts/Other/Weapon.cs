using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new BaseWeapon", menuName = "2D Weapons/Base Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public float damage;
    public float attackRadius;

    public GameObject weaponPrefab;
    
    public GameObject weaponVisuals;
    private Vector3 offset = new Vector3(-1f, 0, 0);

}
