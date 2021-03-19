using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hidePanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.hidePanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.hidePanel.SetActive(false);
    }
}
