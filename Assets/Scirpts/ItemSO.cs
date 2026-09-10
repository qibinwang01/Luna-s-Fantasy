using UnityEngine;

/// <summary>
/// 物品模板（ScriptableObject）。
/// 在 Project 面板右键 → Create → Luna's Fantasy → Item 创建一个物品配置。
/// 让 Candle/Potion 等 prefab 引用同一份数据，避免每个 prefab 都重复填字段。
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Luna's Fantasy/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Consumable,   // 消耗品：使用后 quantity--（血瓶）
        Quest,        // 任务道具：仅用于推进剧情（蜡烛）
        Equipment     // 装备（蓝纹火焰锤等）
    }

    [Header("基本信息")]
    [Tooltip("唯一标识，用于背包堆叠匹配。建议用英文常量，例如 Potion/Candle。")]
    public string itemId = "Potion";

    [Tooltip("UI 上显示的名字（支持中文）。")]
    public string displayName = "Potion";

    [TextArea(2, 4)]
    [Tooltip("物品描述。")]
    public string description = "";

    [Tooltip("背包格子里的图标。")]
    public Sprite icon;

    [Tooltip("物品类型，决定右键 OnRightClick 走哪个分支。")]
    public ItemType itemType = ItemType.Consumable;

    [Header("背包参数")]
    [Min(1)]
    [Tooltip("每个格子最大堆叠数。")]
    public int maxStack = 9;

    [Header("使用效果（仅 Consumable 类型生效）")]
    [Tooltip("恢复多少 HP。")]
    public int healAmount = 40;

    [Tooltip("使用是否扣 MP，0 表示不消耗。")]
    public int mpCost = 0;

    [Header("世界实例")]
    [Tooltip("丢弃时实例化的世界 prefab（如 Candle.prefab / Potion.prefab）。留空则该物品无法被丢弃。")]
    public GameObject worldPrefab;
}