using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdleColor;
    public Color tabHoverColor;
    public Color tabActiveColor;

    public TabButton selectedTab;
    public List<GameObject> PagesToSwap;


    private void Start()
    {
        ResetTabs();
    }

    public void Subscribe(TabButton tabButton)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();

        tabButtons.Add(tabButton);
    }

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTabs();
        if (selectedTab == null || tabButton != selectedTab)
        {
            tabButton.icon.color = tabHoverColor;
        }
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tabButton)
    {

        if(selectedTab!= null)
        {
            selectedTab.Deselect();
        }

        selectedTab = tabButton;
        selectedTab.Select();

        ResetTabs();
        tabButton.icon.color = tabActiveColor;

        int index = tabButton.transform.GetSiblingIndex();
        for (int i = 0; i < PagesToSwap.Count; i++)
        {
            if(i == index)
            {
                PagesToSwap[i].SetActive(true);
            }
            else
            {
                PagesToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (var tabButton in tabButtons)
        {
            if (selectedTab != null && tabButton == selectedTab)
                continue;

            tabButton.icon.color = tabIdleColor;
        }
    }


}
