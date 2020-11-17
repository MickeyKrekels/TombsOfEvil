using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour , IPointerClickHandler ,IPointerEnterHandler ,IPointerExitHandler
{
    public TabGroup tabGroup;
    public SkillType type;
    public Image icon;

    [HideInInspector]
    public Image background;

    private void Start()
    {
        if(type != null)
            icon.sprite = type.Icon;

        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    //gets called when tab is selected
    public void Select()
    {

    }
    //gets called when tab is deselected
    public void Deselect()
    {

    }
}
