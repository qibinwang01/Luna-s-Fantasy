using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //==========Item Data==========//
    public string ItemName;
    public int quality;
    public Sprite sprite;
    public bool isFull;
    public string itemDescription;
    // 保留字段以兼容 prefab 上已有的引用（删除会导致 prefab 出现 Missing 警告）
    // 实际渲染不再使用 —— ClearItem / OnLeftClick 都改为 sprite = null，与 prefab 初始的"无 sprite"状态对齐
    [System.Obsolete("已弃用，ClearItem/OnLeftClick 改用 null。请勿在 prefab 或场景里手动赋值。")]
    public Sprite emptySprite;
    [SerializeField]
    private int maxNumberOfItem;
    // 物品模板引用，由 AddItem 在初始化空格子时写入；右键使用按 itemType 分发
    public ItemSO itemSO;
    //===========Item Slot=========//
    [SerializeField]
    public TMP_Text qualityText;
    [SerializeField]
    public Image itemImage;
    //===========Item Description===//
    public Image itemDescriptionImage;
    public TMP_Text ItemDescrptionNameText;
    public TMP_Text ItemDecriptionText;
    public GameObject selectedShader;
    public bool thisItemSelected;

    // 缓存 prefab 上的默认颜色，ClearItem 时复位（避免拖拽 / DropItem 后颜色残留）
    private Color itemImageDefaultColor = Color.white;        // 兜底：子 ItemImage 应为白色
    private Color qualityTextDefaultColor = new Color(1f, 0f, 0f, 1f);  // 兜底：红色（prefab TMP 颜色）

    void Awake()
    {
        // Awake 比 Start 早一拍，能在所有外部脚本（包括 SetActive / AddItem）运行前抓到 prefab 默认色
        if (itemImage != null)
        {
            itemImageDefaultColor = itemImage.color;
        }
        if (qualityText != null)
        {
            qualityTextDefaultColor = qualityText.color;
        }
    }
    // Start is called before the first frame update
    public int AddItem(string itemName,int quality,Sprite sprite,string itemDecription,int maxStack = 9, ItemSO sourceSO = null)
    {
        // 兼容两种用法：
        // 1) Inspector 里手动配过 maxNumberOfItem → 用 Inspector 的值
        // 2) 配过 0 或没配过 → 使用调用方（ItemSO）传入的 maxStack
        int effectiveMax = maxNumberOfItem > 0 ? maxNumberOfItem : maxStack;

        if (isFull)
        {
            return quality;
        }

        // 空格子时才初始化物品信息；堆叠时不要覆盖 sprite / 描述 / itemSO
        if (string.IsNullOrEmpty(this.ItemName))
        {
            this.ItemName = itemName;
            this.sprite = sprite;
            this.itemDescription = itemDecription;
            itemImage.sprite = sprite;
            if (sourceSO != null)
            {
                this.itemSO = sourceSO;
            }
        }

        this.quality += quality;
        if (this.quality >= effectiveMax)
        {
            qualityText.text = effectiveMax.ToString();
            qualityText.enabled = true;
            isFull = true;
            int extraItem = this.quality - effectiveMax;
            this.quality = effectiveMax;
            return extraItem;
        }

        qualityText.text = this.quality.ToString();
        qualityText.enabled = true;
        return 0;
    }

    /// <summary>
    /// 清空格子（重置所有字段、UI、itemSO）。用于：消耗品归零、任务完成清空背包。
    /// 关键：itemImage.sprite 还原为 null，与 prefab 初始状态一致，避免和"从未装过东西的格子"产生视觉差异。
    /// </summary>
    public void ClearItem()
    {
        ItemName = string.Empty;
        quality = 0;
        isFull = false;
        this.sprite = null;
        itemImage.sprite = null;
        itemDescription = string.Empty;
        qualityText.text = string.Empty;
        qualityText.enabled = false;
        // 描述面板的 sprite 也还原为 null（与 prefab 默认对齐）
        if (itemDescriptionImage != null)
        {
            itemDescriptionImage.sprite = null;
        }
        if (ItemDescrptionNameText != null)
        {
            ItemDescrptionNameText.text = string.Empty;
        }
        if (ItemDecriptionText != null)
        {
            ItemDecriptionText.text = string.Empty;
        }
        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;
        itemSO = null;

        // 复位颜色到 prefab 默认值（Awake 里快照的；Awake 比 Start 早，确保拿到的是 prefab 初始色）
        if (itemImage != null)
        {
            itemImage.color = itemImageDefaultColor;
        }
        if (qualityText != null)
        {
            qualityText.color = qualityTextDefaultColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    //=============Drag To Drop==============//
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 仅响应左键拖拽；右键 / 中键不进入拖拽流程
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        if (string.IsNullOrEmpty(ItemName))
        {
            return;
        }

        // 视觉：原 slot 半透明 + 关闭 raycast（让鼠标能拖到画布外、玩家能看到"在拖"）
        SetDraggingVisual(true);
        if (itemImage != null)
        {
            itemImage.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 拖拽中不需要持续追踪鼠标位置；OnEndDrag 一次性判断是否离开画布
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 先恢复视觉（无论后续走哪条路，alpha 残留必须复位）
        SetDraggingVisual(false);
        if (itemImage != null)
        {
            itemImage.raycastTarget = true;
        }

        // 空格子 / 非左键直接 return
        if (string.IsNullOrEmpty(ItemName))
        {
            return;
        }
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        InventoryManager mgr = InventoryManager.Instance;
        if (mgr == null)
        {
            return;
        }

        // 先看终点命中了哪个 slot
        ItemSlot target = mgr.FindSlotUnderPointer(eventData.position);

        if (target == null)
        {
            // 鼠标没在任何 slot 上 → 走到画布外判定（沿用旧的丢弃路径）
            if (mgr.IsPointerOutsideInventoryCanvas(eventData.position))
            {
                bool isCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                int dropCount = isCtrl ? Mathf.Max(1, quality / 2) : quality;
                mgr.DropItem(this, dropCount);
            }
            // 鼠标在画布内但没在 slot 上 → 视为放回原位，不做事
            return;
        }

        // 拖回自己身上 → 不做事
        if (target == this)
        {
            return;
        }

        // 终点是另一个 slot → 三种分发：
        if (string.IsNullOrEmpty(target.ItemName))
        {
            // (a) 目标空 → move（整个物品搬过去）
            mgr.MoveItem(this, target);
        }
        else if (target.ItemName == ItemName && !target.isFull)
        {
            // (b) 同类未满 → merge（堆叠到 target）
            mgr.MergeItem(this, target);
        }
        else
        {
            // (c) 目标有物品但不同类，或同类但满了 → swap（互换内容）
            mgr.SwapItem(this, target);
        }
    }

    /// <summary>
    /// 拖拽中给原 slot 一个半透明视觉，避免和鼠标下方的"幽灵位置"混淆。
    /// </summary>
    private void SetDraggingVisual(bool dragging)
    {
        float alpha = dragging ? 0.4f : 1f;
        if (itemImage != null)
        {
            Color c = itemImage.color;
            c.a = alpha;
            itemImage.color = c;
        }
        if (qualityText != null)
        {
            Color c = qualityText.color;
            c.a = alpha;
            qualityText.color = c;
        }
    }

    public void OnLeftClick()
    {
        // 通过单例访问 InventoryManager（之前是缓存字段 + GameObject.Find，容易断裂）
        InventoryManager.Instance?.DeselectedAllSlots();
        if (selectedShader != null)
        {
            selectedShader.SetActive(true);
            thisItemSelected = true;
        }

        // 判断依据是"格子是否完全没有物品"（ItemName 为空），
        // 不能用 isFull —— 部分填充的格子也算"有物品"，应该正常显示
        if (string.IsNullOrEmpty(ItemName))
        {
            ItemDescrptionNameText.text = string.Empty;
            ItemDecriptionText.text = string.Empty;
            // 与 ClearItem 对齐：描述面板 Image 也设 null，避免和"从未装过东西的格子"产生渲染路径差异
            if (itemDescriptionImage != null)
            {
                itemDescriptionImage.sprite = null;
            }
        }
        else
        {
            ItemDescrptionNameText.text = ItemName;
            ItemDecriptionText.text = itemDescription;
            itemDescriptionImage.sprite = sprite;
        }
    }
    public void OnRightClick()
    {
        // 空格子右键无反应
        if (string.IsNullOrEmpty(ItemName))
        {
            return;
        }


        if (itemSO == null)
        {
            Debug.LogWarning($"[ItemSlot] {name} 没有 ItemSO 引用，无法分发右键逻辑。");
            return;
        }

        switch (itemSO.itemType)
        {
            case ItemSO.ItemType.Consumable:
                UseConsumable();
                break;
            case ItemSO.ItemType.Quest:
                // 任务道具不可消耗（蜡烛等仅用于推进剧情）
                break;
            case ItemSO.ItemType.Equipment:
                // 装备：暂未实现
                break;
        }
    }

    private void UseConsumable()
    {
        if (itemSO.healAmount <= 0)
        {
            return;
        }

        // 满血时禁止使用消耗品，避免溢出浪费 quantity
        if (GameManager.Instance.lunaCurrentHP >= GameManager.Instance.lunaHP)
        {
            return;
        }

        GameManager.Instance.AddOrDecreaseHP(itemSO.healAmount);
        quality--;
        if (quality <= 0)
        {
            ClearItem();
        }
        else
        {
            qualityText.text = quality.ToString();
        }
    }
}
