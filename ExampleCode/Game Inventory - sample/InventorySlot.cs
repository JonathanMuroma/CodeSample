using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Script creator Jonathan Muroma

public class InventorySlot : MonoBehaviour,/* IPointerDownHandler,*/ IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Item item; //The item that gets assigned to it for it to hold and use.

    [SerializeField]
    private Image icon = null; //The button icon this inventoryslot holds. Used to place own items specific icon to it.

    [SerializeField]
    private GameObject itemTextBox = null; //Inventorys itemTextBox gameObject which we use to get the itemTextBoxController component from it.
    private ItemTextBoxController itemTextBoxController; //We use this to access functions of itemTextBox gameObject, which has a itemTextBoxController script component.

    //Doubleclick related variables
    private float firstClickTime, timeBetweenClicks;
    private bool coroutineAllowed;
    public int clickCounter; //change to private?
    private float doubleClickTimer;

    //Drag&Drop combining related variables
    private Inventory inventory = null; //Used to hold our Inventory singleton instance,
    private Image cursorDragImage = null;
    private Transform cursorDragImageHolder = null;
    private Canvas inventoryCanvas = null;
    //Printing combining failed message variable
    private PrintCombineFailMessage printCombineFailMessage = null;
    private Item dragItemHolder = null; //This holds a dublicate of the item we are dragging. Used to see if we were dragging an item. Necessary for removing the dragged item image, since the original will be destroyd when combining.

    void Start()
    {
        itemTextBoxController = itemTextBox.GetComponent<ItemTextBoxController>(); //Here we assign the itemTextBoxs itemTextBoxController script component for us to use.

        //Doubleclick related variable being assigned.
        firstClickTime = 0.0f;
        timeBetweenClicks = 0.25f;
        clickCounter = 0;
        coroutineAllowed = true;

        //Drag&Drop combining related being assigned.
        inventory = Inventory.instance;
        cursorDragImageHolder = this.transform.parent.transform.parent.transform.Find("CursorDragImageHolder");
        inventoryCanvas = cursorDragImageHolder.parent.parent.gameObject.GetComponent<Canvas>();
        printCombineFailMessage = cursorDragImageHolder.parent.Find("CombineFailMessageCanvas").GetComponent<PrintCombineFailMessage>();
        if (cursorDragImageHolder == null)
            Debug.Log("CursorDragImageHolder not found...");
        if (inventoryCanvas == null)
            Debug.Log("InventoryCanvas was not found....");
        if (printCombineFailMessage == null)
            Debug.Log("PrintCombineFailMessage was not found...");
    }

    void Update()
    {
        //Doubleclick stuff
        if (clickCounter >= 1) //If Doubleclick is activated by adding 1 to clickCounter, (see UseItem() method below), we start checking time with our timer variable.
        {
            doubleClickTimer += Time.unscaledDeltaTime; //We keep adding to our timer with unscaledDeltaTime (unscaledDeltaTime doesn't freeze during pause).
            if (coroutineAllowed) //This is to make sure doubleclick Coroutine is ran alone each time, so that there won't be multiple Coroutines at once running at the same time for no reason.
            {
                firstClickTime = doubleClickTimer; //We "time stamp" our first time we click.
                StartCoroutine(DoubleClickDetection()); //Here we start our Curutine DoubleClickDetection, which is down below.
            }
        }
    }

    public void AddItem(Item newItem) //Used when we want assign an item to an inventorySlot.
    {
        item = newItem; //Takes the item and assigns it for itself.
        icon.sprite = item.icon; //We take the items icon and assign it to our buttons icon.
        icon.enabled = true; //We enable the icon to show the item which inventorySlot is holding.
    }

    public void ClearSlot() //Used when we want to remove item from an inventorySlot.
    {
        //We empty all the variables by setting them null so that "no history is left" and disable the item icon, since we have none.
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem() //When an inventorySlot is clicked in the inventory, useItem() is called. 
    {
        //DoubleClick starts here. 
        //If we have an item, we start our double click action by adding to our click Counter by 1.
        if (item != null)
        {
            clickCounter++;
        };
    }

    public void PrintItem() //This method is called when an inventorySlot is highlighted by the mouse. It is used to print info about the highlighted item to inventoryUI itemTextBox. ItemButtonHighlight script calls this method.
    {
        if (item != null) //If inventorySlot holds an item.
        {
            itemTextBoxController.PrintItemOnUI(item); //We use itemTextBoxController, component of itemTextBox, to send the item we want it to print on the inventoryUI itemTextBox area.
        }
    }

    public void StopPrintItem() //This method is called when we want to stop printing to the itemTextBox. (When we use the item, or dehighlight it). ItemButtonHighlight script calls this method, when we dehighlight the inventorySlot. 
    {
        itemTextBoxController.StopPrintItemOnUI(); // We use itemTextBoxController, component of itemTextBox, to tell itemTextBox to stop printing on the inventoryUI itemTextBox area.
    }

    //Doubleclick methods etc.
    private IEnumerator DoubleClickDetection() //Coroutine called DoubleClickDetection, which is used to detect if we have double clicked an inventorySlot.
    {
        coroutineAllowed = false; //We set coroutineAllowed to false, so that there won't be multiple coroutines of DoubleClickDetection running at the same time. Only One!
        while (doubleClickTimer < (firstClickTime + timeBetweenClicks)) //We keep looping the duration have given to wait for a second click.
        {
            if (clickCounter >= 2) //If a second click has happened under our time limit.
            {
                item.Use(); //We call the item (which the inventorySlot holds), to do its Use() method.
                StopPrintItem(); //We tell to print the item on itemTextBox, so that the text won't linger on after the item has been removed from the inventory.
                PrintItem(); //We immediately print again, so that if a new item has moved to replace the used item, we print its information to itemTextBox.
                break; //We break out of the While loop, since there is no reason to keep expecting additional clicks etc.
            }
            yield return new WaitForEndOfFrame(); //We tell our Coroutine to continnue at the end of next frame.
        }
        //Here we reset every variable related to our DoubleClick.
        clickCounter = 0;
        firstClickTime = 0.0f;
        coroutineAllowed = true;
    }

    public Item GetItem() //Method for getting the item the InventorySlot holds. ItemButtonHighlight uses this to check if the InventorySlot currently holds an item.
    {
        return item;
    }

    public void TryToCombine(Item givenItem) //This method combines the items and takes care of all the things related to it.
    {
        Item newItem = item.Combine(givenItem); //We use the method Combine, which all the items have, to see if the given item (in the cursor) can be combined with this highlighted InventorySlots item. It returns a null or a the result of the combining. We then assign the result in a new Item variable.

        if (newItem != null) //We check if the combining of items was succesful. If yes we do the necessary steps.
        {
            Debug.Log("Combine compatible items. COMBINING...");

            inventory.ReplaceItem(item, newItem);    //Replaces the highlighted item with new combined item.
            inventory.RemoveItem(givenItem); //Removes the item which was dragged from the inventory. (The one which was clicked and drag/cursor holds)
        }
        else //If the items couldn't be combined we send a message to the screen that, combining these two items is not possible
        {
            //Debug.Log("Cannot combine these...");
            CombiningFailedMessage(); //Method used to print a message on screen, when items couldn't be combined.
        }
    }

    public void CombiningFailedMessage() //Method used to print a message on screen, when items couldn't be combined.
    {
        printCombineFailMessage.StartPrintingMessage(); //We run a method in CombineFailMessageCanvas, which does the message printing.
    }

    public void OnBeginDrag(PointerEventData eventData) //When we click and hold and start dragging this method starts.
    {

        if(item != null) //We check if we have an item.
        {
            Debug.Log("BeginDrag");
            dragItemHolder = item;
            cursorDragImage = Instantiate(icon, this.transform.position, this.transform.rotation, cursorDragImageHolder); //We instantiate an Image which will be moved by the cursor. We also assigne it to an Image variable for additional use.
            cursorDragImage.raycastTarget = false; //We disable raycastTarget on the instantiated Image, so that it won't block cursors Raycast to Canvas.
            cursorDragImage.transform.GetComponent<CanvasGroup>().blocksRaycasts = false; //We disable raycastTarget on the instantiated Image, so that it won't block cursors Raycast to Canvas.
            cursorDragImage.transform.GetComponent<CanvasGroup>().alpha = 0.6f; //We put some transparency on the draggable Image.
        }
    }

    public void OnDrag(PointerEventData eventData) //This method happens during the dragging action.
    {
        if (dragItemHolder != null) //We check if we have an item.
        {
            Debug.Log("Dragging");
            cursorDragImage.rectTransform.anchoredPosition += eventData.delta / inventoryCanvas.scaleFactor; //Here we move the image, by adding the cursors movement to it. The cursors movement must be divided by the canvas scaleFactor, for it to move correctly.
        }
    }


    public void OnEndDrag(PointerEventData eventData) //This method is run, when we let go of the MouseButton and stop dragging.
    {
        //Debug.Log("EndDrag");
        if (dragItemHolder != null) //We check if we have an item.
        {
            Debug.Log("EndDrag");
            dragItemHolder = null;
            Destroy(cursorDragImage.gameObject); //We destroy the dragable Image we Instantiated.
        }
    }

    public void OnDrop(PointerEventData eventData) //This method is run, when we are on top of an IventorySlot when we let the MouseButton go and stopDragging.
    {
        if (item != null && eventData.pointerDrag.GetComponent<InventorySlot>().GetItem() != null && eventData.pointerDrag.GetComponent<InventorySlot>().GetItem() != item) //We check if the InventorySlot we are dropping to has an item. That the InventorySlot we clicked has an item and that it isn't the same InventorySlot we started dragging.
        {
            Debug.Log("Dropped it on something...");
            TryToCombine(eventData.pointerDrag.GetComponent<InventorySlot>().GetItem()); //We send the item which the cursor holds to be combined.
        }
    }
}