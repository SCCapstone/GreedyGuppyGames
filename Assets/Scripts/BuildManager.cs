// <copyright file="BuildManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public UpgradeUI upgradeUI;

    public GameObject buildEffect;
    public GameObject hitObject;

    private MyNode selectedNode;

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
    
    public void SelectNode(GameObject node)
    {
        
        //node.GetComponent<MyNode>().SelectForUpgradeColor();
        if(node == null)
        {
            return;
        }
        selectedNode = node.GetComponent<MyNode>();
        selectedNode.SelectForUpgradeColor();
        Debug.Log("selected");
        upgradeUI.SetTurret(selectedNode);
    }

    public void DeselectNode()
    {
        selectedNode.ResetColor();
        selectedNode = null;
        upgradeUI.Hide();
    }

    public TurretBlueprint GetTurretBlueprint()
    {
        return turretToBuild;
    }
}
