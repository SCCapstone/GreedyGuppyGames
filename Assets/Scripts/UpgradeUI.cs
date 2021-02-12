using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject ui;
    public RectTransform transformUI;
    public Image turretImage;
    public Text killCount;
    private bool UIOpen = false;

    [Header("Upgrade Text")]
    public Text upgradeLeftText;
    public Text upgradeRightText;

    [Header("Upgrade Buttons")]
    public Button upgradeLeftButton;
    public Button upgradeRightButton;

    [Header("Sell Functionality")]
    public Text sellText;

    private MyNode nodeToUpgrade;

    private void Start()
    {
        ui.SetActive(false);
    }
    public void Update()
    {
        // if upgrade ui is open and we pause it closes the upgrade ui
        if (UIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ui.SetActive(false);
        }
        if (nodeToUpgrade != null)
        {
            this.sellText.text = "$" + this.nodeToUpgrade.moneySpentOnTurret * Shop.sellPercent;
            this.turretImage.sprite = this.nodeToUpgrade.turretBlueprint.turretImage;
            this.killCount.text = "Kills: " + this.nodeToUpgrade.turret.gameObject.GetComponent<Turret>().killCount.ToString();


            if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 0)
            {
                upgradeLeftText.text = nodeToUpgrade.turretBlueprint.upgrade10Text;
                upgradeRightText.text = nodeToUpgrade.turretBlueprint.upgrade01Text;

                if (nodeToUpgrade.turretBlueprint.upgradeCost01 <= PlayerStats.Money)
                {
                    upgradeRightButton.interactable = true;
                }
                else
                {
                    upgradeRightButton.interactable = false;
                }
                if (nodeToUpgrade.turretBlueprint.upgradeCost10 <= PlayerStats.Money)
                {
                    upgradeLeftButton.interactable = true;
                }
                else
                {
                    upgradeLeftButton.interactable = false;
                }
            }
            else if (nodeToUpgrade.upgradePathOne == 1 && nodeToUpgrade.upgradePathTwo == 0)
            {
                upgradeLeftText.text = nodeToUpgrade.turretBlueprint.upgrade20Text;
                upgradeRightButton.interactable = false;
                upgradeRightText.text = "Locked";

                if (nodeToUpgrade.turretBlueprint.upgradeCost20 <= PlayerStats.Money)
                {
                    upgradeLeftButton.interactable = true;
                }
                else
                {
                    upgradeLeftButton.interactable = false;
                }
            }
            else if (nodeToUpgrade.upgradePathOne == 2 && nodeToUpgrade.upgradePathTwo == 0)
            {
                upgradeLeftText.text = nodeToUpgrade.turretBlueprint.upgrade30Text;
                upgradeRightButton.interactable = false;
                upgradeRightText.text = "Locked";

                if (nodeToUpgrade.turretBlueprint.upgradeCost30 <= PlayerStats.Money)
                {
                    upgradeLeftButton.interactable = true;
                }
                else
                {
                    upgradeLeftButton.interactable = false;
                }
            }
            else if (nodeToUpgrade.upgradePathOne == 3 && nodeToUpgrade.upgradePathTwo == 0)
            {
                upgradeLeftText.text = "Fully Upgraded";
                upgradeRightText.text = "Locked";

                upgradeRightButton.interactable = false;
                upgradeLeftButton.interactable = false;
            }
            else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 1)
            {
                upgradeRightText.text = nodeToUpgrade.turretBlueprint.upgrade02Text;
                upgradeLeftButton.interactable = false;
                upgradeLeftText.text = "Locked";

                if (nodeToUpgrade.turretBlueprint.upgradeCost02 <= PlayerStats.Money)
                {
                    upgradeRightButton.interactable = true;
                }
                else
                {
                    upgradeRightButton.interactable = false;
                }
            }
            else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 2)
            {
                upgradeRightText.text = nodeToUpgrade.turretBlueprint.upgrade03Text;
                upgradeLeftButton.interactable = false;
                upgradeLeftText.text = "Locked";

                if (nodeToUpgrade.turretBlueprint.upgradeCost03 <= PlayerStats.Money)
                {
                    upgradeRightButton.interactable = true;
                }
                else
                {
                    upgradeRightButton.interactable = false;
                }
            }
            else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 3)
            {
                upgradeRightText.text = "Fully Upgraded";
                upgradeLeftText.text = "Locked";

                upgradeRightButton.interactable = false;
                upgradeLeftButton.interactable = false;
            }
            else
            {
                Debug.LogError("upgrade settings wrong");
            }
        }
    }

    public void SetTurret(MyNode node)
    {
        nodeToUpgrade = node;
        // transform.position = nodeToUpgrade.GetBuildPosition();
        if (!node.leftNode)
        {
            transformUI.anchorMin = new Vector2(0, .15f);
            transformUI.anchorMax = new Vector2(.25f, .95f);
            this.Activate();
        }
        else
        {
            transformUI.anchorMin = new Vector2(.75f, 0.15f);
            transformUI.anchorMax = new Vector2(1, .95f);
            this.Activate();
        }


    }

    public void Activate()
    {
        UIOpen = true;
        ui.SetActive(true);
    }

    public void Hide()
    {
        UIOpen = false;
        ui.SetActive(false);
    }

    public void UpgradePathOne()
    {
        if (nodeToUpgrade.upgradePathTwo > 0)
        {
            return;
        }
        nodeToUpgrade.upgradePathOne++;
        nodeToUpgrade.UpgradeTurret();

        this.SetTurret(nodeToUpgrade);
    }

    public void UpgradePathTwo()
    {
        if (nodeToUpgrade.upgradePathOne > 0)
        {
            return;
        }
        nodeToUpgrade.upgradePathTwo++;
        nodeToUpgrade.UpgradeTurret();

        this.SetTurret(nodeToUpgrade);
    }

    public void Sell()
    {
        this.nodeToUpgrade.SellTurret();
        this.nodeToUpgrade = null;
        Hide();
    }
}
