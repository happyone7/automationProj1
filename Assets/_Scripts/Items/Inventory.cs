using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int width = 9;
    public int height = 4;
    public ItemSlot[] slots;

    public Inventory()
    {
    }

    public Inventory(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.slots = new ItemSlot[width * height];
        
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
        }
    }

    void Awake()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = new ItemSlot[width * height];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new ItemSlot();
            }
        }
    }

    // 슬롯 가져오기
    public ItemSlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index];
    }

    public ItemSlot GetSlot(int x, int y)
    {
        int index = y * width + x;
        return GetSlot(index);
    }

    // 아이템 추가
    public int AddItem(Item item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        // 1. 같은 아이템이 있는 슬롯 먼저 확인
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CanAdd(item, amount))
            {
                amount = slots[i].Add(item, amount);
                if (amount <= 0) return 0;
            }
        }

        // 2. 빈 슬롯에 추가
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].item = item;
                slots[i].amount = Mathf.Min(amount, item.maxStack);
                amount -= slots[i].amount;
                if (amount <= 0) return 0;
            }
        }

        return amount;  // 남은 양 반환
    }

    // 아이템 제거
    public int RemoveItem(string itemId, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item?.id == itemId)
            {
                amount = slots[i].Remove(amount);
                if (amount <= 0) return 0;
            }
        }
        return amount;
    }

    // 아이템 개수 확인
    public int GetItemCount(string itemId)
    {
        int count = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item?.id == itemId)
            {
                count += slots[i].amount;
            }
        }
        return count;
    }

    // 비었는지
    public bool IsEmpty()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty) return false;
        }
        return true;
    }

    // 전체 비우기
    public void Clear()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Clear();
        }
    }
}
