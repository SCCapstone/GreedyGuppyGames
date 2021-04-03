// <copyright file="Achievements.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    //Beat Level One
    public Image Noob;
    //Beat Level One taking no damage
    public Image Hero;
    //Beat Level Two
    public Image Padawan;
    //Beat Level Two taking no damage
    public Image Veteran;
    //Beat Level Three
    public Image Master;
    //Beat Level Three taking no damage
    public Image Survivor;
    //Beat each level taking no damage
    public Image Untouchable;
    //Beat a level with $3,000 remaining
    public Image GreedyGuppy;
    
    //Sprites for image
    public Sprite xSprite;
    public Sprite checkSprite;

    //Colors for sprites
    public Color redColor;
    public Color greenColor;

    void Start()
    {   
        //Initializes all achievements as a red X
        init();
    }

    void Update()
    {
        //Beat Level One and took no damage
        if(PlayerStats.globalLevel1NoDam == true)
        {
            ChangeImageAndColor(Noob, checkSprite, greenColor);
            ChangeImageAndColor(Hero, checkSprite, greenColor);
        }
        //Beat Level One
        else if(PlayerStats.globalLevel1 == true)
        {
            ChangeImageAndColor(Noob, checkSprite, greenColor);
        }
        //Reset
        else
        {
            ChangeImageAndColor(Noob, xSprite, redColor);
            ChangeImageAndColor(Hero, xSprite, redColor);
        }

        //Beat Level Two and took no damage
        if(PlayerStats.globalLevel2NoDam == true)
        {
            ChangeImageAndColor(Padawan, checkSprite, greenColor);
            ChangeImageAndColor(Veteran, checkSprite, greenColor);
        }
        //Beat Level Two
        else if(PlayerStats.globalLevel2 == true)
        {
            ChangeImageAndColor(Padawan, checkSprite, greenColor);
        }
        //Reset
        else
        {
            ChangeImageAndColor(Padawan, xSprite, redColor);
            ChangeImageAndColor(Veteran, xSprite, redColor);
        }

        //Beat Level Three and take no damage
        if(PlayerStats.globalLevel3NoDam == true)
        {
            ChangeImageAndColor(Master, checkSprite, greenColor);
            ChangeImageAndColor(Survivor, checkSprite, greenColor);
        }
        //Beat Level Three
        else if(PlayerStats.globalLevel3 == true)
        {
            ChangeImageAndColor(Master, checkSprite, greenColor);
        }
        //Reset
        else
        {
            ChangeImageAndColor(Master, xSprite, redColor);
            ChangeImageAndColor(Survivor, xSprite, redColor);
        }

        //Beat every level taking no damage
        if(PlayerStats.globalLevel1NoDam && PlayerStats.globalLevel2NoDam && PlayerStats.globalLevel3NoDam)
        {
            ChangeImageAndColor(Untouchable, checkSprite, greenColor);
        }
        //Reset
        else
        {
            ChangeImageAndColor(Untouchable, xSprite, redColor);
        }

        //Beat a level with $3,000 remaining
        if(PlayerStats.globalGreedyGuppy == true)
        {
            ChangeImageAndColor(GreedyGuppy, checkSprite, greenColor);
        }
        //Reset
        else
        {
            ChangeImageAndColor(GreedyGuppy, xSprite, redColor);
        }
    }

    //Sets all images for achievements to a red X
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
