using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public Item item;
    public int amount;

    public ItemSlot()
    {
        item = null;
        amount = 0;
    }

    public ItemSlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public bool IsEmpty => item == null || amount <= 0;

    public bool CanAdd(Item newItem, int count)
    {
        if (newItem == null || count <= 0) return false;
        if (item == null) return true;
        return item.id == newItem.id && amount + count <= item.maxStack;
    }

    public int Add(Item newItem, int count)
    {
        if (newItem == null || count <= 0) return 0;

        if (item == null)
        {
            item = newItem;
            amount = Mathf.Min(count, item.maxStack);
            return count - amount;
        }

        if (item.id != newItem.id) return count;

        int space = item.maxStack - amount;
        int toAdd = Mathf.Min(space, count);
        amount += toAdd;
        return count - toAdd;
    }

    public int Remove(int count)
    {
        if (IsEmpty) return 0;
        
        int toRemove = Mathf.Min(count, amount);
        amount -= toRemove;
        
        if (amount <= 0)
        {
            item = null;
        }
        
        return toRemove;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }

    public ItemSlot Clone()
    {
        return new ItemSlot(item, amount);
    }
}
