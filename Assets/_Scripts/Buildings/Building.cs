using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData Data { get; private set; }
    public Tile Tile { get; private set; }
    public Direction CurrentDirection { get; private set; }

    [Header("Runtime State")]
    public bool isProcessing;
    public float processTimer;
    public ItemSlot inputSlot = new ItemSlot();
    public ItemSlot outputSlot = new ItemSlot();

    public void Initialize(BuildingData data)
    {
        this.Data = data;
        this.CurrentDirection = Direction.Down;
        this.inputSlot = new ItemSlot();
        this.outputSlot = new ItemSlot();
    }

    public void SetTile(Tile tile)
    {
        this.Tile = tile;
    }

    public void Initialize(int x, int y)
    {
        //建筑业가 배치될 때 초기화
        transform.position = GridManager.Instance.GridToWorld(x, y);
    }

    public void Rotate()
    {
        if (!Data.canRotate) return;
        
        CurrentDirection = CurrentDirection.RotateClockwise();
        transform.Rotate(0, 0, -90);
    }

    public void OnRemoved()
    {
        //建筑业가 제거될 때 정리
        Tile = null;
        Destroy(gameObject);
    }

    // 크래프팅 가능한지
    public bool CanProcess()
    {
        if (Data.recipe == null) return false;
        if (isProcessing) return false;
        
        // 입력 슬롯에 레시피 재료가 있는지 확인
        return HasRequiredInputs();
    }

    private bool HasRequiredInputs()
    {
        if (Data.recipe == null) return false;
        
        // 간단한 확인: 입력 슬롯에 아이템이 있고 양이 충분한지
        // 실제로는 레시피의 모든 재료를 확인해야 함
        foreach (var ingredient in Data.recipe.inputs)
        {
            if (inputSlot.item?.id == ingredient.item.id && inputSlot.amount >= ingredient.amount)
            {
                return true;
            }
        }
        return false;
    }

    // 크래프팅 시작
    public void StartProcessing()
    {
        if (!CanProcess()) return;

        isProcessing = true;
        processTimer = 0f;
    }

    // Tick - 매 프레임 호출
    public void OnTick(float deltaTime)
    {
        if (!isProcessing) return;

        processTimer += deltaTime;

        if (processTimer >= Data.processTime)
        {
            CompleteProcessing();
        }
    }

    private void CompleteProcessing()
    {
        isProcessing = false;
        processTimer = 0f;

        if (Data.recipe != null)
        {
            // 입력 아이템 소모
            foreach (var ingredient in Data.recipe.inputs)
            {
                inputSlot.Remove(ingredient.amount);
            }

            // 출력 아이템 생성
            outputSlot.Add(Data.recipe.output, Data.recipe.outputAmount);
        }
    }

    // 출력 슬롯에서 아이템 꺼내기
    public ItemSlot TryTakeOutput()
    {
        if (outputSlot.IsEmpty) return null;
        
        ItemSlot result = outputSlot.Clone();
        outputSlot.Clear();
        return result;
    }
}
