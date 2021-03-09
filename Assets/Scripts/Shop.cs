// <copyright file="Shop.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static double sellPercent;
    public double startingSellPercent;

    public Button basicTowerButton;
    public Button cannonTowerButton;
    public Button machineGunTowerButton;
    public Button laserTowerButton;
    public Button electricTowerButton;
    public Button supportTowerButton;
    public Button flameThrowerTowerButton;
    public GameObject basicTowerDescription;
    public GameObject cannonTowerDescription;
    public GameObject machineGunTowerDescription;
    public GameObject laserTowerDescription;
    public GameObject electricTowerDescription;
    public GameObject supportTowerDescription;
    public GameObject flameThrowerTowerDescription;

    public Text basicTowerPrice;
    public Text cannonTowerPrice;
    public Text machineTowerPrice;
    public Text laserTowerPrice;
    public Text electricTowerPrice;
    public Text supportTowerPrice;
    public Text flameThrowerPrice;


    public TurretBlueprint BasicTower;
    public TurretBlueprint CannonTower;
    public TurretBlueprint MachineGunTower;
    public TurretBlueprint LaserTower;
    public TurretBlueprint ElectricTower;
    public TurretBlueprint SupportTower;
    public TurretBlueprint FlameThrowerTower;
    private BuildManager buildManager;

    // sets the text's price on the shop
    private void Start()
    {
        this.buildManager = BuildManager.instance;
        basicTowerPrice.text = "$" + BasicTower.cost;
        cannonTowerPrice.text = "$" + CannonTower.cost;
        machineTowerPrice.text = "$" + MachineGunTower.cost;
        laserTowerPrice.text = "$" + LaserTower.cost;
        electricTowerPrice.text = "$" + ElectricTower.cost;
        supportTowerPrice.text = "$" + SupportTower.cost;
        flameThrowerPrice.text = "$" + FlameThrowerTower.cost;
        sellPercent = this.startingSellPercent;
        
    }

    // makes the buttons turn green/red and make them pressable or not if you have money
    private void Update()
    {
        if(BasicTower.cost <= PlayerStats.Money)
        {
            basicTowerButton.interactable = true;
        }
        else if(BasicTower.cost > PlayerStats.Money)
        {
            basicTowerButton.interactable = false;
        }

        if (CannonTower.cost <= PlayerStats.Money)
        {
            cannonTowerButton.interactable = true;
        }
        else if (CannonTower.cost > PlayerStats.Money)
        {
            cannonTowerButton.interactable = false;
        }

        if (MachineGunTower.cost <= PlayerStats.Money)
        {
            machineGunTowerButton.interactable = true;
        }
        else if (MachineGunTower.cost > PlayerStats.Money)
        {
            machineGunTowerButton.interactable = false;
        }

        if (LaserTower.cost <= PlayerStats.Money)
        {
            laserTowerButton.interactable = true;
        }
        else if (LaserTower.cost > PlayerStats.Money)
        {
            laserTowerButton.interactable = false;
        }

        if (ElectricTower.cost <= PlayerStats.Money)
        {
            electricTowerButton.interactable = true;
        }
        else if (ElectricTower.cost > PlayerStats.Money)
        {
            electricTowerButton.interactable = false;
        }

        if (SupportTower.cost <= PlayerStats.Money)
        {
            supportTowerButton.interactable = true;
        }
        else if (SupportTower.cost > PlayerStats.Money)
        {
            supportTowerButton.interactable = false;
        }

        if (FlameThrowerTower.cost <= PlayerStats.Money)
        {
            flameThrowerTowerButton.interactable = true;
        }
        else if (FlameThrowerTower.cost > PlayerStats.Money)
        {
            flameThrowerTowerButton.interactable = false;
        }
    }


    public void SelectBasicTurret()
    {
        // Debug.Log("Standard Turret Selected");
        this.buildManager.SelectTurretToBuild(this.BasicTower);
    }

    public void SelectCannonTurret()
    {
        // Debug.Log("Cannon Turret Selected");
        this.buildManager.SelectTurretToBuild(this.CannonTower);
    }

    public void SelectMachineGunTurret()
    {
        // Debug.Log("Machine Turret Selected");
        this.buildManager.SelectTurretToBuild(this.MachineGunTower);
    }

    public void SelectLaserTurret()
    {
        this.buildManager.SelectTurretToBuild(this.LaserTower);
    }
    public void SelectElectricTurret()
    {
        this.buildManager.SelectTurretToBuild(this.ElectricTower);
    }
    public void SelectSupportTurret()
    {
        this.buildManager.SelectTurretToBuild(this.SupportTower);
    }
    public void SelectFlameThrowerTurret()
    {
        this.buildManager.SelectTurretToBuild(this.FlameThrowerTower);
    }
}
