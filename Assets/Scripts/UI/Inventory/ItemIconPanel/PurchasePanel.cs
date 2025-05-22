using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject sellButton;
    [SerializeField] GameObject hideItemIconParent;

    public List<ItemIcon> playerItemIcon = new List<ItemIcon>();
    bool storeType = true;
    List<StoreItemIcon> itemList = new List<StoreItemIcon>();
    int totalPrice = 0;

    private List<ItemIcon> playerItemIconList = new List<ItemIcon>();


 

    public void InsertItem(StoreItemIcon item)
    {
        item.transform.SetParent(scrollRect.content);
        if(item.isInventoryItem) {
            Debug.Log($"test InsertItem");
            totalPrice += (item.item.itemData.price * item.item.quantity); }
        else { totalPrice += (item.itemData.price * item.itemData.maxQuantity); }
        priceText.text = totalPrice.ToString();
        itemList.Add(item);
    }

    public void RemoveItemIcon(StoreItemIcon item)
    {
        /*if (item.isInventoryItem) { ItemIcon itemIcon = InventoryController.Instance.GetCreateItemIcon(item.item); InventoryController.Instance.inventoryPanel.InsertItem(itemIcon); 
            totalPrice -= item.item.itemData.price;
            item.item = null;
        }
        else { totalPrice -= item.itemData.price; }
*/
        if(playerItemIconList.Count > 0)
        {
            InventoryController.Instance.inventoryPanel.InsertItem(item.item.itemIcon.GetComponent<ItemIcon>());
        }


        priceText.text = totalPrice.ToString();
        itemList.Remove(item);
        Destroy(item.gameObject);
        
    }

    public void SetItem(Item item)
    {
        if (storeType) { if (itemList.Count != 0) return;
            storeType = !storeType;
            purchaseButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(true);
        }
        GameObject storeItemIcon = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Inventory/Store_ItemIcon"));
        StoreItemIcon _itemIcon = storeItemIcon.GetComponent<StoreItemIcon>();
        _itemIcon.SetItem(item);
        InsertItem(_itemIcon);
    }

    public void SetItem(StoreItemIcon item)
    {
        if (!storeType) {
            if (itemList.Count != 0) return;
            storeType = !storeType;
            sellButton.gameObject.SetActive(false);
            purchaseButton.gameObject.SetActive(true) ;
        }
        GameObject storeItemIcon = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Inventory/Store_ItemIcon"));
        StoreItemIcon _itemIcon = storeItemIcon.GetComponent<StoreItemIcon>();
        _itemIcon.SetItem(item.itemData);
        _itemIcon.isPurchasePanel = true;
        _itemIcon.isInventoryItem = true;
        InsertItem(_itemIcon);
    }

    public void PurchaseButton()
    {
        if(InventoryController.Instance.money < totalPrice)
        {
            //TODO 자금이 모자르다는 UI표시 혹은 버튼 비활성화
            Debug.LogError("Not enought money");
            return;
        }
        foreach(StoreItemIcon itemIcon in itemList)
        {
            Item item = new Item(itemIcon.itemData, itemIcon.itemData.maxQuantity, itemIcon.itemData.maxItemDurability);
            ItemIcon _itemIcon = InventoryController.Instance.GetCreateItemIcon(item);
            InventoryController.Instance.inventoryPanel.InsertItem(_itemIcon);
        }
        InventoryController.Instance.money -= totalPrice;
        InventoryController.Instance.SetMoney();
        ClearPurchasePanel();
    }

    public void SellButton()
    {
        InventoryController.Instance.money += totalPrice;
        InventoryController.Instance.SetMoney();
        foreach(Transform child in scrollRect.content)
        {
            child.GetComponent<StoreItemIcon>().item = null;
            Destroy(child.gameObject);
            for(int i = playerItemIcon.Count - 1; i >= 0; i--)
            {
                Destroy(playerItemIcon[i].gameObject);
            }
        }
        totalPrice = 0;
        priceText.text = totalPrice.ToString();
    }

    public void ClearPurchasePanel()
    {
        if(playerItemIcon.Count > 0)
        {
            for(int i = playerItemIcon.Count - 1; i >=0; i--)
            {
                InventoryController.Instance.inventoryPanel.InsertItem(playerItemIcon[i]);
                playerItemIcon.Remove(playerItemIcon[i]);
            }
            
        }
        foreach (Transform child in scrollRect.content)
        {
            child.GetComponent<StoreItemIcon>().item = null;
            Destroy(child.gameObject);
        }
        totalPrice = 0;
        priceText.text = totalPrice.ToString();
        itemList.Clear();

    }

    public void InsertPlayerItem(ItemIcon itemIcon)
    {
        if (itemList.Count > 0) { return ; }
        itemIcon.itemPanel.TakeOutItem(null, itemIcon);

        playerItemIcon.Add(itemIcon);
        itemIcon.transform.SetParent(hideItemIconParent.transform);
        SetItem(itemIcon.item);


        

        return ;
    }

    public void ResetPlayerItem()
    {
        foreach(ItemIcon itemIcon in playerItemIconList)
        {
            InventoryController.Instance.inventoryPanel.InsertItem(itemIcon);
        }
    }
}
