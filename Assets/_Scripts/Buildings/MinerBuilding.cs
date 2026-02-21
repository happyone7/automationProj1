using UnityEngine;

public class MinerBuilding : MonoBehaviour
{
    public Building building;
    public Item outputItem;  // 채굴로 얻는 아이템

    private float miningTimer;
    public float miningInterval = 2f;  // 채굴 간격 (초)

    void Start()
    {
        building = GetComponent<Building>();
    }

    public void OnTick(float deltaTime)
    {
        if (outputItem == null) return;

        // 출력 슬롯이 비었으면 채굴 시작
        if (building.outputSlot.IsEmpty)
        {
            miningTimer += deltaTime;

            if (miningTimer >= miningInterval)
            {
                miningTimer = 0f;
                // 아이템 생성
                building.outputSlot.Add(outputItem, 1);
            }
        }
        else
        {
            miningTimer = 0f;
        }
    }
}
