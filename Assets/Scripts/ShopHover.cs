// <copyright file="ShopHover.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Just for changing panels states depending if the mouse is hovering over them
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
