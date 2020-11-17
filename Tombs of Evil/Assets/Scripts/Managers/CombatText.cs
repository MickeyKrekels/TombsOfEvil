using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
    public float textSpeed = 3f;
    public float fadeTime = 5f;

    private Text textUi;

    public void SetCombatText(string text, Color color,Vector3 position, bool critical = false)
    {
        textUi = GetComponent<Text>();
        textUi.text = text;
        textUi.color = color;

        transform.position = position;
        gameObject.SetActive(true);

        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        float time = fadeTime;
        while (time > 0)
        {
            Color color = textUi.color;
            float ratio = time/fadeTime;
            color.a = Mathf.Lerp(0, 1, ratio);
            textUi.color = color;

            textUi.transform.Translate(new Vector3(0, (textSpeed * 10)) * Time.deltaTime);
            time -= Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
