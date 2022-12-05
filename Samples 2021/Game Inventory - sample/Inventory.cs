using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script creator Jonathan Muroma

public class Inventory : MonoBehaviour
{
    //Here we make Inventory into a Singleton class. (Press the plus on the left of it to open and collapse the code).
    #region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("More than one instance of inventory. It has been destroyed.");
            return;
        }
        instance = this; //We set our Inventory as the one we want to exist.
        //GameObject.DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    //We create a Delegate method which is called everytime changes happen to the items in our inventory.
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;
    public bool invFilled = false;

    [SerializeField]
    private int inventorySpace = 20; //This is the size of our inventory. by default it holds 20 items. Can be set through the inspector, which Iäve made to hold 18 items.

    public List<Item> items = new List<Item>(); //A list of all of the items our inventory holds.

    public bool AddItem(Item item) //Adds an item to the players inventory. returns a boolean, which is used to confirm if the item was added or not (for example if our inventory would be full, we would not add it).
    {
        if(items.Count >= inventorySpace) //We check if the inventory has space for the item
        {
            return false; //If there is not enough space, we return false and don't add it to the inventory.
        }
        items.Add(item); //We add the item to our items list.

        if(onItemChangedCallBack != null) //We check if our delegate method holds any methods which should be ran.
        {
            onItemChangedCallBack.Invoke(); //We run all the methods which onItemChangedCallBack holds.
        } 
        return true; //We return true, because the item was added to our inventory.
    }

    public void RemoveItem(Item item) //Called when we want to remove an item from the inventory. 
    {
        items.Remove(item); //Removes the given item from the items list.

        if (onItemChangedCallBack != null) //We check if our delegate method holds any methods which should be ran.
        {
            onItemChangedCallBack.Invoke(); //We run all the methods which onItemChangedCallBack holds.
        }
    }

    public void ReplaceItem(Item itemInList, Item replaceItem) //This is used when combining items by the InventorySlot. It replaces the combined item with the result of the combination.
    {
        items[items.IndexOf(itemInList)] = replaceItem; //Checks the index of the item we are combining to, and replaces it with the result of the combination.
    }
}
