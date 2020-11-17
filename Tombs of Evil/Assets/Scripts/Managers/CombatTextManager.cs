using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTextManager : MonoBehaviour
{
    public static CombatTextManager current;

    public GameObject textPrefab;
    public Transform TextParent;

    private int pooledAmount = 10;
    public List<GameObject> combatTextPool;

    private void Start()
    {
        current = this;

        PoolCombatTexts();

    }

    private void PoolCombatTexts()
    {
        combatTextPool = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(textPrefab, TextParent);
            obj.SetActive(false);
            combatTextPool.Add(obj);
        }
    }

    public void SetCombatText(string text,Color color,Vector3 position,Vector3 offset, bool critical = false)
    {
        //instansite combatText
        position += offset;
        Vector3 newPosition= ConvertWorldToScreen(position);
        GameObject obj = GetNonActiveCombatTexts();

        if (obj == null)
            return;

        CombatText combatText = obj.GetComponent<CombatText>();
        combatText.SetCombatText(text, color, newPosition, critical);
    }

    private GameObject GetNonActiveCombatTexts()
    {
        for (int i = 0; i < combatTextPool.Count; i++)
        {
            if (!combatTextPool[i].activeInHierarchy)
            {
                return combatTextPool[i];
            }
        }
        Debug.LogError("All pool items are active!");
        return null;
    }

    //world position to pixels cordinates 
    public Vector3 ConvertWorldToScreen(Vector3 positionIn)
    {
        RectTransform rectTrans = TextParent.GetComponentInParent<RectTransform>(); //RenderTextHolder

        Vector2 viewPos = Camera.main.WorldToViewportPoint(positionIn);
        Vector2 localPos = new Vector2(viewPos.x * rectTrans.sizeDelta.x, viewPos.y * rectTrans.sizeDelta.y);
        Vector3 worldPos = rectTrans.TransformPoint(localPos);
        float scalerRatio = (1 / this.transform.lossyScale.x) * 2; //Implying all x y z are the same for the lossy scale

        return new Vector3(worldPos.x - rectTrans.sizeDelta.x / scalerRatio, worldPos.y - rectTrans.sizeDelta.y / scalerRatio, 1f);
    }

}
