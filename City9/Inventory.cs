using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Structure that holds the display information of a piece of armor
/// </summary>
public struct ArmorDisplayInformation
{
    //the display name
    string DisplayName;

    //The icon of the armor
    Sprite ico;

    //The armor value of the armor
    int armorValue;

    /// <summary>
    /// Creates display information of a piece of armor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    public ArmorDisplayInformation(string name, Sprite icon, int ArmorValue)
    {
        DisplayName = name;
        ico = icon;
        armorValue = ArmorValue;
    }

    /// <summary>
    /// Get the information of a ArmorDisplayInformation
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    public void GetInformation(out string name, out Sprite icon, out int ArmorValue)
    {
        name = DisplayName;
        icon = ico;
        ArmorValue = armorValue;
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// InventorySlot holds all information about items in those slots
/// </summary>
public class InventorySlot
{
    //Variables

    int itemID;    //The ID of the item

    int amount;    //The amount of items

    bool stacking;     //Does the item stack

    protected Item item;    //The item in the slot

    //The index of the inventory slot
    int index;

    //The max amount of items that can be added
    int maxStacksize;

    /// <summary>
    /// Constructor that creates an empty slot
    /// </summary>
    public InventorySlot()
    {
        itemID = -1;
        stacking = false;
    }

    //Methods and functions

    /// <summary>
    /// The Constructor with an item param
    /// </summary>
    /// <param name="Item"> The item to put in to the slot</param>
    public InventorySlot(Item Item)
    {
        if(Item.itemID == -1)
        {
            string e = "Item " + Item.ToString() + " has invalid ID";
            throw new Exception(e);
        }
        item = Item;
        itemID = Item.itemID;
        stacking = Item.Stacksize > 1;
        amount = 1;
        maxStacksize = Item.Stacksize;
    }

    /// <summary>
    /// Constructor that takes Amount as an additional parameter
    /// </summary>
    /// <param name="Item"> The item to add to the slot</param>
    /// <param name="Amount"> The amount of items to add</param>
    public InventorySlot(Item Item, int Amount)
    {
        if (Item.itemID == -1)
        {
            string e = "Item " + Item.ToString() + " has invalid ID";
            throw new Exception(e);
        }
        item = Item;
        itemID = Item.itemID;
        stacking = Item.Stacksize > 1;
        amount = Amount;
        maxStacksize = Item.Stacksize;
    }

    /// <summary>
    /// Get the ID of the item in the inventory slot
    /// </summary>
    /// <returns>itemID</returns>
    public int GetID()
    {
        return itemID;
    }

    /// <summary>
    /// Can another item be added to the inventory slot
    /// </summary>
    /// <returns></returns>
    public bool CanStack()
    {
        return amount < maxStacksize;
    }

    /// <summary>
    /// Adds one to the item amount
    /// </summary>
    public void AddOne()
    {
        amount++;
    }

    public int GetAmount()
    {
        return amount;
    }

    /// <summary>
    /// Removes one from the item amount
    /// </summary>
    public void RemoveOne()
    {
        amount--;
    }

    /// <summary>
    /// Gets the item in the slot
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        return item;
    }

    /// <summary>
    /// Adds an item to the slot
    /// </summary>
    /// <param name="I"></param>
    public virtual void AddItem(Item I)
    {
        if (item != null)
        {
            throw new Exception("Can't add an item to a slot that is not empty");
        }
        item = I;
        itemID = I.itemID;
        amount = 1;
        stacking = I.Stacking;
    }
 
    /// <summary>
    /// Calls the use method of the item that is in the slot
    /// </summary>
    public void UseItemInSlot()
    {
        try
        {
            Debug.Log.Add("Slot item used");
            item.Use();
            InventoryGUI.instance.updateInventory();
        }
        catch(NullReferenceException ex)
        {
            Debug.Log.Add("Using item failed");
            Debug.Log.Add(ex.Message);
        }

    }

    /// <summary>
    /// Tests is the item consumed on use
    /// </summary>
    /// <returns></returns>
    public bool IsConsumed()
    {
        return item.isConsumedOnUse;
    }

    /// <summary>
    /// Adds multiple items to the slot
    /// </summary>
    /// <param name="Amount"></param>
    public void AddItems(int Amount)
    {
        //exeption
        if(amount + Amount > maxStacksize)
        {
            throw new Exception();
        }
        amount += Amount;
    }

    /// <summary>
    /// Get the max stack size
    /// </summary>
    /// <returns></returns>
    public int GetMaxStack()
    {
        return maxStacksize;
    }

    /// <summary>
    /// Gets the saved info about the item slot
    /// </summary>
    /// <returns></returns>
    public string GetItemString()
    {
        return itemID.ToString() + ":" + amount.ToString();
    }

    public void SetIndex(int Index)
    {
        index = Index;
    }

    public int GetIndex()
    {
        return index;
    }

    /// <summary>
    /// Remove multiple items from the slot
    /// </summary>
    /// <param name="Amount"></param>
    public void RemoveItems(int Amount)
    {
        amount -= Amount;
    }

    /// <summary>
    /// Returns the total value of the items in an inventory slot
    /// </summary>
    /// <returns></returns>
    public int GetTotalValue()
    {
        return amount * item.value;
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Armor slot contains a piece of armor. 
/// </summary>
public class ArmorSlot
{
    //The item in the slot
    ArmorItem Item;

    //The world item that displays the armor on the player
    GameObject PlayerItem;

    //The body of the character
    GameObject CharacterBody;

    //The name of the slot
    string slotName;

    //The id of the item in the slot
    public int ID;

    //Constructor
    public ArmorSlot(string Name)
    {
        slotName = Name;
        ID = -1;

    }

    /// <summary>
    /// Gets the item in the slot
    /// </summary>
    /// <returns></returns>
    public ArmorItem GetItem()
    {
        return Item;
    }

    /// <summary>
    /// Sets the gameobject that this changs
    /// </summary>
    /// <param name="item">the item</param>
    public void SetPlayerItem(GameObject item)
    {
        PlayerItem = item;
        //Debug.Log.Add(PlayerItem.ToString());
    }

    /// <summary>
    /// Get the item on the character
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerItem()
    {
        return PlayerItem;
    }

    /// <summary>
    /// Sets the characters body 
    /// </summary>
    /// <param name="body"></param>
    public void SetBody(GameObject body)
    {
        CharacterBody = body;
    }


    /// <summary>
    /// Set the item thats in the slot
    /// </summary>
    /// <param name="item">The item to assing to the slot</param>
    public void SetItem(ArmorItem item)
    {
        try
        {
            //Set the item id
            ID = item.itemID;

            //Set the item
            Item = item;

            //Update the armor value in the UI
            if (UpdateBasicStats.Instance != null)
            {
                UpdateBasicStats.Instance.UpdateArmor();
            }
            PlayerItem.SetActive(true);

            //Change the blend shape of the torso
            if (item.type == ArmorType.chest)
            {
                CharacterBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100);
                //Debug.Log.Add(CharacterBody.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0).ToString());
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log.Add(item.DisplayName);
            Debug.Log.Add(ex.Message);

        }
    }

    /// <summary>
    /// Remove the armor item in the slot
    /// </summary>
    public void RemoveItem()
    {

        //Is the item a chestplate
        //if(Item.type == ArmorType.chest)
        //{
        //    //Reset the blend shape weight
        //    CharacterBody.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);

        //}
        if(Item != null)
        Inventory.instance.AddToInventory(Item);
        Item = null;
        PlayerItem.SetActive(false);
    }

    /// <summary>
    /// Get the armor value of the item in the slot
    /// </summary>
    /// <returns></returns>
    public int GetSlotArmor()
    {
        //Debug.Log.Add("Get Armor");
        try
        {
            return Item.ArmorValue;
        }
        catch
        {

            return 0;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Inventory holds all of the items that the player has
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="Inventory">String used to create a inventory</param>
    public Inventory(string Inventory)
    {
        BuildFromString(Inventory);
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Inventory()
    {


    }

    //Variables

    //Show extra debug messages
    [SerializeField]
    bool DebugInventory = true;

    //A static instance of the inventory
    public static Inventory instance;

    //The inventory 
    List<InventorySlot> InventorySlots = new List<InventorySlot>();

    //Armor slots
    List<ArmorSlot> ArmorSlots = new List<ArmorSlot>()
    {
        new ArmorSlot("Helmet"),
        new ArmorSlot("Chest"),
        new ArmorSlot("Legs"),
        new ArmorSlot("Boots"),
        new ArmorSlot("Gloves")
    };

    //The max amount of inventory slots
    int MaxSize = 15;

    //The currently equipped weapon
    WeaponItem CurrentWeapon;
    GameObject KatanaGraphics;
    GameObject SwordGraphics;

    //Current Shield
    ShieldItem CurShield;
    GameObject shieldGraphics;

    //The level of the inventory
    int inventoryLevel = 1;

    //Methods and functions

    //Called before start
    private void Awake()
    {
        instance = this;
    }

    //Called in the beginning 
    private void Start()
    {

        SetEquippedItems("1(5|6|7|8|12)11]10["); Debug.Log.Add("Full armor and weapons spawned at start for testing purposes");

        //BuildFromString("[1:10|2:5|3:10]");
        //UseItem(1);

        //Does the transform have child gameobjects
        if (transform.childCount > 0)
        {
            //Get the child of the transform of the inventory
            Transform Graphics = transform.GetChild(0);
            if (Graphics)
            {
                //Get the child of the child
                Transform GFX = Graphics.GetChild(0);
                if (GFX)
                {
                    //Get the Armor pieces and give them to the armor slots
                    ArmorSlots[0].SetPlayerItem(GFX.Find("Helm").gameObject);
                    ArmorSlots[1].SetPlayerItem(GFX.Find("Platebody").gameObject);
                    ArmorSlots[1].SetBody(GFX.Find("Body").gameObject);
                    ArmorSlots[2].SetPlayerItem(GFX.Find("Pants").gameObject);
                    ArmorSlots[3].SetPlayerItem(GFX.Find("Shoes").gameObject);
                    ArmorSlots[4].SetPlayerItem(GFX.Find("Gauntlets").gameObject);
                    SwordGraphics = GFX.Find("Steel Sword").gameObject;
                    KatanaGraphics = GFX.Find("Katana").gameObject;
                    shieldGraphics = GFX.Find("Shield").gameObject;
                    //shieldGraphics.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Resets the inventory and equipped items to their default values
    /// </summary>
    public void Reset()
    {
        //Reset the inventory
        InventorySlots = new List<InventorySlot>();

        //Reset armor slots
        ArmorSlots = new List<ArmorSlot>()
        {
            new ArmorSlot("Helmet"),
            new ArmorSlot("Chest"),
            new ArmorSlot("Legs"),
            new ArmorSlot("Boots"),
            new ArmorSlot("Gloves")
        };

        //Reset Weapon
        SetCurrentWeapon(null);

    }

    /// <summary>
    /// Adds an item to inventory
    /// </summary>
    /// <param name="item">Item to add</param>
    public void AddToInventory(Item item)
    {
        try
        {
           
            //Is the item null;
            if (item != null)
            {
                int slot = 0;

                //Does the item already exist in the inventory
                if (FindItems(item.itemID, out slot))
                {
                     //Add one more item
                     InventorySlots[slot].AddOne();
                }
                else
                {
                    //Find the item in the item list
                    Item itemi = ItemList.Instance.GetItemFromID(item.itemID);

                    //Create a new inventory slot
                    CreateInventorySlot(itemi);
                }
                if (DebugInventory)
                {
                    Debug.Log.Add("Added item to inventory");
                }

            }
            else throw new NullReferenceException("The item is not valid"); //throw an exeption
        }
        catch (NullReferenceException)
        {
            if (DebugInventory)
            {
                Debug.Log.Add("Item is not valid");
            }
            throw;
        }
        if (InventoryGUI.instance != null)
            InventoryGUI.instance.updateInventory();
    }

    /// <summary>
    /// Adds multiple items to the inventory. 
    /// </summary>
    /// <param name="item">Item to add to the inventory</param>
    /// <param name="amount">The amount of items to add</param>
    public void AddItemsToInventory(Item item, int amount)
    {
        int slot = 0;

        //Does the item already exist in the inventory
        if (FindItems(item.itemID, out slot))
        {
            //The amount of items after adding the amount
            int totalAmount = InventorySlots[slot].GetAmount() + amount;

            //Is total amount more than the maximum amount of items that can be in a slot
            if (totalAmount <= InventorySlots[slot].GetMaxStack())
            {
                InventorySlots[slot].AddItems(amount);
            }
            else
            {
                //The amount that can be added to the inventory
                int newAmount = totalAmount - InventorySlots[slot].GetMaxStack();

                //Add the new amount
                InventorySlots[slot].AddItems(newAmount);

                //Can more slots be added
                if (CanAddSlots())
                {
                    //Create a new slot
                    CreateInventorySlot(item, amount - newAmount);
                }
                else
                {
                    throw new Exception("Max amount of inventory slots reached");
                }
            }
        }
        else
        {
            if (CanAddSlots())
            {
                //Create a new inventory slot
                CreateInventorySlot(item, amount);
            }
            else
            {
                throw new Exception("Max amount of inventory slots reached");
            }
        }
        if(InventoryGUI.instance != null)
        InventoryGUI.instance.updateInventory();
    }

    /// <summary>
    /// Finds the slot of an item in the inventory using itemID. 
    /// Returns true if item was found
    /// </summary>
    /// <param name="ID">The ID of the item you want to find</param>
    /// <param name="slot">The slot where the item was found</param>
    /// <returns>Returns was an item found</returns>
    bool FindFromInventory(int ID, out int slot)
    {
        //Are there items in the inventory
        if (InventorySlots.Count > 0)
        {
            //Go through the inventory
            for (int i = 0; i < InventorySlots.Count; i++)
            {
                //If the id of the item in slot i matches the wanted one
                if (InventorySlots[i].GetID() == ID)
                {
                    //return i
                    slot = i;
                    return true;
                }
            }
        }
        if (DebugInventory)
        {
            Debug.Log.Add("Item Not found");
        }
        slot = -1;
        return false;
    }

    /// <summary>
    /// Finds the slot of an item in the inventory using itemID. 
    /// Returns true if item was found
    /// </summary>
    /// <param name="ID">The ID of the item you want to find</param>
    /// <param name="slot">The slot where the item was found</param>
    /// <returns>Returns was an item found</returns>
    bool FindItems(int ID, out int slot)
    {
        //Are there items in the inventory
        if (InventorySlots.Count > 0)
        {
            //Go through the inventory
            for (int i = 0; i < InventorySlots.Count; i++)
            {
                //If the id of the item in slot i matches the wanted one
                if (InventorySlots[i].GetID() == ID)
                {
                    //return i
                    slot = i;

                    if(InventorySlots[i].CanStack())
                    {
                        return true;
                    }
                }
            }
        }
        if (DebugInventory)
        {
            Debug.Log.Add("Item Not Found");
        }
        slot = -1;
        return false;
    }

    /// <summary>
    /// Remove an item from the inventory using a gameobject
    /// </summary>
    /// <param name="item">Gameobject to remove from the inventory</param>
    public void RemoveFromInventory(Item item)
    {
        try
        {

            //Testing int
            int i = 0;

            //Is there an item of this type in the inventory
            if (FindFromInventory(item.itemID, out i) || i > -1)
            {
                //Are there more than one of that item type
                if (OverOne(InventorySlots[i]))
                {
                    //Reduce the amount
                    InventorySlots[i].RemoveOne();

                    //Add the item to the world
                    //GameObject o = Instantiate(item.gameObject);
                    //o.SetActive(true);
                }
                else
                {
                    //Activate the item
                    //item.SetActive(true);
                    //Remove the item from the inventory
                    InventorySlots.RemoveAt(i);
                }
            }
            else
            {
                throw new Exception("The Item to remove was not found in the inventory");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Remove an item from the inventory using a slot number
    /// </summary>
    /// <param name="slot">The Slot to remove from</param>
    public void RemoveFromInventory(int slot)
    {
        try
        {
            //Does the slot exist
            if (InventorySlots.Count - 1 >= slot)
            {
                //Is there more than one item  
                if (OverOne(InventorySlots[slot]))
                {
                    //Remove one item from the slot
                    InventorySlots[slot].RemoveOne();

                    //Create a gameobject from the item
                    //GameObject g = Instantiate(InventorySlots[slot].GetItem().Object);
                    //g.SetActive(true);
                    if (DebugInventory)
                    {
                        Debug.Log.Add("Removed Item");
                    }
                }
                else
                {
                    //Remove the inventory slot
                    //InventorySlots[slot].GetItem().Object.SetActive(true);
                    InventorySlots.RemoveAt(slot);
                    if (DebugInventory)
                    {
                        Debug.Log.Add("Removed Slot");
                    }
                }
            }
            else
            {
                if (DebugInventory)
                {
                    Debug.Log.Add("Remove Failed");
                }
                throw new Exception("Inventory slot to remove from is not valid");
            }

            //Is the item an armor item
            ArmorItem a = InventorySlots[slot].GetItem() as ArmorItem;

            //Is the item equipped
            if(a != null && a.Equipped)
            {
                //Remove the item in the slot of the item
                ArmorSlots[(int)a.type].RemoveItem();
            }
        }
        catch (Exception e)
        {
            if (DebugInventory)
            {
                Debug.Log.Add(e.Message);
            }
            throw;
        }
    }

    /// <summary>
    /// Remove multiple items from a slot. 
    /// Should be used only when the amount of items in the slot is known
    /// </summary>
    /// <param name="slot">The slot to remove the items</param>
    /// <param name="Amount">The amount of items to remove</param>
    public void RemoveFromInventory(int slot, int Amount)
    {
        try
        {
            //Does the slot exist
            if (InventorySlots.Count - 1 >= slot)
            {
                //Are there less items in the slot than are being removed
                if (InventorySlots[slot].GetAmount() < Amount)
                {
                    throw new Exception("Amount of items to remove is greater than the amount of items in the slot");
                }
                else
                {
                    //Is there more items in the slot than are removed 
                    if (InventorySlots[slot].GetAmount() > Amount)
                    {
                        //Remove the amount of items from the slot
                        InventorySlots[slot].RemoveItems(Amount);

                        if (DebugInventory)
                        {
                            Debug.Log.Add("Removed Items");
                        }
                    }
                    else
                    {
                        //Remove the inventory slot
                        InventorySlots.RemoveAt(slot);
                        if (DebugInventory)
                        {
                            Debug.Log.Add("Removed Slot");
                        }
                    }
                }
            }
            else
            {
                if (DebugInventory)
                {
                    Debug.Log.Add("Remove Failed");
                }
                throw new Exception("Inventory slot to remove from is not valid");
            }
        }
        catch (Exception e)
        {
            if (DebugInventory)
            {
                Debug.Log.Add(e.Message);
            }
            throw;
        }
    }

    /// <summary>
    /// Remove items from inventory
    /// </summary>
    /// <param name="item">items to remove</param>
    /// <param name="amount">amount of items to remove</param>
    public void RemoveFromInventory(Item item, int amount)
    {
        try
        {
            //Testing int
            int i = 0;

            //Is there an item of this type in the inventory
            if (FindFromInventory(item.itemID, out i) || i > -1)
            {
                //Are there more than one of that item type
                if (InventorySlots[i].GetAmount() > amount)
                {
                    //Reduce the amount of items in the inventory
                    InventorySlots[i].RemoveItems(amount);

                }
                else
                {
                    RemoveSlot(i);
                }
            }
            else
            {
                throw new Exception("The Item to remove was not found in the inventory");
            }
        }
        catch (Exception e)
        {
            Debug.Log.Add(e.Message);
        }
    }

    /// <summary>
    /// Removes a slot from inventory
    /// </summary>
    /// <param name="slot"></param>
    public void RemoveSlot(int slot)
    {
        //Remove the slot
        InventorySlots.RemoveAt(slot);
        if (InventoryGUI.instance != null)
        {
            //Update the inventory gui 
            InventoryGUI.instance.updateInventory();
        }
    }

    /// <summary>
    /// More than one item in the inventroy slot
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    static bool OverOne(InventorySlot slot)
    {
        return slot.GetAmount() > 1;
    }

    /// <summary>
    /// Creates a new inventory slot
    /// </summary>
    /// <param name="item">Item to add to the slot</param>
    /// <return>returns the index of the created slot</return>
    int CreateInventorySlot(Item item)
    {
        if(DebugInventory)
        {
            Debug.Log.Add("Slot Created");
        }
        InventorySlot itemToAdd = new InventorySlot(item);
        if (InventorySlots.Count > 0)
        {
            itemToAdd.SetIndex(InventorySlots.Count);
        }
        else itemToAdd.SetIndex(0);
        InventorySlots.Add(itemToAdd);
        return InventorySlots.Count - 1;
    }

    /// <summary>
    /// Creates a new inventory slot.
    /// </summary>
    /// <param name="item">The item to put to the slot</param>
    /// <param name="Amount">The amount of items to put to the slot</param>
    /// <returns></returns> 
    int CreateInventorySlot(Item item, int Amount)
    {
        //Create a new slot with amount and items
        InventorySlot itemToAdd = new InventorySlot(item, Amount);

        //Set the index of the slot
        itemToAdd.SetIndex(InventorySlots.Count);

        //Add the slot to the inventory
        InventorySlots.Add(itemToAdd);
        return InventorySlots.Count - 1;
    }

    /// <summary>
    /// Creates an inventoryslot that has no item in it
    /// </summary>
    void CreateEmptyInventorySlot()
    {
        //Create a new slot and add it to the inventory
        InventorySlot i = new InventorySlot();
        InventorySlots.Add(i);
    }

    /// <summary>
    /// Gets the item component of the object
    /// </summary>
    /// <param name="Object">object</param>
    /// <returns></returns>
    public static Item GetItem(GameObject Object)
    {
        return Object.GetComponent<Item>();
    }

    /// <summary>
    /// Gets the maximun amount of inventory slots of the inventory
    /// </summary>
    /// <returns>Returns MaxSize</returns>
    public int GetMaxSize()
    {
        return MaxSize;
    }

    /// <summary>
    /// Gets the item that is in the slot
    /// </summary>
    /// <param name="slot">The slot where to get the item</param>
    /// <returns></returns>
    public Item GetItemFromSlot(int slot)
    {
        return InventorySlots[slot].GetItem();
    }

    /// <summary>
    /// Uses the item in a slot
    /// </summary>
    /// <param name="slot">the slot which item to use</param>
    public void UseItem(int slot)
    {
        //If the item is a backpack
        if(InventorySlots[slot].GetItem().ItemCategory == Category.Backpack)
        {
            //Increase the level of the inventory
            SetInventoryLevel(inventoryLevel + 1);

            //Remove the slot
            RemoveSlot(slot);
            return;
        }
        //Uses the item in the slot
        InventorySlots[slot].UseItemInSlot();

        //Is the item consumed
        if (InventorySlots[slot].IsConsumed())
        {
            //Are there more than one item in the slot
            if (OverOne(InventorySlots[slot]))
            {
                //Remove one of the items in the slot
                InventorySlots[slot].RemoveOne();
            }
            else
            {
                //Remove the slot
                RemoveSlot(slot);
            }
        }
    }

    /// <summary>
    /// Gets the inventory
    /// </summary>
    /// <returns></returns>
    public List<InventorySlot> GetInventory()
    {
        return InventorySlots;
    }

    /// <summary>
    /// Can more slots be added to the inventory
    /// </summary>
    /// <returns></returns>
    public bool CanAddSlots()
    {
        return InventorySlots.Count < MaxSize;
    }

    /// <summary>
    /// Changes the items in the slots
    /// </summary>
    /// <param name="a">Slot one</param>
    /// <param name="b">Slot two</param>
    void ChangeSlots(int a, int b)
    {
        InventorySlot i = InventorySlots[a];
        InventorySlots[a] = InventorySlots[b];
        InventorySlots[b] = i;
    }

    /// <summary>
    /// Sets the level of the inventory
    /// </summary>
    /// <param name="level">The level</param>
    public void SetInventoryLevel(int level)
    {
        inventoryLevel = level;
        MaxSize = 15 + (level - 1) * 6;
    }
    
    /// <summary>
    /// The level of the inventory
    /// </summary>
    /// <returns></returns>
    public int GetInventoryLevel()
    {
        return inventoryLevel;
    }

    /// <summary>
    /// Gets the save information from the inventory
    /// </summary>
    /// <returns>Hello</returns>
    public string InventoryToString()
    {
        //Create a string and add the beginning to it
        string rtn = "[";

        //Iterate all slots
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            //Add the item string to the string
            rtn += InventorySlots[i].GetItemString();

            //If this isnt the last slot add a thingy
            if (i < InventorySlots.Count - 1)
                rtn += "|";
        }
        //Add a different thingy to the end of the string
        rtn += "]";
        return rtn;
    }

    /// <summary>
    /// Creates an inventory from a string
    /// </summary>
    /// <param name="Data">The string</param>
    public void BuildFromString(string Data)
    {
        if(Data == null)
        {
            return;
        }

        //Create a emtpy inventory
        List<InventorySlot> newInventory = new List<InventorySlot>(); 

        //The last character that was processed
        char LastChar = 'I';

        //The Slot to be added
        InventorySlot LastSlot = new InventorySlot();

        //The item that will be added
        Item I = null; 

        //Go through the data
        for (int i = 0; i < Data.Length; i++)
        {
            //The loop has reached the end of the Inventory
            if(Data[i] == ']')
            {
                break;
            }
            //Is the character the start of a new index
            if (LastChar == '|' || LastChar == '[')
            {
                //The id of the index
                int id = -1;

                //Find the end point of the id
                for (int a = i; a < Data.Length; a++)
                {
                    //The end point
                    if (Data[a] == ':')
                    {
                        //Get the id
                        string b = Data.Substring(i, a - i);
                        id = int.Parse(b);

                        //Stop the loop
                        break;
                    }
                }
                
                //Get the item list and find the item using the id
                I = ItemList.Instance.GetItemFromID(id);
            }
            //Is the last char the start of amount
            else if(LastChar == ':')
            {
                //Default amount
                int am = 1;

                //Find the end of the amount
                for (int c = i; c < Data.Length; c++)
                {
                    //The end of the amount
                    if (Data[c] == '|' || Data[c] == ']')
                    {
                        //Get the amount
                        string b = Data.Substring(i, c - i);
                        am = int.Parse(b);

                        //Stop the loop
                        break;
                    }
                }
                //Create a new inventory slot with the item I and amount am
                LastSlot = new InventorySlot(I, am);
                
                //Add the item to the new inventory
                newInventory.Add(LastSlot);
            }
            //Make current char the last char
            LastChar = Data[i];
        }

        //Set the inventory to the new inventory
        InventorySlots = newInventory;
    }

    #region FilterFuctions
    /// <summary>
    /// Get a list of Inventory slots filtered by a category
    /// </summary>
    /// <param name="category">The category, if none returns all slots</param>
    /// <returns></returns>
    public List<InventorySlot> GetFilteredList(Category category)
    {
        //Create an empty list
        List<InventorySlot> testList = new List<InventorySlot>();

        //Iterate through the inventory
        foreach (InventorySlot i in InventorySlots)
        {
            //If the wanted category is none or if the category of the slot i matches the wanted category
            if (category == Category.None || i.GetItem().ItemCategory == category)
            {
                //Add the slot i to the filtered list
                testList.Add(i);
            }
            
        }
        //Return the new list
        return testList;
    }

    /// <summary>
    /// Get a list of all Inventory slot filered by two categories. 
    /// if eiter category is set to none returns all inventory slots
    /// </summary>
    /// <param name="category1">The first category</param>
    /// <param name="category2">The second category</param>
    /// <returns></returns>
    public List<InventorySlot> GetFilteredList(Category category1, Category category2)
    {
        //Create an empty list
        List<InventorySlot> testList = new List<InventorySlot>();

        //Iterate through the inventory
        foreach (InventorySlot i in InventorySlots)
        {
            //If the wanted category is none or if the category of the slot i matches the wanted category
            if (category1 == Category.None || category2 == Category.None || i.GetItem().ItemCategory == category1 || i.GetItem().ItemCategory == category2)
            {
                //Add the slot i to the filtered list
                testList.Add(i);
            }

        }
        //Return the new list
        return testList;
    }

    /// <summary>
    /// Get a list of all inventory slots with a certain rarity 
    /// </summary>
    /// <param name="rarity">wanted rarity</param>
    /// <returns></returns>
    public List<InventorySlot> GetFilteredList(Rarity rarity)
    {
        //Create an empty list
        List<InventorySlot> testList = new List<InventorySlot>();

        //Iterate through the inventory
        foreach (InventorySlot i in InventorySlots)
        {
            //Is the item of the wanted rarity
            if (i.GetItem().ItemRarity == rarity)
            {
                //Add the slot i to the filtered list
                testList.Add(i);
            }

        }
        //Return the new list
        return testList;
    }

    /// <summary>
    /// Gets a list of inventory slots filtered by rarities
    /// </summary>
    /// <param name="rarity1">First rarity</param>
    /// <param name="rarity2">Second rarity</param>
    /// <returns></returns>
    public List<InventorySlot> GetFilteredList(Rarity rarity1, Rarity rarity2)
    {
        //Create an empty list
        List<InventorySlot> testList = new List<InventorySlot>();

        //Iterate through the inventory
        foreach (InventorySlot i in InventorySlots)
        {
            //Is the item of the wanted rarity
            if (i.GetItem().ItemRarity == rarity1 || i.GetItem().ItemRarity == rarity2)
            {
                //Add the slot i to the filtered list
                testList.Add(i);
            }
        }
        //Return the new list
        return testList;
    }

    /// <summary>
    /// Get a list of all inventory slots that have potions of wanted type in them
    /// </summary>
    /// <param name="type">Type of potions wanted</param>
    /// <returns></returns>
    public List<InventorySlot> GetPotionsOfType(PotionType type)
    {
        //Create an empty list
        List<InventorySlot> testList = new List<InventorySlot>();

        //Iterate through inventory filtered by category potion
        foreach (InventorySlot i in GetFilteredList(Category.Potion))
        {
            //Get the potion item in the inventory slot
            PotionItem p = i.GetItem() as PotionItem;

            //If the potion is of wanted type
            if(p && p.potionType == type)
            {
                //add the slot to the test list
                testList.Add(i);
            }
        }

        //Return the test list
        return testList;
    }

    /// <summary>
    /// Gets the original indexes of the items in a filtered inventory
    /// </summary>
    /// <param name="filteredInventory"></param>
    /// <returns></returns>
    public List<int> GetFilterIndexes(List<InventorySlot> filteredInventory)
    {
        //Create a temporary list
        List<int> testList = new List<int>();

        //Loop through the inventory
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            if(filteredInventory.Contains(InventorySlots[i]))
            {
                testList.Add(i);
            }
        }

        return testList;
    }
    #endregion

    /// <summary>
    /// Gets a string that contains all item id's of the items that the player has equipped
    /// </summary>
    /// <returns></returns>
    public string GetEquippedItems()
    {
        //The inventory level and the starting char if the items
        string rtn = inventoryLevel.ToString() + "(";

        //Iterate the armor slots
        for (int i = 0; i < ArmorSlots.Count; i++)
        {
            //Add the armor slots ID
            rtn += ArmorSlots[i].ID.ToString();

            //If its not the last armor slot add a thing
            if(i < ArmorSlots.Count -1)
            {
                rtn += '|';
            }

            //Else add an end the armor thing
            else
            {
                rtn += ')';
            }
        }
        //Is a weapon equipped
        try
        {

            //Get the Weapons ID
            rtn += CurrentWeapon.itemID.ToString() + ']';

            //Get the shield ID
            rtn += CurShield.itemID.ToString() + '[';
        }
        catch { }

            //Return the string rtn
            return rtn;
    }

    /// <summary>
    /// Sets the equipped items using a string
    /// </summary>
    /// <param name="Data"></param>
    public void SetEquippedItems(string Data)
    {
        //Start the coroutine to equip the items internally
        StartCoroutine(SetEquippedItems_Internal(Data));
    }

    /// <summary>
    /// The internal method that handles equipping items
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    IEnumerator SetEquippedItems_Internal(string Data)
    {
        //Disable equipped item graphics
        try
        {
            SwordGraphics.SetActive(false);
            KatanaGraphics.SetActive(false);
            shieldGraphics.SetActive(false);
        }
        catch
        {
            Debug.Log.Add("Something off with graphics");
        }

        //Wait for end of frame
        yield return new WaitForEndOfFrame();
        //The other string
        string thing = "";

        Debug.Log.Add(Data);
        //If Data is null return
        if (Data == null)
        {
            yield break;
        }

        //The Number of the current slot
        int slotNum = 0;

        //Iterate through the chars in the string
        for (int i = 0; i < Data.Length; i++)
        {
            //If the char is a number
            if (Data[i] != '(' && Data[i] != '|' && Data[i] != ')' && Data[i] != ']' && Data[i] != '[')
            {
                //Add it to the secondary string
                thing += Data[i];
            }
            else if(Data[i] == '(')
            {
                int level = int.Parse(thing);

                if (level > 0)
                {
                    inventoryLevel = level;
                }
                //Reset the other string
                thing = null;
            }

            //If the char is not a number
            else if (Data[i] == '|' || Data[i] == ')')
            {
                //Get the string as int
                int id = int.Parse(thing);

                //Is the id valid
                if (id > 0)
                {
                    //Set the item in the armor slot
                    ArmorSlots[slotNum].SetItem((ArmorItem)ItemList.Instance.GetItemFromID(id));
                }
                //Reset the other string
                thing = null;

                //Increment the slot number
                slotNum++;
            }
            //If the char is ]
            else if (Data[i] == ']')
            {
                //Get the string as int
                int id = int.Parse(thing);

                //Assingn the current weapon
                SetCurrentWeapon((WeaponItem)ItemList.Instance.GetItemFromID(id));

                //Reset the other string
                thing = null;

                //Debug.Log.Add(GetEquippedItems());
            }
            else if(Data[i] == '[')
            {
                int id = int.Parse(thing);
                
                //Assing the current shield
                SetCurrentShield((ShieldItem)ItemList.Instance.GetItemFromID(id));

                //Reset the other string
                thing = null;
            }

        }
    }

    /// <summary>
    /// Sets the currently equipped weapon
    /// </summary>
    /// <param name="weapon">The weapon to equip</param>
    public void SetCurrentWeapon(WeaponItem weapon)
    {

        //Set the current weapon to weapon
        CurrentWeapon = weapon;

        //Update the player damage in the stats
        try
        {
            //If the item is a katana
            if (weapon.itemID == 9)
            {
                //Acitvate katana graphics
                KatanaGraphics.SetActive(true);

                //Disable sword graphics
                SwordGraphics.SetActive(false);
            }

            //If the item is sword
            else if(weapon.itemID == 11)
            {
                //Disable katana graphics
                KatanaGraphics.SetActive(false);

                //Activate sword
                SwordGraphics.SetActive(true);
            }
            else
            {
                //Disable both
                KatanaGraphics.SetActive(false);
                SwordGraphics.SetActive(false);
            }

            //Update damage
            Motor.instance.stats.PlayerDamage = Motor.instance.stats.BaseDamage + weapon.Damage;

            //Update stats
            UpdateBasicStats.Instance.UpdateDamage();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Sets the current shield
    /// </summary>
    /// <param name="shield"></param>
    public void SetCurrentShield(ShieldItem shield)
    {
        //Set the shield
        CurShield = shield;

        //Sets the shield graphics active
        Debug.Log.Add(GetEquippedItems());

        //Activate graphics
        shieldGraphics.SetActive(true);
    }

    /// <summary>
    /// Gets the currently equipped weapon
    /// </summary>
    /// <returns></returns>
    public WeaponItem GetWeapon()
    {
        return CurrentWeapon;
    }

    /// <summary>
    /// Gets the currently equipped shield
    /// </summary>
    /// <returns></returns>
    public ShieldItem GetShield()
    {
        return CurShield;
    }

    /// <summary>
    /// Gets the damage of the currently equipped weapon
    /// </summary>
    /// <returns></returns>
    public int GetWeaponDamage()
    {
        //Is there a weapon
        if(CurrentWeapon != null)
        {
            return CurrentWeapon.Damage;
        }
        else return 0;

    }

    /// <summary>
    /// Get the total amount of armor in all of the armor slot
    /// </summary>
    /// <returns></returns>
    public int GetTotalArmor()
    {
        try
        {
            //Create a temporary int a
            int a = 0;
            //Go through all armor slots
            foreach (ArmorSlot slot in ArmorSlots)
            {
                //Add the slots armor to a
                a += slot.GetSlotArmor();
            }
            return a;
        }
        catch
        {
            //If that fails return 0
            return 0;
        }
    }

    /// <summary>
    /// Get the equipped armor item of a type 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    ArmorItem GetEquippedArmorOfType(ArmorType type)
    {
        return ArmorSlots[(int)type].GetItem();
    }

    /// <summary>
    /// Equip an armor item 
    /// </summary>
    /// <param name="Item">Item to equip</param>
    public void EquipArmorItem(ArmorItem Item)
    {
        //Set the item in the slot type of the item
        ArmorSlots[(int)Item.type].SetItem(Item);
    }

    /// <summary>
    /// Unequip a piece of armor
    /// </summary>
    /// <param name="type"></param>
    public void UnequipArmorItem(ArmorType type)
    {
        ArmorSlots[(int)type].RemoveItem();
        UpdateBasicStats.Instance.UpdateArmor();
    }

    /// <summary>
    /// Gets the display information of equipped armor item
    /// </summary>
    /// <param name="type">The armor type</param>
    /// <returns></returns>
    public ArmorDisplayInformation GetArmorDisplayInformation(ArmorType type)
    {
        return GetArmorDisplayInformation_Internal(GetEquippedArmorOfType(type));
    }

    /// <summary>
    /// Gets the display information of a piece of armor
    /// </summary>
    /// <param name="armor"></param>
    /// <returns></returns>
    ArmorDisplayInformation GetArmorDisplayInformation_Internal(ArmorItem armor)
    {
        //Is the armor not null
        if (armor)
        {
            //Return the armor display info
            return new ArmorDisplayInformation(armor.DisplayName, armor.ico, armor.ArmorValue);
        }
        //Create a empty display info
        return new ArmorDisplayInformation("Vapaa", null, 0);
    }
}
