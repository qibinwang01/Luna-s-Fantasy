using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 单例（和 GameManager.Instance 对齐），ItemSlot.Start 不再用 GameObject.Find
    public static InventoryManager Instance;

    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;

    // 丢弃物品距离玩家多少单位（防止 OnTriggerEnter2D 立即把丢出的物品拾回）
    [SerializeField]
    private float dropDistance = 1.5f;

    // 记录"打开背包前"角色是否可控制，关闭背包时还原。
    // 不能简单粗暴地在 CloseMenu 里设回 true，否则在 NPC 对话中开背包再关，会把对话状态破坏。
    private bool canControlBeforeOpen;

    private void Awake()
    {
        // 单例自检：如果已有别的实例（场景里挂了多个），销毁自己
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 战斗中背包不应被唤醒；若因边缘情况（如开背包后冲进战斗）还开着，强制关闭
        if (GameManager.Instance.enterBattle)
        {
            if (InventoryMenu.activeSelf)
            {
                CloseMenu();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (InventoryMenu.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public void CloseMenu()
    {
        InventoryMenu.SetActive(false);
        //恢复时间
        Time.timeScale=1f;
        // 还原"打开背包前"的可控状态（避免破坏 NPC 对话等其他禁用控制的状态机）
        GameManager.Instance.CanControlLuna = canControlBeforeOpen;
        //关闭鼠标显示
        // Cursor.lockState=CursorLockMode.Locked;
        // Cursor.visible=false;
    }

    public void OpenMenu()
    {
        // 打开背包前先记下当时 CanControlLuna 的状态，关闭时还原
        canControlBeforeOpen = GameManager.Instance.CanControlLuna;

        InventoryMenu.SetActive(true);
        //暂停时间
        Time.timeScale=0f;
        //显示鼠标
        Cursor.lockState=CursorLockMode.None;
        Cursor.visible=true;
        // 打开背包时禁用角色控制（这样 WASD / Space 对话都不响应）
        GameManager.Instance.CanControlLuna = false;
    }

    public int AddItem(string itemName, int quality, Sprite sprite, string itemDescription, int maxStack = 9, ItemSO sourceSO = null)
    {
        int remaining = quality;

        // 第一阶段：堆叠到"同物品、未满"的格子
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            ItemSlot slot = itemSlot[i];
            if (!slot.isFull && slot.ItemName == itemName)
            {
                remaining = slot.AddItem(itemName, remaining, sprite, itemDescription, maxStack, sourceSO);
            }
        }

        // 第二阶段：剩下的数量放进"空格子"（ItemName 为空）
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            ItemSlot slot = itemSlot[i];
            if (!slot.isFull && string.IsNullOrEmpty(slot.ItemName))
            {
                remaining = slot.AddItem(itemName, remaining, sprite, itemDescription, maxStack, sourceSO);
            }
        }

        // 返回还没放下的数量（>0 表示背包满了，拾取失败）
        return remaining;
    }

    /// <summary>
    /// 按物品名清空指定数量。任务完成时调用（如蜡烛收集 5 根）。
    /// 返回剩余没清掉的数量（理论上 0）。
    /// </summary>
    public int ClearItemByName(string itemName, int quantity)
    {
        int remaining = quantity;
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            ItemSlot slot = itemSlot[i];
            if (slot.ItemName != itemName)
            {
                continue;
            }
            if (slot.quality <= remaining)
            {
                remaining -= slot.quality;
                slot.ClearItem();
            }
            else
            {
                // 这一格够用，只扣一部分
                slot.quality -= remaining;
                slot.qualityText.text = slot.quality.ToString();
                remaining = 0;
            }
        }
        return remaining;
    }
    
    public void DeselectedAllSlots()
    {
        for(int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected=false;
        }
    }

    /// <summary>
    /// 判断屏幕坐标是否在 InventoryMenu 的 RectTransform 之外。
    /// 用于 OnEndDrag 判断"是否拖出画布"。
    /// </summary>
    public bool IsPointerOutsideInventoryCanvas(Vector2 screenPos)
    {
        if (InventoryMenu == null)
        {
            return true;
        }
        RectTransform rect = InventoryMenu.transform as RectTransform;
        if (rect == null)
        {
            return true;
        }
        Canvas canvas = InventoryMenu.GetComponentInParent<Canvas>();
        // ScreenSpaceOverlay 模式传 null，其他模式传 canvas.worldCamera
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;
        return !RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, cam);
    }

    /// <summary>
    /// 检测屏幕坐标在哪个 slot 上（用于拖拽换位 / 合并）。
    /// 命中返回该 slot；鼠标没在任何 slot 上返回 null。
    /// </summary>
    public ItemSlot FindSlotUnderPointer(Vector2 screenPos)
    {
        Canvas canvas = InventoryMenu != null ? InventoryMenu.GetComponentInParent<Canvas>() : null;
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;

        for (int i = 0; i < itemSlot.Length; i++)
        {
            ItemSlot slot = itemSlot[i];
            if (slot == null)
            {
                continue;
            }
            RectTransform rect = slot.transform as RectTransform;
            if (rect == null)
            {
                continue;
            }
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, cam))
            {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// 拖拽换位（数据互换）。两边都不空且 itemId 不同时用。
    /// </summary>
    public void SwapItem(ItemSlot a, ItemSlot b)
    {
        if (a == null || b == null || a == b)
        {
            return;
        }

        SlotSnapshot sa = SnapshotSlot(a);
        SlotSnapshot sb = SnapshotSlot(b);
        ApplySlotData(sb, a);
        ApplySlotData(sa, b);

        // 互换后两边 selectedShader 都清掉 —— 两侧的内容都换过了，需要玩家重新点击选中
        if (a.selectedShader != null)
        {
            a.selectedShader.SetActive(false);
            a.thisItemSelected = false;
        }
        if (b.selectedShader != null)
        {
            b.selectedShader.SetActive(false);
            b.thisItemSelected = false;
        }
    }

    /// <summary>
    /// 拖到空格子：把 src 整体搬到 dst（dst 清空、src 清空）。
    /// </summary>
    public void MoveItem(ItemSlot src, ItemSlot dst)
    {
        if (src == null || dst == null || src == dst)
        {
            return;
        }

        SlotSnapshot s = SnapshotSlot(src);
        ApplySlotData(s, dst);
        // 搬出去后原来那格清空（视觉上的 selected 也清掉）
        src.ClearItem();
        if (dst.selectedShader != null)
        {
            dst.selectedShader.SetActive(false);
            dst.thisItemSelected = false;
        }
    }

    /// <summary>
    /// 同类未满时合并：把 src 数量堆到 dst 上。dst 满了就保留 src 剩余。
    /// </summary>
    public void MergeItem(ItemSlot src, ItemSlot dst)
    {
        if (src == null || dst == null || src == dst)
        {
            return;
        }

        // dst.AddItem 会做 maxStack 截断，并返回剩余
        int leftover = dst.AddItem(
            src.ItemName,
            src.quality,
            src.sprite,
            src.itemDescription,
            src.itemSO != null ? src.itemSO.maxStack : 9,
            src.itemSO
        );

        if (leftover > 0)
        {
            // dst 满了也没合完：保留 src 剩余数量
            src.quality = leftover;
            if (src.qualityText != null)
            {
                src.qualityText.text = leftover.ToString();
            }
            src.isFull = true;
        }
        else
        {
            // 全合进去 → src 清空
            src.ClearItem();
        }

        // 描述面板上选中状态清掉（避免互换后描述图错位）
        if (dst.selectedShader != null)
        {
            dst.selectedShader.SetActive(false);
            dst.thisItemSelected = false;
        }
    }

    // ----- 内部工具：SnapshotSlot / ApplySlotData，避免 Swap/Move 重复代码 -----
    private struct SlotSnapshot
    {
        public string itemName;
        public int quality;
        public Sprite sprite;
        public string description;
        public ItemSO itemSO;
        public bool isFull;
    }

    private static SlotSnapshot SnapshotSlot(ItemSlot s)
    {
        return new SlotSnapshot
        {
            itemName = s.ItemName,
            quality = s.quality,
            sprite = s.sprite,
            description = s.itemDescription,
            itemSO = s.itemSO,
            isFull = s.isFull,
        };
    }

    private static void ApplySlotData(SlotSnapshot data, ItemSlot target)
    {
        target.ItemName = data.itemName;
        target.quality = data.quality;
        target.sprite = data.sprite;
        target.itemDescription = data.description;
        target.itemSO = data.itemSO;
        target.isFull = data.isFull;

        if (target.itemImage != null)
        {
            target.itemImage.sprite = data.sprite;
        }
        if (target.qualityText != null)
        {
            if (data.quality > 0)
            {
                target.qualityText.text = data.quality.ToString();
                target.qualityText.enabled = true;
            }
            else
            {
                target.qualityText.text = string.Empty;
                target.qualityText.enabled = false;
            }
        }
    }

    /// <summary>
    /// 从背包丢弃指定数量。会在玩家位置附近随机方向生成 count 个世界 prefab，
    /// 并从对应格子扣除 quantity（归零则 ClearItem）。
    /// </summary>
    public void DropItem(ItemSlot slot, int count)
    {
        if (slot == null || slot.itemSO == null || slot.itemSO.worldPrefab == null)
        {
            Debug.LogWarning($"[InventoryManager] 无法丢弃 {slot?.ItemName ?? "null"}：ItemSO 缺少 worldPrefab 引用，请在 ItemSO.asset 的 Inspector 里拖入对应的世界 prefab。");
            return;
        }

        int dropCount = Mathf.Min(count, slot.quality);
        if (dropCount <= 0)
        {
            return;
        }

        GameObject luna = GameObject.FindWithTag("Luna");
        if (luna == null)
        {
            Debug.LogWarning("[InventoryManager] 找不到 tag=Luna 的玩家对象，无法生成丢弃物品。");
            return;
        }

        // 丢弃方向：相对玩家的随机单位向量 × dropDistance
        // 这样多个物品不会完全重叠，也避免触发器立即拾回
        Vector2 dir = Random.insideUnitCircle;
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = Vector2.right;
        }
        dir.Normalize();
        Vector3 dropPos = luna.transform.position + new Vector3(dir.x, dir.y, 0f) * dropDistance;

        for (int i = 0; i < dropCount; i++)
        {
            Instantiate(slot.itemSO.worldPrefab, dropPos, Quaternion.identity);
        }

        // 扣数量（归零则清格）
        slot.quality -= dropCount;
        if (slot.quality <= 0)
        {
            slot.ClearItem();
        }
        else
        {
            slot.qualityText.text = slot.quality.ToString();
        }
    }
}
