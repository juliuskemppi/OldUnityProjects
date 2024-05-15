using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Item Categories
/// </summary>
public enum Category
{
    None, Armor, Weapon, Food, Potion, Valuable, Other, Backpack, Shield
}

/// <summary>
/// Item Rarities
/// </summary>
public enum Rarity
{
    Common, Uncommon, Rare,  VeryRare, Epic, Legendary
}

/// <summary>
/// Base class for all Items
/// </summary>
public abstract class Item : MonoBehaviour
{

    public int value;

    //The ID of the item
    public int itemID;

    //Does the item stack
    public bool Stacking;

    public int Stacksize = 10;

    public Category ItemCategory;

    public Rarity ItemRarity;


    public string Info;

    public Sprite ico;

    public string DisplayName;

    //The object this item spawns
    public GameObject Object;

    public bool isConsumedOnUse;

    /// <summary>
    /// Base method for using the item. 
    /// Has to be overriden in child classes
    /// </summary>
    public virtual void Use()
    {
        Debug.Log.Add(DisplayName + " used");
    }

    /// <summary>
    /// Get the information of the item
    /// </summary>
    /// <returns></returns>
    public virtual string GetInfo()
    {
        return "";
    }
}
