using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , ISelectable
{
    private AnimationController walkManager;
    private Vector3 offset = new Vector3(0.5f,1.2f, 0);

    private float health = 100f;

    private bool isAttacking ,IsSpawned;

    private void Start()
    {
        walkManager = GetComponent<AnimationController>();
        GameEvents.current.OnDungeonCreated += SpawnInDungeon;
        GameEvents.current.OnNextRound += NextRound;
    }

    private void NextRound()
    {
        Walk();
    }

    private void Walk()
    {
        if (isAttacking)
            return;
        Derection direction = (Derection)Random.Range(0, System.Enum.GetValues(typeof(Derection)).Length-1);
        walkManager.WalkTowards(direction);
    }

    public void SpawnInDungeon()
    {
        if (!IsSpawned)
            OnDestruct();

        MapGenerator.current.MoveInDungeon(transform);
        IsSpawned = true;
    }



    public void Interact()
    {
        //togle Stats enemy in Ui 
        Debug.Log(gameObject.name);
        //OnDeath();
    }

    public Vector3 GetPosition()
    {
        return (transform.position + offset);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        LeanTween.scale(walkManager.spriteRenderer.gameObject, new Vector3(0.4f, 0.7f, 0), 0.1f).setEase(LeanTweenType.easeInBounce).setLoopPingPong(1);


        string damageString = "-" + damage.ToString();
        //NEEDS FIX spawning result in lag on mobile build
        CombatTextManager.current.SetCombatText(damageString, Color.red, transform.position, offset);

        transform.LeanScaleX(1f, 0.1f);
        if (health <= 0)
        {
            OnDestruct();
        }

    }

    public void OnDestruct()
    {
        GameEvents.current.OnNextRound -= NextRound;
        GameEvents.current.OnDungeonCreated -= SpawnInDungeon;
        SelectableManager.current.selectables.Remove(this);
        Destroy(gameObject);
    }
}
