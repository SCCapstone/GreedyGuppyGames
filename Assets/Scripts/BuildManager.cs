// <copyright file="BuildManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;



    public GameObject buildEffect;
    public GameObject myNode;

    private TurretBlueprint turretToBuild;
    private GameObject defaultNode;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BuildManager in scene!");
            return;
        }

        instance = this;
        defaultNode = myNode;
    }
    //This update function is for selecting a node to upgrade
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            myNode.GetComponent<MyNode>().ClearUpgradeColor();
            Debug.Log("Mouse 1 being pressed");
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //Debug.Log(hit);
            if (hit && hitInfo.transform.gameObject.tag == myNode.tag)
            {
                Debug.Log("Clicking a node");
                myNode = hitInfo.transform.gameObject;
                if (myNode.GetComponent<MyNode>().turret != null)
                {
                    SelectNodeToUpgrade(myNode);
                }
            }
        }

    }
    public bool CanBuild
    {
        get { return this.turretToBuild != null; }
    }

    public bool HasMoney
    {
        get { return PlayerStats.Money >= this.turretToBuild.cost; }
    }

    public void BuildTurretOn(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= this.turretToBuild.cost;

        // Build a turret
        // GameObject turretToBuild = buildManager.GetTurretToBuild();
        GameObject turret = (GameObject)Instantiate(this.turretToBuild.prefab, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turret;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret build! Money left: " + PlayerStats.Money);
    }

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        this.turretToBuild = turret;
    }

    public void ResetTurretToBuild()
    {
        Debug.Log("Turret Unselected");
        this.SelectTurretToBuild(null);
    }
    
    private void SelectNodeToUpgrade(GameObject node)
    {
        node.GetComponent<MyNode>().SelectForUpgradeColor();
        Debug.Log("test");

        // If we don't have anything selected and we can't build return true
        // To be used for upgrades
        /*
        if (this.CanBuild == false && this.myNode.turret != null)
        {
            // Remove the line below after implementing upgrades
            Debug.Log("Selecting node for upgrading");
            this.myNode.SelectForUpgradeColor();
            return;
        }*/

    }
}
