// <copyright file="BuildManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public UpgradeUI upgradeUI;

    public GameObject buildEffect;
    public GameObject myNode;

    private TurretBlueprint turretToBuild;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    //This update function is for selecting a node to upgrade
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            myNode.GetComponent<MyNode>().ClearUpgradeColor();
            //Debug.Log("Mouse 1 being pressed");
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit && hitInfo.transform.gameObject.tag == myNode.tag)
            {
                //Debug.Log("Clicking a node");
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




    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        this.turretToBuild = turret;
    }

    public void ResetTurretToBuild()
    {
        // Debug.Log("Turret Unselected");
        this.SelectTurretToBuild(null);
    }
    
    private void SelectNodeToUpgrade(GameObject node)
    {
        //node.GetComponent<MyNode>().SelectForUpgradeColor();
        if(node == null)
        {
            return;
        }
        MyNode upgradeNode = node.GetComponent<MyNode>();
        upgradeNode.SelectForUpgradeColor();
        Debug.Log("selected");
        upgradeUI.SetTurret(upgradeNode);
    }

    public TurretBlueprint GetTurretBlueprint()
    {
        return turretToBuild;
    }
}
