// <copyright file="BuildManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    // Setup fields and globals
    public static BuildManager instance;

    public UpgradeUI upgradeUI;

    public GameObject buildEffect;
    public GameObject hitObject;

    private MyNode selectedNode;

    private TurretBlueprint turretToBuild;

    // Should only run once, if not then there are more than one build manager... and thats bad!
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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (selectedNode != null)
            {
                this.DeselectNode();
            }
            //hitObject.GetComponent<MyNode>().ClearUpgradeColor();
            //Debug.Log("Mouse 1 being pressed");
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit && hitInfo.transform.gameObject.tag == hitObject.tag)
            {
                


                //Debug.Log("Clicking a node");
                hitObject = hitInfo.transform.gameObject;
                if (hitObject.GetComponent<MyNode>().turret != null)
                {
                    SelectNode(hitObject);
                }
            }
        }

    }

    // Checks if tile is empty and thus able to be built on
    public bool CanBuild
    {
        get { return this.turretToBuild != null; }
    }

    // Checks if player has enough money to afford their purchase
    public bool HasMoney
    {
        get { return PlayerStats.Money >= this.turretToBuild.cost; }
    }

    // Selects the turret
    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        this.turretToBuild = turret;
    }

    // Deselects a turret
    public void ResetTurretToBuild()
    {
        // Debug.Log("Turret Unselected");
        this.SelectTurretToBuild(null);
    }
    
    // Runs when user clicks on a node, allows users to inspect what is on the node
    public void SelectNode(GameObject node)
    {
        if(selectedNode != null)
        {
            selectedNode.GetComponent<MyNode>().HideRangeIndicator();
        }
        //node.GetComponent<MyNode>().SelectForUpgradeColor();
        if(node == null)
        {
            return;
        }
        selectedNode = node.GetComponent<MyNode>();
        selectedNode.SelectForUpgradeColor();
        Debug.Log("selected");
        upgradeUI.SetTurret(selectedNode);
        node.GetComponent<MyNode>().ShowRangeIndicator();
    }

    // Deselecting the node
    public void DeselectNode()
    {
        selectedNode.ResetColor();
        selectedNode.HideRangeIndicator();
        selectedNode = null;
        upgradeUI.Hide();
        
    }

    // Gets the blueprint of whatever turret the user wants
    public TurretBlueprint GetTurretBlueprint()
    {
        return turretToBuild;
    }
}
