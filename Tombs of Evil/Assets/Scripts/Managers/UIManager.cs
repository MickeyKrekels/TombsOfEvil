using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager current;
    public GameObject StatPanel;

    private bool StatToggle = true;

    void Start()
    {
        current = this;
    }

    public void MoveStatPanel()
    {
        if (!StatToggle)
        {
            LeanTween.moveX(StatPanel, -150, 0.5f);
            StatToggle = true;
        }
        else
        {
            LeanTween.moveX(StatPanel, 285, 0.5f);
            StatToggle = false;
        }
    }

}
