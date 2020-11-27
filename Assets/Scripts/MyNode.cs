// <copyright file="MyNode.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.EventSystems;

public class MyNode : MonoBehaviour
{
    public Color hoverColor;
    public Color noteEnoughMoneyColor;
    public Vector3 positionOffset;

    [Header("Optional")]
    public GameObject turret;

    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;

    private void Start()
    {
        this.rend = this.GetComponent<Renderer>();
        this.startColor = this.rend.material.color;
        this.buildManager = BuildManager.instance;
    }
    public Vector3 GetBuildPosition()
    {
        return this.transform.position + this.positionOffset;
    }

    private void OnMouseDown()
    {
        if (this.buildManager.CanBuild == false && this.turret != null)
        {
            Debug.Log("test");
            return;
        }
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!this.buildManager.CanBuild)
        {
            return;
        }

        if (this.turret != null)
        {
            Debug.Log("Can't build there! - TODO: Dispaly on screen.");
            return;
        }

        this.buildManager.BuildTurretOn(this);
        this.buildManager.ResetTurretToBuild();
        this.rend.material.color = this.startColor;
    }

    private void OnMouseEnter()
    {
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!this.buildManager.CanBuild)
        {
            return;
        }

        if (this.buildManager.HasMoney)
        {
            this.rend.material.color = this.hoverColor;
        }
        else
        {
            this.rend.material.color = this.noteEnoughMoneyColor;
        }
    }

    private void OnMouseExit()
    {
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }

        this.rend.material.color = this.startColor;
    }

    private void OnMouseOver()
    {
        // changes the color back if right clicked to deselect tower on the node
        if (Input.GetMouseButtonDown(1))
        {
            this.rend.material.color = this.startColor;
        }
    }
}
