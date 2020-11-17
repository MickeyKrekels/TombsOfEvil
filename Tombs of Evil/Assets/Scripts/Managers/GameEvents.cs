using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action OnNextRound;
    public void NextRound()
    {
        if(OnNextRound != null)
        {
            OnNextRound();
        }
    }

    public event Action OnDungeonCreated;
    public void DungeonCreated()
    {
        if (OnDungeonCreated != null)
        {
            OnDungeonCreated();
        }
    }

    public event Action OnMovedToTile;
    public void MovedToTile()
    {
        if (OnDungeonCreated != null)
        {
            OnDungeonCreated();
        }
    }

}
