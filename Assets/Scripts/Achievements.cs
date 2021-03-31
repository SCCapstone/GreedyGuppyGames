// <copyright file="Achievements.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    public Image Noob;
    public Image Hero;
    public Image Padawan;
    public Image Veteran;
    public Image Master;
    public Image Survivor;
    public Image Untouchable;
    public Image GreedyGuppy;
    
    public Sprite xSprite;
    public Sprite checkSprite;
    public Color redColor;
    public Color greenColor;
    void Start()
    {
        init();
    }

    void Update()
    {
        if(PlayerStats.globalLevel1 == true)
        {
            ChangeImageAndColor(Noob, checkSprite, greenColor);
        }
        else
        {
            ChangeImageAndColor(Noob, xSprite, redColor);
        }

        if(PlayerStats.globalLevel2 == true)
        {
            ChangeImageAndColor(Padawan, checkSprite, greenColor);
        }
        else
        {
            ChangeImageAndColor(Padawan, xSprite, redColor);
        }

        if(PlayerStats.globalLevel3 == true)
        {
            ChangeImageAndColor(Master, checkSprite, greenColor);
        }
        else
        {
            ChangeImageAndColor(Master, xSprite, redColor);
        }

        //Need to add the other 5 achievements here and "save" the data
    }

    //Sets all images for achievements to a red x
    public void init()
    {
        ChangeImageAndColor(Noob, xSprite, redColor);
        
        ChangeImageAndColor(Hero, xSprite, redColor);

        ChangeImageAndColor(Padawan, xSprite, redColor);

        ChangeImageAndColor(Veteran, xSprite, redColor);

        ChangeImageAndColor(Master, xSprite, redColor);

        ChangeImageAndColor(Survivor, xSprite, redColor);

        ChangeImageAndColor(Untouchable, xSprite, redColor);

        ChangeImageAndColor(GreedyGuppy, xSprite, redColor);
    }

    //Changes the sprite and color of an image
    public void ChangeImageAndColor(Image image, Sprite sprite, Color color)
    {
        image.GetComponent<Image>().sprite = sprite;
        image.GetComponent<Image>().color = color;
    }
}
