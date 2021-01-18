using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnivesCountIndicator : MonoBehaviour
{
    public Sprite knifeIconSprite;

    public void Init(int maxKnivesCount)
    {
        for (int i = 0; i < maxKnivesCount; i++)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.sprite = knifeIconSprite;
            image.rectTransform.sizeDelta = new Vector2(50, 50);
            
            image.gameObject.transform.SetParent(transform);
            image.gameObject.transform.SetAsFirstSibling();
        }
    }

    public void MinusOne()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
