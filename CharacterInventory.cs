using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    #region Variable Declarations
    public static CharacterInventory instance;

    public CharacterStats charStats;

    public Image[] hotBarDisplayHolders = new Image[4];
    public GameObject InventoryDisplayHolder;
    public Image[] inventoryDisplaySlots = new Image[30];

    int inventoryItemCap = 20;
    int idCount = 1;
    bool addedItem = true;

    public Dictionary<int, InventoryEntry> itemsInInventory = new Dictionary<int, InventoryEntry>();
    public InventoryEntry itemEntry;

    #endregion

    #region Initializations
    void Start()
    {
        instance = this;

        itemEntry = new InventoryEntry(0, null, null);
        itemsInInventory.Clear();

        inventoryDisplaySlots = InventoryDisplayHolder.GetComponentsInChildren<Image>();

        charStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
    }
    #endregion

    private void Update()
    {
        #region Watch for Hotbar Keypresses - called by character controller later
        //checking for a hotbar key to be pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerItemUse(101);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerItemUse(102);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerItemUse(103);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TriggerItemUse(104);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
        #endregion

        // check to see if the item has already been added - prevent duplicate adds for 1 item
        if (!addedItem)
        {
            TryPickUp();
        }
    }

    public void StoreItem(ItemPickUp ItemToStore)
    {
        addedItem = false;

        if ((charStats.characterDefinition.currentEncumbrance + ItemToStore.itemDefinition.itemWeight) <= charStats.characterDefinition.maxEncumbrance)
        {
            itemEntry.invEntry = ItemToStore;
            itemEntry.stactSize = 1;
            itemEntry.hbSprite = ItemToStore.itemDefinition.itemIcon;

            ItemToStore.gameObject.SetActive(false);
        }
    }

    void TryPickUp()
    {
        bool itsInInv = true;

        // check to see if the item to be stored was properly submitted to the inventory and is no null - continue if Yes otherwise do nothing
        if (itemEntry.invEntry)
        {
            // check to see if any items exist in the inventory already - if not, add this item
            if (itemsInInventory.Count == 0)
            {
                addedItem = AddItemToInv(addedItem);
            }
            // if items exist in inventory
            else
            {
                // checks to see if the item is stackable - Continue if stackable
                if (itemEntry.invEntry.itemDefinition.isStackable)
                {
                    foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
                    {
                        // does this item already exist in inventory? Continue if yes
                        if (itemEntry.invEntry.itemDefinition == ie.Value.invEntry.itemDefinition)
                        {
                            //add 1 to stack and destroy the new instance
                            ie.Value.stactSize += 1;
                            AddItemToHotBar(ie.Value);
                            itsInInv = true;
                            DestroyObject(itemEntry.invEntry.gameObject);
                            break;
                        }
                        //if item not exist already in inventory then continue here
                        else
                        {
                            itsInInv = false;
                        }
                    }
                }
                // if item is not stackable then continue here
                else
                {
                    itsInInv = false;

                    // if no space and item is not stackable - say inventory full
                    if (itemsInInventory.Count == inventoryItemCap)
                    {
                        itemEntry.invEntry.gameObject.SetActive(true);
                        Debug.Log("Inventory is full");
                    }
                }

                //check if there is space in inventory - if yes , continue here
                if (!itsInInv)
                {
                    addedItem = AddItemToInv(addedItem);
                    itsInInv = true;
                }
            }
        }
    }

    bool AddItemToInv(bool finishedAdding)
    {
        itemsInInventory.Add(idCount, new InventoryEntry(itemEntry.stactSize, Instantiate(itemEntry.invEntry),itemEntry.hbSprite));
        charStats.characterDefinition.currentEncumbrance += itemEntry.invEntry.itemDefinition.itemWeight;

        DestroyObject(itemEntry.invEntry.gameObject);

        FillInventoryDisplay();
        AddItemToHotBar(itemsInInventory[idCount]);

        idCount = IncreateID(idCount);

        #region Reset itemEntry
        itemEntry.invEntry = null;
        itemEntry.stactSize = 0;
        itemEntry.hbSprite = null;
        #endregion

        finishedAdding = true;

        return finishedAdding;


    }

    int IncreateID(int currentID)
    {
        int newID = 1;

        for (int itemCount = 1; itemCount <= itemsInInventory.Count; itemCount++)
        {
            if (itemsInInventory.ContainsKey(newID))
            {
                newID += 1;
            }
            else
            {
                return newID;
            }
        }

        return newID;
    }

    void AddItemToHotBar(InventoryEntry itemForHotBar)
    {
        int hotBarCounter = 0;
        bool increaseCount = false;

        // check for open hotbar slot
        foreach (Image images in hotBarDisplayHolders)
        {
            hotBarCounter += 1;
            if (itemForHotBar.hotBarSlot == 0)
            {
                if (images.sprite == null)
                {
                    //add item to open hotbar slot
                    itemForHotBar.hotBarSlot = hotBarCounter;
                    //change hotbar sprite to show item
                    images.sprite = itemForHotBar.hbSprite;
                    increaseCount = true;
                    break;
                }
            }
            else if (itemForHotBar.invEntry.itemDefinition.isStackable)
            {
                increaseCount = true;
            }
        }

        if (increaseCount)
        {
            hotBarDisplayHolders[itemForHotBar.hotBarSlot - 1].GetComponentInChildren<Text>().text = itemForHotBar.stactSize.ToString();
        }

        increaseCount = false;
    }

    void DisplayInventory()
    {
        if (InventoryDisplayHolder.activeSelf == true)
        {
            InventoryDisplayHolder.SetActive(false);
        }
        else
        {
            InventoryDisplayHolder.SetActive(true);
        }
    }

    void FillInventoryDisplay()
    {
        int slotCounter = 9;

        foreach (KeyValuePair<int,InventoryEntry> ie in itemsInInventory)
        {
            slotCounter += 1;
            inventoryDisplaySlots[slotCounter].sprite = ie.Value.hbSprite;
            ie.Value.inventorySlot = slotCounter - 9;
        }

        while (slotCounter < 29)
        {
            slotCounter++;
            inventoryDisplaySlots[slotCounter].sprite = null;
        }
    }

    void TriggerItemUse(int itemToUseID)
    {
        bool triggerItem = false;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
        {
            if (itemToUseID > 100)
            {
                itemToUseID -= 100;
                if (ie.Value.hotBarSlot == itemToUseID)
                {
                    triggerItem = true;
                }
            }
            else
            {
                if (ie.Value.inventorySlot == itemToUseID)
                {
                    triggerItem = true;
                }
            }

            if (triggerItem)
            {
                if (ie.Value.stactSize == 1)
                {
                    if (ie.Value.invEntry.itemDefinition.isStackable)
                    {
                        if (ie.Value.hotBarSlot != 0)
                        {
                            hotBarDisplayHolders[ie.Value.hotBarSlot - 1].sprite = null;
                            hotBarDisplayHolders[ie.Value.hotBarSlot - 1].GetComponentInChildren<Text>().text = "0";
                        }

                        ie.Value.invEntry.UseItem();
                        itemsInInventory.Remove(ie.Key);
                        break;
                    }
                    else
                    {
                        ie.Value.invEntry.UseItem();
                        if (!ie.Value.invEntry.itemDefinition.isIndestructable)
                        {
                            itemsInInventory.Remove(ie.Key);
                            break;
                        }
                    }
                }
                else
                {
                    ie.Value.invEntry.UseItem();
                    ie.Value.stactSize -= 1;
                    hotBarDisplayHolders[ie.Value.hotBarSlot - 1].GetComponentInChildren<Text>().text = ie.Value.stactSize.ToString();
                    break;
                }
            }
        }

        FillInventoryDisplay();
    }
}