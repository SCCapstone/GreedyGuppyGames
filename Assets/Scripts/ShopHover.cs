// <copyright file="ShopHover.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

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
